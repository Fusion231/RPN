using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace NET {
	class Program {
		public class RPN {
			public string inflix;
			public List < string > ONP;
			public double x;
			public double x_min;
			public double x_max;
			public double n;
			private Operatory operatory;
			public RPN(string[] args) {
				this.inflix = checkInput(args[0]);
				this.x = Convert.ToDouble(args[1]);
				this.x_min = Convert.ToDouble(args[2]);
				this.x_max = Convert.ToDouble(args[3]);
				this.n = Convert.ToDouble(args[4]);
				this.operatory = new Operatory();
			}
			private string checkInput(string wartosc) {
				string wejscie = wartosc;
				wejscie = wejscie.Replace(" ", string.Empty);
				string separator = ".";
				wejscie = wejscie.Replace(".", separator);
				wejscie = wejscie.Replace(",", separator);
				wejscie = Regex.Replace(wejscie, @"^\-", @"-");
				if (!checkcorrectSigns(wejscie)) {
					throw new Exception("Coś jest źle z równaniem: niedozwolony znak");
				}
				if (!checkBrackets(wejscie)) {
					throw new Exception("Coś jest źle z równaniem: problem z nawiasami");
				}
				if (!checkSigns(wejscie)) {
					throw new Exception("Coś jest źle z równaniem: nie mogą być 2 symbole koło siebie");
				}
				if (!checkbegoreBrackets(wejscie)) {
					throw new Exception("Coś jest źle z równaniem: po cyfrze nie wystepuje symbol przed nawiasem otwierającym");
				}
				if (!checkDots(wejscie)) {
					throw new Exception("Coś jest źle z równaniem: 2 kropki w cyfrze");
				}
				return wejscie;
			}
			//Sprawdza poprawnosc znakow niedozwolonych
        private bool checkcorrectSigns(string input) {
            foreach(char character in input) {
                if (Regex.IsMatch(character.ToString(), @"[\\ | \@ | \! | \# | \$ | \` | \~]")) {
                    return false;
                }
            }
            return true;
        }
		//Sprawdza poprawnosc kropek
		private bool checkDots(string input) {
			int number_dots = 0,
			number_digital = 0;
			foreach(char character in input) {
				if (Regex.IsMatch(character.ToString(), @"^[\+|\-|\^|\*|/]$")) {
					if (number_dots > 1) {
						return false;
					} else {
						number_dots = 0;
					}
				}
				if (Regex.IsMatch(character.ToString(), @"\d")) {
					number_digital++;
				}
				if (character == '.') {
					number_dots++;
				}
			}
			if (number_dots > 1) {
				return false;
			} else {
				return true;
			}
		}
		//Sprawdza poprawnosc nawiasow
		private bool checkBrackets(string input) {
			int number = 0;
			foreach(char character in input) {
				if (character == '(') {
					number++;
				}
				if (character == ')') {
					number--;
				}
			}
			if (number == 0) {
				return true;
			} else {
				return false;
			}

		}
		//Sprawdza poprawnosc znakow
		private bool checkSigns(string input) {
			if (Regex.IsMatch(input, @"[\+|\-|\^|\*|/]([\+|\-|\^|\*|/])")) {
				return false;
			}
			return true;
		}
		//Sprawdza poprawnosc przez nawiasem
		private bool checkbegoreBrackets(string input) {
			if (Regex.IsMatch(input, @"\d([\(])")) {
				return false;
			}
			return true;
		}
		//Funkcja dla operatorów
		private void removingStack(string character, Stack < string > ONP_stos) {
			int prioritymainElement = this.operatory.podstawowe.FirstOrDefault(x =>x.Key == character).Value;
			for (int w = 0; w <= this.ONP.Count; w++) {
				if (ONP_stos.Count == 0 || ONP_stos.Peek() == "(") {
					ONP_stos.Push(character);
					break;
				}
				int priorityElement = this.operatory.podstawowe.FirstOrDefault(x =>x.Key == ONP_stos.Peek()).Value;
				if (prioritymainElement > priorityElement) {
					ONP_stos.Push(character);
					break;
				}
				if (priorityElement >= prioritymainElement) {
					this.ONP.Add(" ");
					this.ONP.Add(ONP_stos.Pop());
				}
			}
		}
		//Funkcja dla nawiasu
		private void removingStack(Stack < string > ONP_stos) {
			for (int w = 0; w <= ONP_stos.Count; w++) {
				if (ONP_stos.Peek() == "(") {
					ONP_stos.Pop();
					if (ONP_stos.Count != 0) {
						if (ONP_stos.Peek().Length > 1) {
							this.ONP.Add(" ");
							this.ONP.Add(ONP_stos.Pop());
						}
					}
					break;
				}
				this.ONP.Add(" ");
				this.ONP.Add(ONP_stos.Pop());
			}
		}
		//glowna funkcja przeksztalcenia na RPN
			public void translateRPN() {
				this.ONP = new List < string > ();
				Stack < string > ONP_stos = new Stack < string > ();
				string tempValue = "";
				this.inflix = Regex.Replace(this.inflix, @"\s+", String.Empty);
				for (int q = 0; q < this.inflix.Length; q++) {
					string tempSymbol = this.inflix[q].ToString();
					if (Regex.IsMatch(tempSymbol, @"^[\+|\-|\^|\*|/]$")) { //Jeżeli jest operatorem
						if (q != 0 && Regex.IsMatch(this.inflix[q - 1].ToString(), @"\($") && (Regex.IsMatch(this.inflix[q + 1].ToString(), @"\d$") || Regex.IsMatch(this.inflix[q + 1].ToString(), @"^[a-zA-Z]+$"))) {
							tempValue = "-";
							continue;
						} else if (q == 0 && Regex.IsMatch(this.inflix[q + 1].ToString(), @"\d$")) {
							tempValue = "-";
							continue;
						}
						if (this.operatory.podstawowe.TryGetValue(tempSymbol, out int priorytet_elementu)) {
							if (tempSymbol == "/" && this.inflix[q + 1] == '0') {
								throw new Exception("Coś jest źle z równaniem: nie dziel przez 0");
							}
							int priorytet_elementu_z_stosu = 0;
							if (ONP_stos.Count != 0) {
								priorytet_elementu_z_stosu = this.operatory.podstawowe.FirstOrDefault(x =>x.Key == ONP_stos.Peek()).Value;
							}
							if (priorytet_elementu > priorytet_elementu_z_stosu) {
								ONP_stos.Push(tempSymbol);
							} else {
								if (ONP_stos.Count == 0) {
									ONP_stos.Push(tempSymbol);
								} else if (ONP_stos.Count != 0) {
									removingStack(tempSymbol, ONP_stos);
								}
							}
						} else {
							throw new Exception("Coś jest źle z równaniem: Zly operator");
						}
					} else if (Regex.IsMatch(tempSymbol, @"^\d$")) { //Jeżeli jest liczbą
						if (this.inflix.Length - 1 > q) {
							if (Regex.IsMatch(this.inflix[q + 1].ToString(), @"^\d$") || Regex.IsMatch(this.inflix[q + 1].ToString(), @"^[\.]$")) {
								for (int r = q; r <= this.inflix.Length; r++) {
									if (this.inflix.Length <= r) {
										q = r - 1;
										break;
									}
									if (Regex.IsMatch(this.inflix[r].ToString(), @"^[\.]$")) {} else if (!Regex.IsMatch(this.inflix[r].ToString(), @"^\d$")) {
										q = r - 1;
										break;
									}
									tempValue += this.inflix[r];
								}
								this.ONP.Add(" ");
								this.ONP.Add(tempValue);
								tempValue = "";
							} else {
								tempValue += tempSymbol;
								this.ONP.Add(" ");
								this.ONP.Add(tempValue);
								tempValue = "";
							}
						} else {
							if (tempSymbol != "0") {
								tempValue += tempSymbol;
								this.ONP.Add(" ");
								this.ONP.Add(tempValue);
								tempValue = "";
							}
						}
					} else if (tempSymbol == "(") { //Jeżeli jest nawiasem (
						ONP_stos.Push(tempSymbol);
					} else if (tempSymbol == ")") { //Jeżeli jest nawiasem )
						removingStack(ONP_stos);
					} else if (Regex.IsMatch(tempSymbol, @"^[a-zA-Z]+$")) { //Jeżeli jest literą
						if (tempSymbol == "x") {
							this.ONP.Add(" ");
							if (tempValue == "-") {
								this.ONP.Add("-x");
							} else {
								this.ONP.Add("x");
							}
							tempValue = "";
						} else {
							for (int w = 0; w < this.inflix.Length; w++) {
								if (this.inflix.Length <= q + w || !Regex.IsMatch(this.inflix[q + w].ToString(), @"^[a-zA-Z]+$")) {
									q += w - 1;
									break;
								}
								tempValue += this.inflix[q + w].ToString();
							}
							if (!this.operatory.pr_method.ContainsKey(tempValue)) {
								throw new Exception("Coś jest źle z równaniem: Brak takiej funkcji lub blednie wpisana funkcja");
							}
							if (tempValue[0] == '-') {
								tempValue = tempValue.Replace("-", "");
								if (this.operatory.pr_method.ContainsKey(tempValue)) {
									if (tempValue == "pi") {
										this.ONP.Add(" ");
										this.ONP.Add("-pi");
										tempValue = "";
									} else {
										ONP_stos.Push("-" + tempValue);
										tempValue = "";
									}
								}
							} else {
								if (this.operatory.pr_method.ContainsKey(tempValue)) {
									if (tempValue == "pi") {
										this.ONP.Add(" ");
										this.ONP.Add("pi");
										tempValue = "";
									} else {
										ONP_stos.Push(tempValue);
										tempValue = "";
									}
								}
							}
						}
					}
					if (q + 1 >= this.inflix.Length && ONP_stos.Count != 0) {
						foreach(Object e in ONP_stos) {
							this.ONP.Add(" ");
							this.ONP.Add(e.ToString());
						}
					}
				}
				this.ONP.Add(" ");
			}
			public string displayRPN() {
				string displayString = "";
				foreach(string str in this.ONP) {
					displayString += str.ToString();
				}
				return displayString;
			}
			public string calculateRPN(double wartosc) {
				Stack < double > stosdigital = new Stack < double > ();
				string tempValue = "",
				stringRPN = displayRPN();
				for (int q = 0; q < stringRPN.Length; q++) {
					if (Regex.IsMatch(stringRPN[q].ToString(), @"^[\+|\-|\^|\*|/]$")) { //Jeżeli jest operatorem
						if (stringRPN.Length - 1 > q) {
							if (Regex.IsMatch(stringRPN[q + 1].ToString(), @"\d$")) {
								tempValue = "-";
								continue;
							}
							if (Regex.IsMatch(stringRPN[q].ToString(), @"^[\-]$") && Regex.IsMatch(stringRPN[q + 1].ToString(), @"^[a-zA-Z]+$")) {
								stosdigital.Push(stosdigital.Pop() * ( - 1));
								continue;
							}
						}
						double liczba1 = stosdigital.Pop(),
						liczba2 = 0;
						if (stosdigital.Count != 0) {
							liczba2 = stosdigital.Pop();
						}
						if (liczba1 < 0 && stringRPN[q] == '+') {
							stosdigital.Push(liczba2 - (liczba1 * -1));
						} else {
							switch (stringRPN[q]) {
							case '+':
								stosdigital.Push(liczba2 + liczba1);
								break;
							case '-':
								if (liczba1 < 0) {
									stosdigital.Push(liczba2 + (liczba1 * -1));
								} else {
									stosdigital.Push(liczba2 - liczba1);
								}
								break;
							case '*':
								if (liczba1 < 0 && liczba2 < 0) {
									stosdigital.Push((liczba2 * ( - 1)) * (liczba1 * ( - 1)));
								} else if (liczba1 < 0) {
									stosdigital.Push(liczba2 * (liczba1 * ( - 1)) * -1);
								} else if (liczba2 < 0) {
									stosdigital.Push((liczba2 * ( - 1)) * liczba1 * -1);
								} else {
									stosdigital.Push(liczba2 * liczba1);
								}
								break;
							case '/':
								if (liczba1 <= 0) {
									throw new Exception("Coś jest źle z równaniem: nie dziel przez 0");
								}
								stosdigital.Push(liczba2 / liczba1);
								break;
							case '^':
								stosdigital.Push(Math.Pow(liczba2, liczba1));
								break;
							}
						}
					} else if (Regex.IsMatch(stringRPN[q].ToString(), @"^\d$")) { //Jeżeli jest liczbą
						if (stringRPN.Length > q) {
							if (Regex.IsMatch(stringRPN[q + 1].ToString(), @"\d$") || Regex.IsMatch(stringRPN[q + 1].ToString(), @"\.$")) {
								for (int r = q; r < stringRPN.Length; r++) {
									if (Regex.IsMatch(stringRPN[r].ToString(), @"^[\.]$")) {} else if (!Regex.IsMatch(stringRPN[r].ToString(), @"^\d$")) {
										q = r;
										break;
									}
									tempValue += stringRPN[r];
								}
								stosdigital.Push(Convert.ToDouble(tempValue, System.Globalization.CultureInfo.InvariantCulture)); //CultureInfo jest użyte bo bez tego nie działa konwersacja z kropką
								tempValue = "";
							} else {
								tempValue += stringRPN[q];
								stosdigital.Push(Convert.ToDouble(tempValue, System.Globalization.CultureInfo.InvariantCulture)); //CultureInfo jest użyte bo bez tego nie działa konwersacja z kropką
								tempValue = "";
							}
						}
					} else if (Regex.IsMatch(stringRPN[q].ToString(), @"^[a-zA-Z]+$")) { //Jeżeli jest literą
						if (stringRPN[q].ToString() == "x") {
							stosdigital.Push(wartosc);
						} else {
							for (int w = q; w <= stringRPN.Length; w++) {
								if (w >= stringRPN.Length || !Regex.IsMatch(stringRPN[w].ToString(), @"^[a-zA-Z]+$")) {
									q = w;
									break;
								}
								tempValue += stringRPN[w].ToString();
							}
							switch (tempValue) {
							case "log":
								if (stosdigital.Peek() < 0) {
									throw new Exception("Coś jest źle z równaniem: liczba w logarytnie jest mniejsza od 0");
								}
								break;
							case "-log":
								if (stosdigital.Peek() < 0) {
									throw new Exception("Coś jest źle z równaniem: liczba w logarytnie jest mniejsza od 0");
								}
								break;
							case "sqrt":
								if (stosdigital.Peek() < 0) {
									throw new Exception("Coś jest źle z równaniem: Nie ma pierwiastka z ujemnej liczby");
								}
								break;
							case "asin":
								if (stosdigital.Peek() > 1 || stosdigital.Peek() < -1) {
									throw new Exception("Coś jest źle z równaniem: asin ma przedzial (-1,1)");
								}
								break;
							case "pi":
								stosdigital.Push(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(1)));
								tempValue = "";
								break;
							case "acos":
								if (stosdigital.Peek() > 1 || stosdigital.Peek() < -1) {
									throw new Exception("Coś jest źle z równaniem: acos ma przedzial (-1,1)");
								}
								break;
							default:
								break;
							}
							if (Double.IsNaN(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(stosdigital.Pop())))) {
								throw new Exception("Coś jest źle z równaniem: Nieprawidłowa wartość");
							}
							stosdigital.Push(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(stosdigital.Pop())));
							tempValue = "";
						}

					}
				}
				if (Double.IsNaN(stosdigital.Peek())) {
					throw new Exception("Zla skladnia rownania");
				}
				return stosdigital.Pop().ToString();
			}
			public void calculatenextRPN() {
				double liczba_dodawana = (this.x_max - this.x_min) / (this.n - 1),
				tymczasowe_min = this.x_min;
				for (double q = 0; q < this.n; q++) {
					System.Console.WriteLine(tymczasowe_min + " => " + this.calculateRPN(tymczasowe_min));
					tymczasowe_min += liczba_dodawana;
				}
			}
		}
        public class Operatory{
            public Stack<string> operatory = new Stack<string>();
            public Dictionary<string,int> podstawowe = new Dictionary<string, int>() {
                {"(",0},
                {"+",1},
                {"-",1},
                {"*",2},
                {"/",2},
                {"^",3}
            };
            public Dictionary<string, Func<double,double>> pr_method = new Dictionary<string, Func<double,double>>(){
                {"sin",(v1)=>Math.Sin(v1)},
                {"cos",(v1)=>Math.Cos(v1)},
                {"exp",(v1)=>Math.Exp(v1)},
                {"abs",(v1)=>Math.Abs(v1)},
                {"asin",(v1)=>Math.Asin(v1)},
                {"log",(v1)=>Math.Log(v1)},
                {"sqrt",(v1)=>Math.Sqrt(v1)},
                {"tan",(v1)=>Math.Tan(v1)},
                {"cosh",(v1)=>Math.Cosh(v1)},
                {"sinh",(v1)=>Math.Sinh(v1)},
                {"tanh",(v1)=>Math.Tanh(v1)},
                {"acos",(v1)=>Math.Acos(v1)},
                {"atan",(v1)=>Math.Atan(v1)},
                {"pi",(v1)=>Math.PI}
            };
        };
        static void Main(string[] args)
        {
            if(args!=null && args.Length==5){
                RPN Klasa_wyrazenia = new RPN(args);
                Klasa_wyrazenia.translateRPN();
                System.Console.WriteLine(Klasa_wyrazenia.inflix);
                System.Console.WriteLine(Klasa_wyrazenia.displayRPN());
                System.Console.WriteLine(Klasa_wyrazenia.calculateRPN(Klasa_wyrazenia.x)); 
                Klasa_wyrazenia.calculatenextRPN(); 
            }else{
                throw new Exception("Coś jest źle: problem z parametrami wejsciowymi");
            }
            
        }
    }
}