$(document).ready(function () {
    $('.js-example-basic-single').select2();
    productListOnChange()
    productDetailsOnChange();
});
function fillCorrespondingRows(productListClass, data) {
    debugger;
    if (data.rate != 0) {
        $(".productQty").val(1);
        $(".productPrice").val(data.rate);
        $(".productDiscount").val(0);
        $(".productPrice").trigger("change")
    }
}
function productDetailsOnChange() {
    $(".productQty, .productPrice, .productDiscount").on('change', function () {
        countTotal(this);
    });
}
function productListOnChange() {
    $(".productList").on('change', function () {
        debugger;
        var productListClass = $(this);
        var val = $(this).val();
        $.ajax({
            type: "GET",
            url: "/bills/bill/getall",
            dataType: "json",
            data: { productId: val },
            success: function (response) {
                debugger;
                if (response.success) {
                    fillCorrespondingRows(productListClass, response.data);
                }
            }
        });
    })
}
function countTotal(that) {
    debugger;
    var QTY = $(".productQty").val();
    var Price = $(".productPrice").val();
    var Discount = $(".productDiscount").val();
    var TotalPrice = QTY * Price - Discount;
    $(".productTotal").val(TotalPrice);
}