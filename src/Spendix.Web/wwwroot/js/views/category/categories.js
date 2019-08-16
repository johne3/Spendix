var categoryTemplate;
var subCategoryTemplate;


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
});

function getRowHtml() {
    return '<div class="col-md-11">' +
        '<div class="form-group">' +
        '<input type="text" id="" name="" class="form-control" placeholder="Name" value="">' +
        '</div>' +
        '</div >' +
        '<div class="col-md-1">' +
        '<a href="javascript:void(0)" class="float-right mt-2"><i class="fas fa-fw fa-trash"></i></a>' +
        '</div>';
}