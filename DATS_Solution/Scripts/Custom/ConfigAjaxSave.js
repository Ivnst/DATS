$(document).ready(function () {
    $('#iksar').click(function (event) {
        event.preventDefault();
        var subFind = $("#FormForSave");
        if (validium()) {
            var data = subFind.serialize();
            var url = subFind.attr('action');
            $.post(url, data, function () {
                window.location = "/ConfigSetting/Index?sid=@ViewBag.Stadium.Id";
            });
        }
    });
    // start validation
    $(".iksar").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
    // end validation
});
function validium() {
    var nonError = true;
    var errMessage1 = "", errMessage2 = "";
    $(".iksar").val(function (i, v) {
        if ((v.length > 0) && (isFinite(v) == false)) { nonError = false; errMessage1 = "Данные не сохранены, так как одно или несколько значений содержат ошибку.\n" }
        if ((v.length > 0) && (isFinite(v) == true) && ((v < 0) || (v > 2147483647))) { nonError = false; errMessage2 = "Данные не сохранены, так как количество минут ограничено диапазоном от 0 до 2147483647." }
        return v;
    });
    if (nonError == false) alert(errMessage1 + errMessage2);
    return nonError;
}