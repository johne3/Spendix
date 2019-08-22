var categoryTemplate;
var subCategoryTemplate;

var rowToDelete;

$(document).ready(function () {
    var categoryTemplateHtml = $('#category-template').html();
    categoryTemplate = Handlebars.compile(categoryTemplateHtml);

    var subCategoryTemplateHtml = $('#sub-category-template').html();
    subCategoryTemplate = Handlebars.compile(subCategoryTemplateHtml);

    $('#addPaymentCategory').on('click', function () {
        var number = findNextCategoryNumber('Payment');

        var html = categoryTemplate({ number: number, transactionType: 'Payment' });
        $('#paymentCategoryColumn').append(html);
    });

    $(document).on('click', '.addSubCategory', function () {
        var inputNamePrefix = $(this).data('inputNamePrefix');
        var number = findNextSubCategoryNumber(inputNamePrefix);

        $(this).parents('.card-body').find('.subCategoryHeader').removeClass('d-none');

        var html = subCategoryTemplate({ inputNamePrefix: inputNamePrefix, subCategoryNumber: number });

        $(this).parent().parent().before(html);
    });

    $('#addDepositCategory').on('click', function () {
        var number = findNextCategoryNumber('Deposit');

        var html = categoryTemplate({ number: number, transactionType: 'Deposit' });
        $('#depositCategoryColumn').append(html);
    });

    $(document).on('click', '.confirmDeleteCategoryButton', function () {
        rowToDelete = $(this).parents('.card');
        $('#deleteCategoryModal #deleteCategoryButton').data('categoryId', $(this).data('categoryId'));
        $('#deleteCategoryModal').modal('show');
    });

    $(document).on('click', '.confirmDeleteSubCategoryButton', function () {
        rowToDelete = $(this).parents('.subCategoryRow');
        $('#deleteSubCategoryModal #deleteSubCategoryButton').data('subCategoryId', $(this).data('subCategoryId'));
        $('#deleteSubCategoryModal').modal('show');
    });

    $('#deleteCategoryButton').on('click', function () {
        var categoryId = $(this).data('categoryId');

        $.post('/api/Category/DeleteCategory/' + categoryId, function () {
            $('#deleteCategoryModal').modal('hide');

            if (rowToDelete) {
                rowToDelete.remove();
            }
        }, 'json');
    });

    $('#deleteSubCategoryButton').on('click', function () {
        var subCategoryId = $(this).data('subCategoryId');

        $.post('/api/Category/DeleteSubCategory/' + subCategoryId, function () {
            $('#deleteSubCategoryModal').modal('hide');

            if (rowToDelete) {
                rowToDelete.remove();
            }
        }, 'json');
    });
});

function findNextCategoryNumber(transactionType) {
    var nextNumber = 0;
    var foundNext = false;

    while (foundNext === false) {
        var element = $('#' + transactionType + 'Category_' + nextNumber + '_CategoryId');

        if (element.length === 0) {
            foundNext = true;
        } else {
            nextNumber++;
        }
    }

    return nextNumber;
}

function findNextSubCategoryNumber(inputNamePrefix) {
    //PaymentCategory_0_SubCategory_0_SubCategoryId

    var nextNumber = 0;
    var foundNext = false;

    while (foundNext === false) {
        var element = $('#' + inputNamePrefix + 'SubCategory_' + nextNumber + '_SubCategoryId');

        if (element.length === 0) {
            foundNext = true;
        } else {
            nextNumber++;
        }
    }

    return nextNumber;
}