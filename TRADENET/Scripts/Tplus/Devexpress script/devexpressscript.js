function exportToPDF() {
    var width = (parseInt($("#columnWidth").val()) + 100) / 3.779;
    var doc = new jsPDF('l', 'mm', [width, width / 1.6]);
    var dataGrid = $("#tblRMSSummary").dxDataGrid("instance");
    var i = 1;

    DevExpress.pdfExporter.exportDataGrid({
        jsPDFDocument: doc,
        component: dataGrid,
        autoTableOptions: {
            margin: { top: 35 },
            styles: {
                fontSize: 8
            },
            didDrawCell: (data) => {
                if (data.section === 'head' && data.column.index === 0 && i === 1) {
                    var image = document.getElementById("imageid");
                    if (image.src !== "" && image.src !== null && image.src !== undefined) {
                        var base64 = getBase64Image(image);
                        doc.addImage(base64, 'bmp', 15, 8, 25, 25);
                    }

                    var headerXOffset = (doc.internal.pageSize.width / 2) - (doc.getStringUnitWidth($("#header").val()) * doc.internal.getFontSize() / 2);
                    doc.setFontSize(25);
                    doc.text($("#header").val(), headerXOffset, 13, 'center').setFont(undefined, 'bold');

                    doc.setFontSize(18);
                    doc.text($("#subheader1").val(), headerXOffset, 20, 'center')

                    doc.setFontSize(18);
                    doc.text($("#subheader2").val(), headerXOffset, 28, 'center')

                    i++;
                }
            },
            didDrawPage: function (data) {
                // Reseting top margin. The change will be reflected only after print the first page.
                data.settings.margin.top = 15;
            }
        },
        keepColumnWidths: true
    }).then(function () {
        doc.save("RMSSummary.pdf");
    });
}

function exporting() {
    var dataGrid1 = $("#tblRMSSummary").dxDataGrid("instance");
    var workbook = new ExcelJS.Workbook();
    var RMSSummary = workbook.addWorksheet('RMSSummary');

    DevExpress.excelExporter.exportDataGrid({
        component: dataGrid1,
        worksheet: RMSSummary,
        topLeftCell: { row: 6, column: 1 },
        autoFilterEnabled: true,
        customizeCell: function (options) {
            var gridCell = options.gridCell;
            var excelCell = options.excelCell;

            if (gridCell.rowType === "data") {
                if (typeof (gridCell.value) === 'number') {
                    excelCell.value = parseInt(gridCell.value);

                    //if (excelCell.value === 0) {
                    //    excelCell.value = '';
                    //}
                }
            }
            if (gridCell.rowType === "totalFooter" || gridCell.rowType === "groupFooter" ) {
                if (typeof (gridCell.value) === 'number') {
                    excelCell.value = parseInt(gridCell.value);
                }
            }
        }
    }).then(function (dataGridRange) {
        var headerRow = RMSSummary.getRow(2);
        var endColIndex = dataGridRange.to.column;

        RMSSummary.mergeCells(2, 1, 4, 1);
        RMSSummary.mergeCells(2, 2, 2, endColIndex);
        RMSSummary.mergeCells(3, 2, 3, endColIndex);
        RMSSummary.mergeCells(4, 2, 4, endColIndex);

        headerRow.height = 23;
        var img = document.getElementById("imageid");

        if (img.src !== "" && img.src !== null && img.src !== undefined) {
            var base64 = getBase64Image(img);
            const image = workbook.addImage({
                base64: base64,
                extension: 'bmp',
            });
            RMSSummary.addImage(image, 'A2:A4');
        }

        headerRow.getCell(2).value = $("#header").val();
        headerRow.getCell(2).alignment = { horizontal: 'center' };
        headerRow.getCell(2).font = { size: 20, bold: true };

        RMSSummary.getRow(3).getCell(2).value = $("#subheader1").val();
        RMSSummary.getRow(3).getCell(2).alignment = { horizontal: 'center' };
        RMSSummary.getRow(3).getCell(2).font = { size: 15, bold: true };

        RMSSummary.getRow(4).getCell(2).value = $("#subheader2").val();
        RMSSummary.getRow(4).getCell(2).alignment = { horizontal: 'center' };
        RMSSummary.getRow(4).getCell(2).font = { size: 15, bold: true };

    }).then(function () {
        workbook.xlsx.writeBuffer().then(function (buffer) {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'RMSSummary.xlsx');
        });
    });
    //e.cancel = true;
}

function getBase64Image(img) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);
    var dataURL = canvas.toDataURL("image/png");
    return dataURL.replace(/^data:image\/(bmp|jpg);base64,/, "");
}

function onCellPrepared(e) {
    if (e.rowType == "header") {
        e.cellElement.css("text-align", "center");
    }
}

function onInitialized(e) {
    debugger;
    let dataGrid = $("#tblRMSSummary").dxDataGrid("instance");
    var a = e.rowIndex;
    var b = dataGrid.totalCount() - 1;

}

function customizeTextWithoutSep(cellInfo) {

    if (cellInfo.value === 0) {
        return '';
    }
    else if (typeof (cellInfo.value) === 'string') {
        return cellInfo.value;
    }
    else {
        let re = /,/gi;
        var x = cellInfo.valueText.replace(re, "");
        return x.toString();
    }
}

function customizeTextWithoutSepWithZero(cellInfo) {

    if (cellInfo.value === 0) {
        return cellInfo.valueText;
    }
    else if (typeof (cellInfo.value) === 'string') {
        return cellInfo.value;
    }
    else {
        let re = /,/gi;
        var x = cellInfo.valueText.replace(re, "");
        return x.toString();
    }
}

function customizeText(cellInfo) {

    if (cellInfo.value === 0) {
        return '';
    }
    else if (typeof (cellInfo.value) === 'string') {
        return cellInfo.value;
    }
    else {
        return NumberFormat(cellInfo.valueText);
    }
}

function customizeTextWithZero(cellInfo) {

    if (cellInfo.value === 0) {
        return cellInfo.valueText;
    }
    else if (typeof (cellInfo.value) === 'string') {
        return cellInfo.value;
    }
    else {
        return NumberFormat(cellInfo.valueText);
    }
}

function NumberFormat(x) {
    let re = /,/gi;
    x = x.toString();
    x = x.replace(re, "");
    var sign = '';
    if (x.substring(0, 1) === '-') {
        x = x.substring(1, x.length);
        sign = '-'
    }
    var afterPoint = '';
    if (x.indexOf('.') > 0)
        afterPoint = x.substring(x.indexOf('.'), x.length);
    x = Math.floor(x);
    x = x.toString();
    var lastThree = x.substring(x.length - 3);
    var otherNumbers = x.substring(0, x.length - 3);
    if (otherNumbers != '')
        lastThree = ',' + lastThree;
    var res = sign + otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + afterPoint;

    return res;
}

function onVisibleOptionsValueChanged(data) {

    onVisibleBefore().then(() => { onVisibleOption(data).then(() => { $('#divModalProgress1').modal('hide'); }) });
}

function onVisibleBefore() {

    $('#divModalProgress1').modal('show');

    return new Promise((resolve) => {
        setTimeout(() => {
            resolve({ action: 'boop' });
        }, 1000);
    });
}

function onVisibleOption(data) {
    let dataGrid = $("#tblRMSSummary").dxDataGrid("instance");
    if (data.value == true) {
        for (var i = 0; i < dataGrid.columnCount(); i++) {
            dataGrid.columnOption(i, "visible", data.value);
        }
    }
    else {
        dataGrid.state({});
    }

    return new Promise((resolve) => {
        setTimeout(() => {
            resolve({ action: 'boop' });
        }, 1000);
    });
}

$(document).ready(function () {
    $('.dx-datagrid').css({ 'font-size': 12 + 'px' });
});
