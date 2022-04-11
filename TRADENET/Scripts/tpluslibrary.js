$(document).ready(function () {
});

$('#cmbSelect').change(function () {
    event.preventDefault();
    $("#ClientMasterSearch").empty();
    var value = this.value;
    var search = "";
    if (value == "CL") {
        search = "Search For Client"
        $("#txtSearchbyName").css("visibility", "visible");
    }
    else if (value == "FM") {
        search = "Search For Family"
        $("#txtSearchbyName").css("visibility", "visible");
    }
    else if (value == "GR") {
        search = "Search For Group"
        $("#txtSearchbyName").css("visibility", "visible");
    }
    else if (value == "BR") {
        search = "Search For Branch"
        $("#txtSearchbyName").css("visibility", "visible");
    }
    else if (value == "SB") {
        search = "Search For SubBrokers"
        $("#txtSearchbyName").css("visibility", "visible");
    }
    $('#txtSearchbyName').val('');
    $('#txtSearchbyName').attr("placeholder", search);
});


$('#txtSearchbyName').on("input", function () {
    if ($('#txtSearchbyName').val() == "") {
        return;
    }
    $("#ClientMasterSearch").empty();
    var select = $('#cmbSelect').val();
    if (select == "") {
        select = "CL";
    }
    var options = {};
    options.url = '/Tplus/GetClientDetails';
    options.type = "GET";
    options.data = { 'criteria': $('#txtSearchbyName').val(), 'SearchBy': select };
    options.dataType = "json";
    options.success = function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#ClientMasterSearch').append("<option value='" +
            data[i].ClientName + "'></option>");
        }
    };
    $.ajax(options);
});









