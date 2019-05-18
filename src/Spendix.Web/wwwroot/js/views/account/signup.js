$(document).ready(function () {
    $('form').validate({
        rules: {
            emailAddress: {
                required: true,
                email: true,
                remote: {
                    url: '/api/Account/UniqueEmail',
                    type: 'post',
                    data: {
                        email: function () {
                            return $('#emailAddress').val();
                        }
                    }
                }
            },
            password: {
                required: true,
                minlength: 8,
                strongpassword: true
            },
            confirmPassword: {
                equalTo: '#password'
            }
        },
        messages: {
            emailAddress: {
                required: 'Email Address is required.',
                email: 'Enter a valid email address.',
                remote: 'Email Address already exists.'
            },
            password: {
                required: 'Password is required.',
                minlength: 'Password must be at least 8 characters.',
                strongpassword: 'Password must contain at least 1 digit and lowercase letter.'
            },
            confirmPassword: {
                equalTo: 'Passwords must match.'
            }
        }
    });
});