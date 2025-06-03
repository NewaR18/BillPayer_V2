const BhukkadsList = [1, 2, 3, 4, 5, 6, 7];
for (const item of BhukkadsList) {
    $(".Bhukkad_" + item).attr("hidden", true);
    $(".addProduct_" + item).attr("hidden", true);
}

$(document).ready(function () {
    $('.js-example-basic-single').select2();
    $(".paidByUser").on('change', function () {
        const urlParams = new URLSearchParams(window.location.search);
        const BillSummaryId = parseInt(urlParams.get('Id'))
        var payerId = $(".paidByUser").val();
        if (BillSummaryId && BillSummaryId != "" && payerId && payerId != ""){
            $.ajax({
                type: "POST",
                url: "/bills/bill/UpdatePayerId",
                dataType: "json",
                data: { billSummaryId: BillSummaryId, payerId: payerId },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message)
                    } else {
                        toastr.error(data.message)
                    }
                }
            });
        }
    })
    $("#BillSummary_Date").on('change', function () {
        debugger;
        OnChangeDate();
    })
    function OnChangeDate() {
        const urlParams = new URLSearchParams(window.location.search);
        const BillSummaryId = parseInt(urlParams.get('Id'))
        const DateEaten = $("#BillSummary_Date").val();
        if (BillSummaryId && BillSummaryId != "" && DateEaten && DateEaten != "") {
            $.ajax({
                type: "POST",
                url: "/bills/bill/UpdateDate",
                dataType: "json",
                data: { billSummaryId: BillSummaryId, dateEaten: DateEaten },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message)
                    } else {
                        toastr.error(data.message)
                    }
                }
            });
        }
    }
})