// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var link = 'http://192.168.99.100:8000/api/';
function TokensFunkcja(){
    var wynik = document.getElementById("wynik");
    var formula = document.getElementById("formulawartosc").value;
    if (formula.trim() == '') {
        document.getElementById("wynik").innerHTML = "<p style='color:red'>Proszę wpisać liczbę</p>";
        return "Proszę wpisać liczbę"
    } else{
        document.getElementById("wynik").innerHTML = "";
        var request = new XMLHttpRequest();
        request.open('GET', link + 'tokens?formula='+formula, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
            }
    request.onload = function () {
        var data = JSON.parse(this.response);
        var infix;
        var rpn;
        if (request.status >= 200 && request.status < 400) {
            if (data.status == "ok") {
                wynik.innerHTML = "";
                infix = data.result[0].inflix;
                rpn = data.result[0].rpn;
                wynik.innerHTML += "Infix: ";
                infix.forEach(i => {
                    wynik.innerHTML += i + " ";
                });
                wynik.innerHTML += "<br>RPN: ";
                rpn.forEach(i => {
                    wynik.innerHTML += i + " ";
                });
            }
            else {
                Error(data);
            }
        }
        else {
            document.getElementById("wynik").innerHTML = "Błąd z APP";
        }
    };
    request.send();
    }
    
}
function CalculateXY(){
    var formulawartosc = document.getElementById("formulawartosc").value;
    var formulawartoscfrom = document.getElementById("formulawartoscfrom").value;
    var formulawartoscto = document.getElementById("formulawartoscto").value;
    var formulawartoscrange = document.getElementById("formulawartoscrange").value;
    if (formulawartoscfrom.trim() == '' || formulawartoscto.trim() == '' || formulawartoscrange.trim() == '') {
        document.getElementById("wynik").innerHTML = "<p style='color:red'>Proszę wpisać liczbę</p>";
        document.getElementById("Wykres").innerHTML = "";
    } else if(isNaN(formulawartoscfrom) || isNaN(formulawartoscto) || isNaN(formulawartoscrange)){
        document.getElementById("wynik").innerHTML = "<p style='color:red'>Problem z poprawnym wypełnieniem pól</p>";
        document.getElementById("Wykres").innerHTML = "";
    }else{
        document.getElementById("wynik").innerHTML = "";
        document.getElementById("Wykres").innerHTML = "";
        var request = new XMLHttpRequest();
        request.open('GET', link + 'calculate/xy?formula='+formulawartosc +'&from='+formulawartoscfrom+'&to='+formulawartoscto+'&n='+formulawartoscrange, true);
                    request.onerror = function () {
                        alert("Połączenie z API zostało zerwane");
        }
            request.onload = function () {
                var data = JSON.parse(this.response);
                if (request.status >= 200 && request.status < 400) {
                    if (data.status == "ok") {
                        document.getElementById("wynik").innerHTML = "Status: "+data.status;
                        RenderChart(data.result);
                        
                    }
                    else {
                        Error(data);
                    }
                }
                else {
                    document.getElementById("wynik").innerHTML = "Błąd z APP";
                }
            };
            request.send();
    }
    
}
function Calculate(){
    var formulawartosc = document.getElementById("formulawartosc").value;
    var formulawartoscfrom = document.getElementById("x").value;
    if (formulawartoscfrom.trim() == '' ) {
        document.getElementById("wynik").innerHTML = "<p style='color:red'>Proszę wpisać liczbę</p>";
    } else if(isNaN(formulawartoscfrom)){
        document.getElementById("wynik").innerHTML = "<p style='color:red'>Problem z poprawnym wypełnieniem pól</p>";
    }else{
        document.getElementById("wynik").innerHTML = "";
        var request = new XMLHttpRequest();
        request.open('GET', link + 'calculate?formula='+formulawartosc +'&x='+formulawartoscfrom, true);
            request.onerror = function () {
                alert("Połączenie z API zostało zerwane");
        }
    request.onload = function () {
        var data = JSON.parse(this.response);
        if (request.status >= 200 && request.status < 400) {
            if (data.status == "ok") {
                document.getElementById("wynik").innerHTML = "Status: "+data.status + "<br> Wynik: "+ data.result;
            }
            else {
                Error(data);
            }
        }
        else {
            document.getElementById("wynik").innerHTML = "Błąd z APP";
        }
    };
    request.send();
    }
}
function Error(data){
    document.getElementById("wynik").innerHTML = "Status: "+data.status + "<br> Błąd: "+ data.result;
}
function RenderChart(results) {

    var chart = new CanvasJS.Chart("Wykres",
        {
            animationEnabled: true,
            zoomEnabled: true,
            title: {
                text: "Wyniki obliczeń"
            },
            axisX: {
                valueFormatString: "#0.########",
                title: "Podane wartości",
            },
            axisY: {
                title: "Wyniki",
                includeZero: true,
            },
            legend: {
                cursor: "pointer",
                fontSize: 16,
                itemclick: toggleDataSeries
            },
            toolTip: {
                shared: true
            },
            data: [{
                name: "wynik",
                type: "spline",
                yValueFormatString: "#0.#############",
                showInLegend: true,
                dataPoints: results
            }]
        });
    chart.render();

    function toggleDataSeries(e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
            e.dataSeries.visible = false;
        }
        else {
            e.dataSeries.visible = true;
        }
        chart.render();
    }

}