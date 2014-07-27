$(document).ready(function () {
    $.datepicker.setDefaults($.datepicker.regional['ru']);
    $("#begining").datepicker();
    $('#begining').datepicker('option', 'dateFormat', 'dd.mm.yy 00:00');
});