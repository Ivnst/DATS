    
    $('#myModal form').validate({  // initialize plugin
        rules: {
            Name: { required: true, maxlength: 255, validName: true },
            Address: { maxlength: 255, validName: true }
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
