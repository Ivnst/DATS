
var horde = null;

$(document).ready(function () {

    if (document.cookie.indexOf("gremlins") != -1) {
        startGremlins();
    }

    $(document).keypress(function (e) {
        //$('#Info').html(e.which);
        if (e.which == 91 && e.ctrlKey) { startGremlins(); }
        if (e.which == 93 && e.ctrlKey) { stopGremlins(); }
    });
});

function startGremlins() {
    if (horde == null) {
        horde = gremlins.createHorde()
        horde.unleash();
        $('#Info').html("Гремлины выпущены..(Остановка - Ctrl+])");
    }
    
    if (document.cookie.indexOf("gremlins") == -1) {
        document.cookie = "gremlins=1; path=/";
    }
}

function stopGremlins() {
    if (horde != null) {
        horde.stop();
        horde = null;
        $('#Info').html("Гремлины остановлены!");
        document.cookie = "gremlins=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/";
    }
}