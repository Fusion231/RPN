using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
namespace Etap2
{
    public class classToken
    {
        public string[] inflix {get;set;}
        public string[] RPN {get;set;}
        public classToken(string[] x, string[] y){
            this.inflix=x;
            this.RPN=y;
        }
    }
}