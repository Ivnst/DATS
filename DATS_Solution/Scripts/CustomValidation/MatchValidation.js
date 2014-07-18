$('#myModal form').validate({  // initialize plugin
    rules: {
        Name: { required: true, maxlength: 255 },
        BeginsAt: { required: true },
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


});