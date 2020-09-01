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
        var payeeTextBox = $(this).parent().parent().find('.payeeTextBox');

        subCategorySelect.empty();
        subCategorySelect.append('<option value="">Select One</option>');

        if (categoryId.startsWith('TransferTo_') || categoryId.startsWith('TransferFrom_')) {
            subCategorySelect.attr('readonly', 'readonly');
            subCategorySelect.attr('disabled', 'disabled');

            payeeTextBox.attr('readonly', 'readonly');
            payeeTextBox.attr('disabled', 'disabled');

            var text = $(this).find('option:selected').text();
            payeeTextBox.val(text);
        } else {
            subCategorySelect.removeAttr('readonly');
            subCategorySelect.removeAttr('disabled');

            payeeTextBox.removeAttr('readonly');
            payeeTextBox.removeAttr('disabled');
            payeeTextBox.val(payeeTextBox.data('originalValue'));

            $.get('/api/TransactionSubCategories/' + categoryId, function (data) {
                $.each(data.subCategories, function (index, subCategory) {
                    subCategorySelect.append('<option value="' + subCategory.bankAccountTransactionSubCategoryId + '">' + subCategory.name + '</option>');
                });
            });
        }
    });

    $(document).on('blur', '.payeeTextBox', function () {
        var val = $(this).val();
        var originalVal = $(this).data('originalValue');

        if (val !== originalVal) {
            $(this).data('originalValue', val);
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
    $('#Payee_' + index).rules('add', {
        required: true,
        messages: {
            required: 'Payee is Required.'
        }
    });

    $('#CategoryId_' + index).rules('add', {
        required: '#Save_' + index + ':checked',
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