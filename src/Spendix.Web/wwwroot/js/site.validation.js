$(document).ready(function () {
    $.validator.addMethod('strongpassword', function (value) {
        return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
            && /[a-z]/.test(value) // has a lowercase letter
            && /\d/.test(value); // has a digit
    }, 'Password must contain at least 1 digit and lowercase letter.');
});