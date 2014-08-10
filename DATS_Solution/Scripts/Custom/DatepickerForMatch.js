﻿$(document).ready(function () {
    $.datepicker.setDefaults($.datepicker.regional['ru']);
    $("#begining").datepicker({
        showOn: "button",
        buttonImage: "/Images/calendar.gif",
        showButtonPanel: true,
        firstDay: 1,
        dateFormat: "dd.mm.yy 00:00"
    });
});