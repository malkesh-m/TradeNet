var button;
fnEnableDisableButton("LOAD");
fnEnableDisableControl("LOAD");

$('#txtSearchbyCode').prop('disabled', true);

$('#recDt').datepicker({
    format: 'dd/mm/yyyy',
    autoclose: true,
    startDate: "+0d",
    endDate: "+365d",
    todayHighlight: true,
});
$('#recDt').val(GetFormattedDate);
$('#recDt').datepicker('update');

$('#clearDt').datepicker({
    format: 'dd/mm/yyyy',
    autoclose: true,
    startDate: "+0d",
    endDate: "+365d",
    todayHighlight: true,
});
$('#clearDt').val('');
$('#clearDt').datepicker('update');

$('#txtSrno').change(function () {
    if ($('#txtSrno').val() == "") {
        fnClearFields('')
        return;
    }
    var Dpid = $('#cmbDPID').val();
    var Date = $('#recDt').val();
    var clearDt = $('#clearDt').val();
    var Srno = $('#txtSrno').val();
    var url = $(this).data('request-url');
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: JSON,
        data: { 'Srno': Srno, 'Date': Date, 'DPID': Dpid },
        success: function (result) {
            $("#txtVoucher").val(result.rc_voucherno.trim());
            $("#txtParticular").val(result.rc_particular);
            $("#txtChequeNo").val(result.rc_chequeno);
            $("#txtSearchbyNameRec").val(result.cm_name);
            $("#txtSearchbyCode").val(result.rc_clientcd);
            $('#cmbBank').val(result.rc_bankclientcd);
            $("#txtAmount").val(result.rc_amount);
            $("#cmbReceivedAs").val(result.rc_batchno);
            $("#recDt").val(result.rc_receiptdt);
            $("#clearDt").val(result.rc_cleareddt);
            $('#cmbMICR').html('');
            $(result.GetAllMicr).each(function () {
                $('#cmbMICR').append($("<option></option>").val(this.Value).html(this.Display));
            });
            $('#cmbMICR').val(result.rc_micr);

            $('#cmbAcno').html('');
            $(result.GetAllAc).each(function () {
                $('#cmbAcno').append($("<option></option>").val(this.Value).html(this.Display));
            });
            $('#cmbAcno').val(result.rc_BankActNo);
            fnEnableDisableButton("FETCH")
        },
        error: function (data) { }
    });
});

function GetFormattedDate() {
    var d = new Date();
    var day = ("0" + d.getDate()).slice(-2);
    var month = ("0" + (d.getMonth() + 1)).slice(-2);
    var year = d.getFullYear();
    return day + "/" + month + "/" + year;
}

function GetClearDate() {
    return '' + "/" + '' + "/" + '';
}

function fnEnableDisableButton(value) {
    $('#btnRecAdd').prop('disabled', value == "ADD" || value == "EDIT" || value == "FETCH");
    $('#btnRecEdit').prop('disabled', value == "LOAD" || value == "ADD" || value == "EDIT");
    $('#btnRecDelete').prop('disabled', value == "LOAD" || value == "ADD");
    $('#btnRecSave').prop('disabled', value == "LOAD" || value == "FETCH");
}

function fnClearFields(value) {
    $("#txtSrno").val("");
    $("#txtVoucher").val("");

    $('#recDt').val(GetFormattedDate);
    $('#recDt').datepicker('update');

    $('#clearDt').val('');
    $('#clearDt').datepicker('update');

    if (!"@blnFromDashboard") {
        $("#txtSearchbyNameRec").val("");
        $("#txtSearchbyCode").val("");
        $('#cmbMICR').html('"<option>Select</option>"');
    }
    $("#txtSearchbyNameRec").val("");
    $("#txtSearchbyCode").val("");
    $('#cmbMICR').html('"<option>Select</option>"');
    $("#txtParticular").val("");
    $("#txtChequeNo").val("");
    $("#txtAmount").val("");
    $("#cmbReceivedAs").val("0");
    $('#cmbDPID').val("");
    $('#cmbAcno').html('"<option>Select</option>"');
    $('#cmbBank').html('"<option>Select</option>"');
    $('#FileUpload1').val("");
}

function fnEnableDisableControl(value) {
    $('#txtSrno').prop('disabled', value == "ADD" || value == "EDIT");
    $('#cmbDpid').prop('disabled', value == "EDIT");
    $('#cmbBank').prop('disabled', value == "LOAD");
    $('#txtVoucher').prop('disabled', value == "LOAD");
    $('#recDt').prop('disabled', value == "LOAD");
    $('#clearDt').prop('disabled', value == "LOAD");
    $('#txtSearchbyNameRec').prop('disabled', value == "LOAD" && "@blnFromDashboard");
    $('#txtParticular').prop('disabled', value == "LOAD");
    $('#cmbReceivedAs').prop('disabled', value == "LOAD");
    $('#txtChequeNo').prop('disabled', value == "LOAD");
    $('#cmbMICR').prop('disabled', value == "LOAD");
    $('#cmbAcno').prop('disabled', value == "LOAD");
    $('#txtAmount').prop('disabled', value == "LOAD");
    $('#FileUpload1').prop('disabled', value == "LOAD");
}

$('#txtSearchbyNameRec').change(function () {
    if ($('#txtSearchbyNameRec').val() == "") {
        $('#txtSearchbyCode').val('');
        $('#cmbMICR').html('"<option>Select</option>"');
        return;
    }
    var valNew = $('#txtSearchbyNameRec').val().split("~");
    if (valNew.length != 2) {
        $('#cmbMICR').html('"<option>Select</option>"');
        return;
    }
    var url = $(this).data('request-url');
    var client = valNew[1];
    $('#txtSearchbyNameRec').val(valNew[0].trim());
    $('#txtSearchbyCode').val(valNew[1].trim());
    $('#cmbMICR').html('');
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: JSON,
        data: { 'Code': client },
        success: function (result) {
            $(result).each(function () {
                $('#cmbMICR').append($("<option></option>").val(this.Value).html(this.Display));
            });
        },
        error: function (data) { }
    });
});

$(document).on('change', '#cmbDPID', function (event) {
    var dpid = this.value;
    if (dpid.trim() == '') {
        $('#cmbBank').html('"<option>Select</option>"');
        return
    }
    var url = $(this).data('request-url');
    $('#cmbBank').html('');
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: JSON,
        data: { 'DPID': dpid },
        success: function (result) {
            $(result).each(function () {
                $('#cmbBank').append($("<option></option>").val(this.Value).html(this.Display));
            });
        },
        error: function (data) { }
    });
});

$(document).on('change', '#cmbMICR', function (event) {
    var Micr = this.value;
    if (Micr.trim() == '' || Micr.trim() == 'Select') {
        $('#cmbAcno').html('"<option>Select</option>"');
        return
    }
    var url = $(this).data('request-url');
    var Code = $('#txtSearchbyCode').val();
    $('#cmbAcno').html('');
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: JSON,
        data: { 'Code': Code, 'Micr': Micr },
        success: function (result) {
            $(result).each(function () {
                $('#cmbAcno').append($("<option></option>").val(this.Value).html(this.Display));
            });
        },
        error: function (data) { }
    });
});

$(document).on('click', '#btnRecAdd', function (event) {
    button = "ADD";
    fnEnableDisableButton("ADD")
    fnEnableDisableControl("ADD")
    //$('#cmbDPID').val('NC');
    //$('#cmbDPID').trigger("change");
});

$(document).on('click', '#btnRecEdit', function (event) {
    button = "EDIT";
    fnEnableDisableControl("EDIT")
    fnEnableDisableButton("EDIT")
});

$(document).on('click', '#btnRecDelete', function (event) {
    button = "DELETE";
});

$(document).on('click', '#btnRecSave', function (event) {
    if ($("#txtAmount").val() == "") {
        $("#txtAmount").prop('required', true);
        return;
    }

    if (confirm('Are you sure to Save?')) {
        // Save it!
    } else {
        return "";
    }

    var url = $(this).data('request-url');

    var data = new FormData();
    var files = $("#FileUpload1").get(0).files;
    if (files.length > 0) {
        data.append("HelpSectionImages", files[0]);
    }
    data.append("SRNO", $('#txtSrno').val());
    data.append("MODE", button);
    data.append("DPID", $('#cmbDPID').val());
    data.append("BANK", $('#cmbBank').val());
    data.append("VOUCHER", $('#txtVoucher').val());
    data.append("DATE", $('#recDt').val());
    data.append("CLEARDT", $('#clearDt').val());
    data.append("CODE", $('#txtSearchbyCode').val());
    data.append("PARTICULAR", $('#txtParticular').val());
    data.append("RECEIVED", $('#cmbReceivedAs').val());
    data.append("CHEQUENO", $('#txtChequeNo').val());
    data.append("MICR", $('#cmbMICR').val());
    data.append("ACNO", $('#cmbAcno').val());
    data.append("AMOUNT", $('#txtAmount').val());
    data.append("DEBITFLAG", 'C');
    $.ajax({
        url: url,
        type: "POST",
        processData: false,
        contentType: false,
        data: data,
        success: function (response) {
            alert(response);
            //$('#modalSuccessError').modal('show');
        },
        error: function (er) {
            alert(er);
        }
    });

    fnEnableDisableButton("LOAD")
    fnEnableDisableControl("LOAD")
    fnClearFields('')

});

$(document).on('click', '#btnRecCancel', function (event) {
    fnEnableDisableButton("LOAD")
    fnEnableDisableControl("LOAD")
    fnClearFields('')

});

