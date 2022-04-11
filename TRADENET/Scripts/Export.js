



function exportToExcel(Filename, tablename, Cname, Title) {
    var htmls = "";
    var uri = 'data:application/vnd.ms-excel;base64,';
    var template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><tr>{table}</tr></body></html>';
    var base64 = function (s) {
        return window.btoa(unescape(encodeURIComponent(s)))
    };

    var format = function (s, c) {
        return s.replace(/{(\w+)}/g, function (m, p) {
            return c[p];
        })
    };

    var tab_text = "<table border='1px'> <tr style='font-weight:bold;'><b>" + Cname + "<br/>" + Title + "<b/>";

    var textRange; var j = 0;
    tab = document.getElementById(tablename); // id of table


    for (j = 0 ; j < tab.rows.length ; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
        //tab_text=tab_text+"</tr>";
    }

    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
    tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params
    $("#multiple37 thead").remove();


    htmls = tab_text;

    var ctx = {
        worksheet: 'Worksheet',
        table: htmls
    }


    var link = document.createElement("a");
    link.download = Filename + ".xls";
    link.href = uri + base64(format(template, ctx));
    link.click();
}

function deleteLastColumn() {
    $('#multiple37 tr').find('th:last-child, td:last-child').remove()
}


function Export(Filename, tablename) {
    html2canvas(document.getElementById(tablename), {
        onrendered: function (canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                content: [{
                    image: data,
                    width: 500
                }]
            };
            pdfMake.createPdf(docDefinition).download(Filename + ".pdf");
        }
    });
}




$(document).ready(function () {
    $('body').append($('<script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.22/pdfmake.min.js"></script>'));
    $('body').append($('<script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js"></script>'));
});


function downloadCSV(csv, filename) {
    var csvFile;
    var downloadLink;
    // CSV file
    csvFile = new Blob([csv], { type: "text/csv" });

    // Download link
    downloadLink = document.createElement("a");


    // File name
    downloadLink.download = filename;

    // Create a link to the file
    downloadLink.href = window.URL.createObjectURL(csvFile);

    // Hide download link
    downloadLink.style.display = "none";

    // Add the link to DOM
    document.body.appendChild(downloadLink);

    // Click download link
    downloadLink.click();
}

function exportTableToCSV(filename) {
    var csv = [];
    var rows = document.querySelectorAll("table tr");

    for (var i = 0; i < rows.length; i++) {
        var row = [], cols = rows[i].querySelectorAll("td, th");

        for (var j = 0; j < cols.length; j++)
            row.push(cols[j].innerText.replace('&nbsp;', ' '));

        csv.push(row.join(","));
    }

    // Download CSV file
    downloadCSV(csv.join("\n"), filename);
}
