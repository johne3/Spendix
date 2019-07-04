$(document).ready(function () {
    $('#addPaymentCategory').on('click', function () {
        var html = getRowHtml();
        $('#paymentCategoriesRow').append(html);
    });

    $('#addDepositCategory').on('click', function () {
        var html = getRowHtml();
        $('#depositCategoriesRow').append(html);
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