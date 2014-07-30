
    // Определение функции для отображения текущего времени
    $().ready(function () { displayTime(); setInterval(function () { displayTime(); }, 30000); })
   function displayTime()
    {
        var element = document.getElementById("Clock"); // Найти элемент с id="Clock"
        var now = new Date(); // Получить текущее время
        var month = now.getMonth() + 1;
        if (month <= 9)
        {
            month.toLocaleString();
            month = "0" + month;
        }
        var day = now.getDate();
        var year = now.getFullYear();
        var hours = now.getHours();
        var minutes = now.getMinutes();
        if (minutes <= 9)
        {
            minutes.toLocaleString();
            minutes = "0" + minutes;
        }
       
        element.innerHTML =  day + '.' +month + '.' + year + "      " + hours + ":" + minutes; // Отобразить его
       
    }         



    