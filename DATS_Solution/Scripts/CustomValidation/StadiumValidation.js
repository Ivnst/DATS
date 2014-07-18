    
    $('#myModal form').validate({  // initialize plugin
        rules: {
            Name: { required: true },
        },

        highlight: function (element) {
            $(element)               
                .closest('.editor-field').removeClass('has-success').addClass('has-error');
        },
        success: function (element) {
            element.addClass('valid')            
            .closest('.editor-field').removeClass('has-error').addClass('has-success');;
        }

    });
