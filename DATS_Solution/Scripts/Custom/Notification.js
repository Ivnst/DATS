

$().ready(function () {

    var notificationKey = null;

    //поиск свойства notify в адресной строке
    if (location.search) {
        var parts = location.search.substring(1).split('&');

        for (var i = 0; i < parts.length; i++) {
            var par = parts[i].split('=');
            if (par[0] == 'notify') {
                notificationKey = par[1];
                break;
            }
        }
    } else { return; }


    //если указан ключ записи в кэше, то выводим сообщение, которое оно содержит
    if (notificationKey != null) {
        $.post("/Utils/GetCachedData", { key: notificationKey },
        function (data) {
            //здесь data - это значение, полученное из кэша
            if (data == null || data == undefined) return;

            //избавление от мусора, который добавляет somee.com
            data = data.split("<!")[0];

            //парсинг данных
            var response = jQuery.parseJSON(data);

            //отображение сообщения
            ShowMessageBox(response.message, response.header, response.error);
        })
    }
})

//отображение всплывающего модального сообщения
function ShowMessageBox(message, header, error) {

    $('#myModalNotification').modal({
        remote: '/Utils/MessageBox?header=' + encodeURIComponent(header) + '&message=' + encodeURIComponent(message) + '&error=' + encodeURIComponent(error)
    })

}