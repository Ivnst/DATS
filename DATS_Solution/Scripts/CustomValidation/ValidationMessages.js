
$.validator.messages.required = "Это поле обязательно для заполнения.";
$.validator.messages.remote = "Пожалуйста, исправьте это поле.";
$.validator.messages.email = "Пожалуйста, введите корректный email-адрес.";
$.validator.messages.url = "Пожалуйста, введите корректный URL.";
$.validator.messages.date = "Пожалуйста, введите корректное значение даты.";
$.validator.messages.dateISO = "Пожалуйста, введите корректное значение даты (ISO).";
$.validator.messages.number = "Пожалуйста, введите корректное число.";
$.validator.messages.digits = "Пожалуйста, введите только цифры.";
$.validator.messages.maxlength = $.validator.format("Пожалуйста, введите не более {0} символов.");
$.validator.messages.minlength = $.validator.format("Пожалуйста, введите более {0} символов.");
$.validator.messages.rangelength = $.validator.format("Пожалуйста, введите значение длиной от {0} до {1} символовg.");
$.validator.messages.range = $.validator.format("Пожалуйста, введите значение от {0} до {1}.");
$.validator.messages.max = $.validator.format("Пожалуйста, введите значение меньшее или равное {0}.");
$.validator.messages.min = $.validator.format("Пожалуйста, введите значение большее или равное {0}.");

$.ajaxSetup({
    async: false
});
