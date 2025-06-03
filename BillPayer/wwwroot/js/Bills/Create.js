$(document).ready(function () {
    $('.js-example-basic-single').select2();
    productListOnChange();
    productDetailsOnChange();
    totalPriceChange();
});
function productDetailsOnChange() {
    $(".productQty, .productPrice, .productDiscount").on('change', function () {
        countTotal(this);
        $(".totalPricePerProduct").trigger('change')
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
        console.log(val);
    })
}
function totalPriceChange() {
    $(".totalPricePerProduct").on('change', function () {
        debugger;
        var totalPrice = findTotal();
        document.getElementById('TotalOfAll').innerHTML = totalPrice;
    })
}
function findTotal() {
    var arraysOfTotalPrice = document.getElementsByClassName('totalPricePerProduct');
    var total = 0.00;
    for (var i = 0; i < arraysOfTotalPrice.length; i++) {
        if (parseFloat(arraysOfTotalPrice[i].value))
            total += parseFloat(arraysOfTotalPrice[i].value);
    }
    return total
    
}
function AddwithEveryRow() {
    $('.js-example-basic-single').select2();
    productListOnChange();
    productDetailsOnChange();
    totalPriceChange();
}

function fillCorrespondingRows(productListClass, data) {
    debugger;
    if (data.rate != 0) {
        $(productListClass).closest("tr").eq(0).find("td:eq(2)").find(".productQty").val(1);
        $(productListClass).closest("tr").eq(0).find("td:eq(3)").find(".productPrice").val(data.rate);
        $(productListClass).closest("tr").eq(0).find("td:eq(4)").find(".productDiscount").val(0);
        $(productListClass).closest("tr").eq(0).find("td:eq(3)").find(".productPrice").trigger("change")
    }
}
function AddUser(btn) {
    var table = document.getElementById("dyn");
    var rows = table.getElementsByTagName("tr");
    var outrows = rows[rows.length - 1].outerHTML;
    var IndexOfLastRowI = document.getElementById("flag").value;
    var IndexOfNextRowI = parseInt(IndexOfLastRowI) + 1;
    var IndexOfLastRowJ = document.getElementById("flagJ").value;
    var IndexOfNextRowJ = 0;
    document.getElementById("flag").value = IndexOfNextRowI;
    outrows = outrows.replaceAll('Products[' + IndexOfLastRowJ + ']', 'Products[' + IndexOfNextRowJ + ']')
    outrows = outrows.replaceAll('Products_' + IndexOfLastRowJ, 'Products_' + IndexOfNextRowJ)
    outrows = outrows.replaceAll('BhukkadsList[' + IndexOfLastRowI + ']', 'BhukkadsList[' + IndexOfNextRowI + ']')
    outrows = outrows.replaceAll('BhukkadsList_' + IndexOfLastRowI, 'BhukkadsList_' + IndexOfNextRowI)
    outrows = outrows.replaceAll('addI_' + IndexOfLastRowI + '_' + IndexOfLastRowJ, 'addI_' + IndexOfNextRowI + '_' + IndexOfNextRowJ)
    outrows = outrows.replaceAll('addJ_' + IndexOfLastRowI + '_' + IndexOfLastRowJ, 'addJ_' + IndexOfNextRowI + '_' + IndexOfNextRowJ)
    var newrow = table.insertRow();
    newrow.innerHTML = outrows;
    
    $(newrow).find("td:eq(1)").find("span:eq(0)").remove()
    $(newrow).find("td:eq(1)").find("select:eq(0)").select2({
        width: '100%'
    });
    $(newrow).find("td:eq(0)").find("select:eq(0)").attr("hidden", false)
    $(newrow).find("td:eq(0)").find("span:eq(0)").attr("hidden", false)
    $(newrow).find("td:eq(0)").find("span:eq(1)").attr("hidden", false)
    $("p").removeClass("myClass yourClass")
    var addbtn = document.getElementById(btn.id);
    $(btn).closest("tr").find("td:eq(6)").find("button").attr("hidden", true)
    addbtn.classList.add("invisible");
    $("#flagJ").val(0);
    AddwithEveryRow();
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
    console.log(outrows);
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
    debugger;
    var QTY = $(that).closest("tr").find("td:eq(2)").find("input").val();
    var Price = $(that).closest("tr").find("td:eq(3)").find("input").val();
    var Discount = $(that).closest("tr").find("td:eq(4)").find("input").val();
    var TotalPrice = QTY * Price - Discount;
    $(that).closest("tr").find("td:eq(5)").find("input").val(TotalPrice);
}