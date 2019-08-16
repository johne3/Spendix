var categoryTemplate;
var subCategoryTemplate;

var rowToDelete;

$(document).ready(function () {
    var categoryTemplateHtml = $('#category-template').html();
    categoryTemplate = Handlebars.compile(categoryTemplateHtml);

    var subCategoryTemplateHtml = $('#sub-category-template').html();
    subCategoryTemplate = Handlebars.compile(subCategoryTemplateHtml);

    $('#addPaymentCategory').on('click', function () {
        var html = categoryTemplate({});
        $('#paymentCategoryColumn').append(html);
    });

    $(document).on('click', '.addSubCategory', function () {
        $(this).parents('.card-body').find('.subCategoryHeader').removeClass('d-none');
        var html = subCategoryTemplate({});
        $(this).parent().parent().before(html);
    });

    $('#addDepositCategory').on('click', function () {
        var html = categoryTemplate({});
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