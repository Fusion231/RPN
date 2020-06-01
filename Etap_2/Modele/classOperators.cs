using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
namespace Etap2
{
    public class classOperators{
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
}