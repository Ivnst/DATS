$.validator.addMethod(
    "vdate",
    function (value, element) {
        return validium();
    },
    "Формат должен быть dd.MM.yyyy HH:mm в диапазоне от 01.01.2000 до 31.12.2099."
);

$('#myModal form').validate({  // initialize plugin
    rules: {
        Name: { required: true, maxlength: 255, validName: true },
        BeginsAt: { vdate: true },
        Duration: { required: true, digits: true, range: [1, 2147483647] }
    },

    highlight: function (element) {
        $(element)
            .closest('.editor-field').removeClass('has-success').addClass('has-error');
    },
    success: function (element) {
        element.addClass('valid')
        .closest('.editor-field').removeClass('has-error').addClass('has-success');;
    },
    onfocusout: false

});

function validium() {
    var nonError = true;
    var errMessage1 = "", errMessage2 = "";
    $("#begining").val(function (i, v) {
        var fullStr = v;
        var dateTmp = fullStr.substring(0, fullStr.indexOf(" "));
        var timeTmp = fullStr.substring(fullStr.indexOf(" ") + 1);

        var dayStr = dateTmp.substring(0, dateTmp.indexOf("."));
        dateTmp = dateTmp.substring(dateTmp.indexOf(".") + 1);
        var monthStr = dateTmp.substring(0, dateTmp.indexOf("."));
        dateTmp = dateTmp.substring(dateTmp.indexOf(".") + 1);
        var yearStr = dateTmp;

        var hourStr = timeTmp.substring(0, timeTmp.indexOf(":"));
        timeTmp = timeTmp.substring(timeTmp.indexOf(":") + 1);
        var minuteStr = timeTmp;

        var msNoZone = Date.parse(yearStr + '-' + monthStr + '-' + dayStr + 'T' + hourStr + ':' + minuteStr + ':00.000');

        if (!isFinite(msNoZone)) {
            nonError = false;
        } else {
            if ((msNoZone >= 946677600000) && (msNoZone <= 4102358400000)) {
                // Ok
            } else {
                nonError = false;
            }
        }

        return v;
    });
    return nonError;
}