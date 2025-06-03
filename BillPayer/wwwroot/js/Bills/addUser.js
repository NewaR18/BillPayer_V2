$(document).ready(function () {
    $('.js-example-basic-single').select2();
    productListOnChange();
    productDetailsOnChange();
});
function productDetailsOnChange() {
    $(".productQty, .productPrice, .productDiscount").on('change', function () {
        countTotal(this);
    });
}
function productListOnChange() {
    $(".productList").on('change', function () {
        var productListClass = $(this);
        var val = $(this).val();
        $.ajax({
            type: "GET",
            url: "/bills/bill/getall",
            dataType: "json",
            data: { productId: val },
            success: function (response) {
                if (response.success) {
                    fillCorrespondingRows(productListClass, response.data);
                }
            }
        });
    })
}
function AddwithEveryRow() {
    $('.js-example-basic-single').select2();
    productListOnChange();
    productDetailsOnChange();
}

function fillCorrespondingRows(productListClass, data) {
    if (data.rate != 0) {
        $(productListClass).closest("tr").eq(0).find("td:eq(2)").find(".productQty").val(1);
        $(productListClass).closest("tr").eq(0).find("td:eq(3)").find(".productPrice").val(data.rate);
        $(productListClass).closest("tr").eq(0).find("td:eq(4)").find(".productDiscount").val(0);
        $(productListClass).closest("tr").eq(0).find("td:eq(3)").find(".productPrice").trigger("change")
    }
}
function AddProduct(btn) {
    var table = document.getElementById("dyn");
    var rows = table.getElementsByTagName("tr");
    var outrows = rows[rows.length - 1].outerHTML;
    var IndexOfLastRowJ = document.getElementById("flagJ").value;
    var IndexOfNextRowJ = parseInt(IndexOfLastRowJ) + 1;
    var IndexOfLastRowI = document.getElementById("flag").value;
    document.getElementById("flagJ").value = IndexOfNextRowJ;
    outrows = outrows.replaceAll('Products[' + IndexOfLastRowJ + ']', 'Products[' + IndexOfNextRowJ + ']')
    outrows = outrows.replaceAll('Products_' + IndexOfLastRowJ, 'Products_' + IndexOfNextRowJ)
    outrows = outrows.replaceAll('addJ_' + IndexOfLastRowI + '_' + IndexOfLastRowJ, 'addJ_' + IndexOfLastRowI + '_' + IndexOfNextRowJ)
    outrows = outrows.replaceAll('addI_' + IndexOfLastRowI + '_' + IndexOfLastRowJ, 'addI_' + IndexOfLastRowI + '_' + IndexOfNextRowJ)
    var newrow = table.insertRow();
    newrow.innerHTML = outrows;
    $(btn).closest("tr").find("td:eq(7)").find("button").attr("hidden", true)
    //$(newrow).find("td:eq(0)").find("span:eq(0)").remove()
    //$(newrow).find("td:eq(0)").find("select:eq(0)").select2({
    //    width: '100%'
    //});
    $(newrow).find("td:eq(1)").find("span:eq(0)").remove()
    $(newrow).find("td:eq(1)").find("select:eq(0)").select2({
        width: '100%'
    });
    $(newrow).find("td:eq(0)").find("select:eq(0)").attr("hidden", true)
    $(newrow).find("td:eq(0)").find("span:eq(0)").attr("hidden", true)
    $(newrow).find("td:eq(0)").find("span:eq(1)").attr("hidden", true)
    var addbtn = document.getElementById(btn.id);
    addbtn.classList.add("invisible");
    AddwithEveryRow();

}
function countTotal(that) {
    var QTY = $(that).closest("tr").find("td:eq(2)").find("input").val();
    var Price = $(that).closest("tr").find("td:eq(3)").find("input").val();
    var Discount = $(that).closest("tr").find("td:eq(4)").find("input").val();
    var TotalPrice = QTY * Price - Discount;
    $(that).closest("tr").find("td:eq(5)").find("input").val(TotalPrice);
}