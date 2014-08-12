
var horde = null;

$(document).ready(function () {

    if (document.cookie.indexOf("gremlins") != -1) {
        horde = gremlins.createHorde();
        horde.unleash();
        $('#Info').html("Гремлины выпущены (Остановить - Ctrl+])..");
    }

    $(document).keypress(function (e) {
        //$('#Info').html(e.which);
        if (e.which == 91 && e.ctrlKey) {
            horde = gremlins.createHorde();
            horde.unleash();
            $('#Info').html("Гремлины выпущены..");
            if (document.cookie.indexOf("gremlins") == -1) {
                document.cookie = "gremlins=1; path=/";
            }
        }

        if (e.which == 93 && e.ctrlKey) {
            if (horde != null) {
                horde.stop();
                horde = null;
                $('#Info').html("Гремлины остановлены!");
                document.cookie = "gremlins=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/";
            }
        }

    });
});


// возвращает cookie с именем name, если есть, если нет, то undefined
function getCookie(name) {
    var matches = document.cookie.match(new RegExp(
    "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
  ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}