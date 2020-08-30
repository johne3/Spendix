var importedTransactionRowTemplate;

$(document).ready(function () {
    var importedTransactionRowTemplateHtml = $('#imported-transaction-row').html();
    importedTransactionRowTemplate = Handlebars.compile(importedTransactionRowTemplateHtml);

    initValidation();

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

    $(document).on('change', '.categorySelect', function () {
        var categoryId = $(this).val();

        var subCategorySelect = $(this).parent().parent().find('.subCategorySelect');
        subCategorySelect.empty();
        subCategorySelect.append('<option value="">Select One</option>');

        //Don't try to get subcategories for transfers
        if (!categoryId.startsWith('TransferTo_') && !categoryId.startsWith('TransferFrom_')) {
            $.get('/api/TransactionSubCategories/' + categoryId, function (data) {
                $.each(data.subCategories, function (index, subCategory) {
                    subCategorySelect.append('<option value="' + subCategory.bankAccountTransactionSubCategoryId + '">' + subCategory.name + '</option>');
                });
            });
        }
    });
});

function initValidation() {
    $('#transactionImportForm').validate({
        rules: {
        },
        messages: {
        }
    });
}

function addTransactionImportValidation(index) {
    $("#Payee_" + index).rules("add", {
        required: true,
        messages: {
            required: 'Payee is Required.'
        }
    });

    $("#CategoryId_" + index).rules("add", {
        required: true,
        messages: {
            required: 'Category is Required.'
        }
    });
}

function renderImportedTransactions(result) {
    $('#transactionsTable tbody').empty();
    $('#importTitle').text(result.bankAccountName + ' Transaction Import');

    $('#importedTransactionsRow').show();
    $('#processImportRow').hide();

    $('#transactionImportForm #BankAccountId').val(result.bankAccountId);

    $.each(result.transactions, function (index, value) {
        var html = importedTransactionRowTemplate({ index: index, transaction: value });
        $('#transactionsTable tbody').append(html);

        //Remove transfer options for selected bank account
        $('#CategoryId_' + index + ' option[value="TransferTo_' + result.bankAccountId + '"]').remove();
        $('#CategoryId_' + index + ' option[value="TransferFrom_' + result.bankAccountId + '"]').remove();

        addTransactionImportValidation(index);
    });
}

function showFileImportValidationMessage(message) {
    $('#ImportFile-error').text(message);
    $('#ImportFile-error').show();
}