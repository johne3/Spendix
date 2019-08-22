$(document).ready(function () {
    loadCategories();

    $('#transactionForm').validate({
        rules: {
            Date: {
                required: true
            },
            Payee: {
                required: true
            },
            Category: {
                required: true
            },
            Amount: {
                required: true,
                number: true
            }
        },
        messages: {
            Date: {
                required: 'Date is required.'
            },
            Payee: {
                required: 'Payee is required.'
            },
            Category: {
                required: 'Category is required.'
            },
            Amount: {
                required: 'Amount is required.',
                number: 'Amount must be a valid number.'
            }
        }
    });

    $('#TransactionType').on('change', function () {
        loadCategories();
    });

    $('#Category').on('change', function () {
        loadSubCategries();
    });
});

function loadCategories() {
    var transactionType = $('#TransactionType').val();

    $.get('/api/TransactionCategories/' + transactionType, function (data) {
        $('#Category').empty();
        $('#Category').append('<option value="">Select One</option>');

        $.each(data.categories, function (index, category) {
            $('#Category').append('<option value="' + category.bankAccountTransactionCategoryId + '">' + category.name + '</option>');
        });
    });
}

function loadSubCategries() {
    var categoryId = $('#Category').val();

    $.get('/api/TransactionSubCategories/' + categoryId, function (data) {
        $('#SubCategory').empty();
        $('#SubCategory').append('<option value="">Select One</option>');

        $.each(data.subCategories, function (index, subCategory) {
            $('#SubCategory').append('<option value="' + subCategory.bankAccountTransactionSubCategoryId + '">' + subCategory.name + '</option>');
        });
    });
}