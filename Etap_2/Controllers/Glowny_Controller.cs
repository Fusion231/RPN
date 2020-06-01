using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
namespace Etap2.Controllers
{
    
    public class Glowny_controller : ControllerBase
    {

        //funkcja do przesylu wynikow
        public dynamic sendAnswer(classOnp onp){
            try{
                onp.RPN();
                    //dla calculate/xt
                    if(!Double.IsNaN(onp.x_max) && !Double.IsNaN(onp.x)){
                        List<classCalculate> lista = new List<classCalculate>();
                        double liczba_dodawana = (onp.x_max - onp.x_min) / (onp.n - 1), tymczasowe_min = onp.x_min; 
                        Math.Round(liczba_dodawana,15);
                        for(double q=0;q<onp.n;q++){
                            Math.Round(tymczasowe_min,15);
                            lista.Add(new classCalculate(Math.Round(tymczasowe_min,10),Convert.ToDouble(onp.calculateRPN(tymczasowe_min))));
                            tymczasowe_min+=liczba_dodawana;
                        }
                        var data = new {
                            status = onp.status,
                            result = lista
                        };
                        return data;
                    //dla calculate
                    }else if(Double.IsNaN(onp.x_max)){
                        var data = new {
                            status = onp.status,
                            result = onp.calculateRPN(onp.x)
                        };
                        return data;
                    //dla tokens
                    }else{
                        List<classToken> lista = new List<classToken>();
                        classToken tymczasowe = new classToken(Regex.Split(onp.inflix, @"([*()\^\/]|(?<!E)[\+\-])" ),onp.displayRPN().Trim().Split(" "));
                        lista.Add(tymczasowe);
                        var data = new {
                            status = onp.status,
                            result = lista
                        };
                        return data;
                    }
            }catch(error er){
                throw new error(er.Message);
            }catch(Exception ex){
                throw new error(ex.Message);
            }
        }
        //Funkcja wysyla Error
        private dynamic sendError(error er){
            classError model = new classError(er.Message); 
            var data = new {
                status = model.status,
                result = model.message
            };
            return data;
        }
        
        [HttpGet]
        [Produces("application/json")]
        [Route("api/tokens")]
        public IActionResult tokens(string formula)
        {
            try{
                formula=formula.Replace(' ','+' );
                classOnp onp = new classOnp(formula);
                var data = sendAnswer(onp);
                return Ok(data);
            }catch(error er){
                return Ok(sendError(er));
            }
        }
        [HttpGet]
        [Produces("application/json")]
        [Route("api/calculate")]
        public IActionResult tokens(string formula,double x)
        {   
            try{
                formula=formula.Replace(' ','+' );
                classOnp onp = new classOnp(formula,x);
                var data = sendAnswer(onp);
                return Ok(data);
            }catch(error er){
                return Ok(sendError(er));
            }
        }
        [HttpGet]
        [Produces("application/json")]
        [Route("api/calculate/xy")]
        public IActionResult tokens(string formula, double from,double to,double n)
        {   
            try{
                formula=formula.Replace(' ','+' );
                classOnp onp = new classOnp(formula,from,to,n);
                var data = sendAnswer(onp);
                return Ok(data);
            }catch(error er){
                return Ok(sendError(er));
            }
        }
        
    }
}
