$(document).ready(function () {
    $('form').validate({
        rules: {
            emailAddress: {
                required: true
            },
            password: {
                required: true
            }
        },
        messages: {
            emailAddress: {
                required: 'Email Address is required.'
            },
            password: {
                required: 'Password is required.',
            }
        }
    });
});