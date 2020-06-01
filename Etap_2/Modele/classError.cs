using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
namespace Etap2
{
    public class classError
    {
        public string status {get;set;}
        public string message {get;set;}
        public classError(string message){ 
            this.status = "error";
            this.message = message;
        }
    }
}