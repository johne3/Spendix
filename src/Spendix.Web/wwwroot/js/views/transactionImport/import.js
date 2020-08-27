var importedTransactionRowTemplate;

$(document).ready(function () {
    var importedTransactionRowTemplateHtml = $('#imported-transaction-row').html();
    importedTransactionRowTemplate = Handlebars.compile(importedTransactionRowTemplateHtml);

    $('#ImportFile').on('change', function () {
        var file = $(this)[0].files[0];
        $('#ImportFileLabel').text(file.name);
    });

    $('#processImportButton').on('click', function () {
        $('#ImportFile-error').hide();

        var fileUpload = $("#ImportFile").get(0);
        var files = fileUpload.files;
        var allowedExtensions = /(\.csv)$/i;
        //var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;

        if (files.length === 0) {
            showFileImportValidationMessage('Please select a file.');
        } else if (!allowedExtensions.exec(files[0].name)) {
            showFileImportValidationMessage('Only CSV files are supported.');
        } else {
            var formData = new FormData();

            formData.append('BankAccountId', $('#BankAccountId').val());
            formData.append('BankImportSource', $('#BankImportSource').val());

            formData.append(files[0].name, files[0]);

            $.ajax({
                url: '/Transactions/ProcessImport',
                type: "POST",
                contentType: false,
                processData: false,
                data: formData,
                success: renderImportedTransactions,
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }
    });
});

function renderImportedTransactions(result) {
    $('#transactionsTable tbody').empty();
    $('#importedTransactionsRow').show();
    $('#processImportRow').hide();

    $.each(result.transactions, function (index, value) {
        var html = importedTransactionRowTemplate({ index: index, transaction: value });
        $('#transactionsTable tbody').append(html);
    });
}

function showFileImportValidationMessage(message) {
    $('#ImportFile-error').text(message);
    $('#ImportFile-error').show();
}