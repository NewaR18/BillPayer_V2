function countTotal() {
    var QTY = $("#Qty").val();
    var Price = $("#Price").val();
    var Discount = $("#Discount").val();
    var TotalPrice = QTY * Price - Discount;
    $("#Total").val(TotalPrice.toFixed(2));
}
function productDetailsOnChange() {
    $("#Qty, #Price, #Discount").on('change', function () {
        countTotal();
    });
}
$(document).ready(function () {
    productDetailsOnChange();
});