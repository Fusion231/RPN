using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
namespace Etap2 {
	public class error: Exception {
		public error(string message) : base(message) {}
	}
	public class classOnp {
		public string inflix {
			get;
			set;
		}
		public string status {
			get;
			set;
		}
		public string error {
			get;
			set;
		}
		public List < string > onpOutput;
		public double x {
			get;
			set;
		}
		public double x_min {
			get;
			set;
		}
		public double x_max {
			get;
			set;
		}
		public double n {
			get;
			set;
		}
		private classOperators operatory;
		public classOnp(string formula, double x) {
			this.x = x;
			this.x_max = double.NaN;
			this.status = "ok";
			this.operatory = new classOperators();
			this.inflix = checkInput(formula);
		}
		public classOnp(string formula, double from, double to, double n) {
			this.x_min = from;
			this.x_max = to;
			this.n = n;
			this.status = "ok";
			this.operatory = new classOperators();
			this.inflix = checkInput(formula);
		}
		public classOnp(string args) {
			this.x = double.NaN;
			this.status = "ok";
			this.operatory = new classOperators();
			this.inflix = checkInput(args);
		}
		//Sprawdza całe wejscie
		private string checkInput(string formula) {
			string input = formula;
			input = input.Replace(" ", string.Empty);
			string separator = ".";
			input = input.Replace(".", separator);
			input = input.Replace(",", separator);
			input = Regex.Replace(input, @"^\-", @"-");
			if (!checkcorrectSigns(input)) {
				throw new error("Coś jest źle z równaniem: Niedozwolony znak");
			}
			if (!checkBrackets(input)) {
				throw new error("Coś jest źle z równaniem: problem z nawiasami");
			}
			if (!checkSigns(input)) {
				throw new error("Coś jest źle z równaniem: nie mogą być 2 znaki koło siebie");
			}
			if (!checkbegoreBrackets(input)) {
				throw new error("Coś jest źle z równaniem: po cyfrze nie wystepuje symbol przed nawiasem otwierającym");
			}
			if (!checkDots(input)) {
				throw new error("Coś jest źle z równaniem: cyfra z 2 kropkami");
			}
			return input;
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
			for (int w = 0; w <= this.onpOutput.Count; w++) {
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
					this.onpOutput.Add(" ");
					this.onpOutput.Add(ONP_stos.Pop());
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
							this.onpOutput.Add(" ");
							this.onpOutput.Add(ONP_stos.Pop());
						}
					}
					break;
				}
				this.onpOutput.Add(" ");
				this.onpOutput.Add(ONP_stos.Pop());
			}
		}
		//glowna funkcja przeksztalcenia na RPN
		public void RPN() {
			if (this.status == "error") {
				return;
			}
			this.onpOutput = new List < string > ();
			Stack < string > ONP_stos = new Stack < string > ();
			string tempValue = "";
			this.inflix = Regex.Replace(this.inflix, @"\s+", String.Empty);
			for (int q = 0; q < this.inflix.Length; q++) {
				if (this.status == "error") {
					break;
				}
				string character = this.inflix[q].ToString();
				//Jeżeli jest operatorem
				if (Regex.IsMatch(character, @"^[\+|\-|\^|\*|/]$")) {
					if (q != 0 && Regex.IsMatch(this.inflix[q - 1].ToString(), @"\($") && (Regex.IsMatch(this.inflix[q + 1].ToString(), @"\d$") || Regex.IsMatch(this.inflix[q + 1].ToString(), @"^[a-zA-Z]+$"))) {
						tempValue = "-";
						continue;
					} else if (q == 0 && Regex.IsMatch(this.inflix[q + 1].ToString(), @"\d$")) {
						tempValue = "-";
						continue;
					}
					if (this.operatory.podstawowe.TryGetValue(character, out int priorytet_elementu)) {
						if (character == "/" && this.inflix[q + 1] == '0') {
							throw new error("Coś jest źle z równaniem: nie dziel przez 0");
						}
						int priorytet_elementu_z_stosu = 0;
						if (ONP_stos.Count != 0) {
							priorytet_elementu_z_stosu = this.operatory.podstawowe.FirstOrDefault(x =>x.Key == ONP_stos.Peek()).Value;
						}
						if (priorytet_elementu > priorytet_elementu_z_stosu) {
							ONP_stos.Push(character);
						} else {
							if (ONP_stos.Count == 0) {
								ONP_stos.Push(character);
							} else if (ONP_stos.Count != 0) {
								removingStack(character, ONP_stos);
							}
						}
					} else {
						throw new error("Coś jest źle z równaniem: Zly operator");
					}
					//Jeżeli jest liczbą
				} else if (Regex.IsMatch(character, @"^\d$")) {
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
							this.onpOutput.Add(" ");
							this.onpOutput.Add(tempValue);
							tempValue = "";
						} else {
							tempValue += character;
							this.onpOutput.Add(" ");
							this.onpOutput.Add(tempValue);
							tempValue = "";
						}
					} else {
						if (character != "0") {
							tempValue += character;
							this.onpOutput.Add(" ");
							this.onpOutput.Add(tempValue);
							tempValue = "";
						}
					}
					//Jeżeli jest nawiasem (
				} else if (character == "(") {
					ONP_stos.Push(character);
					//Jeżeli jest nawiasem )
				} else if (character == ")") {
					removingStack(ONP_stos);
					//Jeżeli jest literą
				} else if (Regex.IsMatch(character, @"^[a-zA-Z]+$")) {
					if (character == "x") {
						this.onpOutput.Add(" ");
						if (tempValue == "-") {
							this.onpOutput.Add("-x");
						} else {
							this.onpOutput.Add("x");
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
							throw new error("Coś jest źle z równaniem: Brak takiej funkcji lub blednie wpisana funkcja");
						}
						if (tempValue[0] == '-') {
							tempValue = tempValue.Replace("-", "");
							if (this.operatory.pr_method.ContainsKey(tempValue)) {
								if (tempValue == "pi") {
									this.onpOutput.Add(" ");
									this.onpOutput.Add("-pi");
									tempValue = "";
								} else {
									ONP_stos.Push("-" + tempValue);
									tempValue = "";
								}
							}
						} else {
							if (this.operatory.pr_method.ContainsKey(tempValue)) {
								if (tempValue == "pi") {
									this.onpOutput.Add(" ");
									this.onpOutput.Add("pi");
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
						this.onpOutput.Add(" ");
						this.onpOutput.Add(e.ToString());
					}
				}
			}
			this.onpOutput.Add(" ");
		}
		//Wyswietla RPN
		public string displayRPN() {
			string displayString = "";
			foreach(string str in this.onpOutput) {
				displayString += str.ToString();
			}
			return displayString;
		}
		//Funkcja do obliczenia wyniku RPN
		public string calculateRPN(double wartosc) {
			Stack < double > stosDigital = new Stack < double > ();
			string tempValue = "",
			wyrazenie_ONP = displayRPN();
			for (int q = 0; q < wyrazenie_ONP.Length; q++) {
				//Jeżeli jest operatorem
				if (Regex.IsMatch(wyrazenie_ONP[q].ToString(), @"^[\+|\-|\^|\*|/]$")) {
					if (wyrazenie_ONP.Length - 1 > q) {
						if (Regex.IsMatch(wyrazenie_ONP[q + 1].ToString(), @"\d$")) {
							tempValue = "-";
							continue;
						}
						if (Regex.IsMatch(wyrazenie_ONP[q].ToString(), @"^[\-]$") && Regex.IsMatch(wyrazenie_ONP[q + 1].ToString(), @"^[a-zA-Z]+$")) {
							stosDigital.Push(stosDigital.Pop() * ( - 1));
							continue;
						}
					}
					double liczba1 = stosDigital.Pop(),
					liczba2 = 0;
					if (stosDigital.Count != 0) {
						liczba2 = stosDigital.Pop();
					}
					if (liczba1 < 0 && wyrazenie_ONP[q] == '+') {
						stosDigital.Push(liczba2 - (liczba1 * -1));
					} else {
						switch (wyrazenie_ONP[q]) {
						case '+':
							stosDigital.Push(liczba2 + liczba1);
							break;
						case '-':
							if (liczba1 < 0) {
								stosDigital.Push(liczba2 + (liczba1 * -1));
							} else {
								stosDigital.Push(liczba2 - liczba1);
							}
							break;
						case '*':
							if (liczba1 < 0 && liczba2 < 0) {
								stosDigital.Push((liczba2 * ( - 1)) * (liczba1 * ( - 1)));
							} else if (liczba1 < 0) {
								stosDigital.Push(liczba2 * (liczba1 * ( - 1)) * -1);
							} else if (liczba2 < 0) {
								stosDigital.Push((liczba2 * ( - 1)) * liczba1 * -1);
							} else {
								stosDigital.Push(liczba2 * liczba1);
							}
							break;
						case '/':
							if (liczba1 <= 0) {
								throw new error("Coś jest źle z równaniem: nie dziel przez 0");
							}
							stosDigital.Push(liczba2 / liczba1);
							break;
						case '^':
							stosDigital.Push(Math.Pow(liczba2, liczba1));
							break;
						}
					}
					//Jeżeli jest liczbą
				} else if (Regex.IsMatch(wyrazenie_ONP[q].ToString(), @"^\d$")) {
					if (wyrazenie_ONP.Length > q) {
						if (Regex.IsMatch(wyrazenie_ONP[q + 1].ToString(), @"\d$") || Regex.IsMatch(wyrazenie_ONP[q + 1].ToString(), @"\.$")) {
							for (int r = q; r < wyrazenie_ONP.Length; r++) {
								if (Regex.IsMatch(wyrazenie_ONP[r].ToString(), @"^[\.]$")) {} else if (!Regex.IsMatch(wyrazenie_ONP[r].ToString(), @"^\d$")) {
									q = r;
									break;
								}
								tempValue += wyrazenie_ONP[r];
							}
							stosDigital.Push(Convert.ToDouble(tempValue, System.Globalization.CultureInfo.InvariantCulture)); //CultureInfo jest użyte bo bez tego nie działa konwersacja z kropką
							tempValue = "";
						} else {
							tempValue += wyrazenie_ONP[q];
							stosDigital.Push(Convert.ToDouble(tempValue, System.Globalization.CultureInfo.InvariantCulture)); //CultureInfo jest użyte bo bez tego nie działa konwersacja z kropką
							tempValue = "";
						}
					}
					//Jeżeli jest literą
				} else if (Regex.IsMatch(wyrazenie_ONP[q].ToString(), @"^[a-zA-Z]+$")) {
					if (wyrazenie_ONP[q].ToString() == "x") {
						stosDigital.Push(wartosc);
					} else {
						for (int w = q; w <= wyrazenie_ONP.Length; w++) {
							if (w >= wyrazenie_ONP.Length || !Regex.IsMatch(wyrazenie_ONP[w].ToString(), @"^[a-zA-Z]+$")) {
								q = w;
								break;
							}
							tempValue += wyrazenie_ONP[w].ToString();
						}
						switch (tempValue) {
						case "log":
							if (stosDigital.Peek() < 0) {
								throw new error("Coś jest źle z równaniem: liczba w logarytnie jest mniejsza od 0");
							}
							break;
						case "-log":
							if (stosDigital.Peek() < 0) {
								throw new error("Coś jest źle z równaniem: liczba w logarytnie jest mniejsza od 0");
							}
							break;
						case "sqrt":
							if (stosDigital.Peek() < 0) {
								throw new error("Coś jest źle z równaniem: Nie ma pierwiastka z ujemnej liczby");
							}
							break;
						case "asin":
							if (stosDigital.Peek() > 1 || stosDigital.Peek() < -1) {
								throw new error("Coś jest źle z równaniem: asin ma przedzial (-1,1)");
							}
							break;
						case "pi":
							stosDigital.Push(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(1)));
							tempValue = "";
							break;
						case "acos":
							if (stosDigital.Peek() > 1 || stosDigital.Peek() < -1) {
								throw new error("Coś jest źle z równaniem: acos ma przedzial (-1,1)");
							}
							break;
						default:
							break;
						}
						if (Double.IsNaN(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(stosDigital.Pop())))) {
							throw new error("Coś jest źle z równaniem: Nieprawidłowa wartość");
						}
						stosDigital.Push(Convert.ToDouble(this.operatory.pr_method.FirstOrDefault(x =>x.Key == tempValue).Value(stosDigital.Pop())));
						tempValue = "";
					}
				}
			}
			if (Double.IsNaN(stosDigital.Peek())) {
				throw new error("Coś jest źle z równaniem: Zla skladnia rownania");
			}
			return stosDigital.Pop().ToString();
		}
	}
}