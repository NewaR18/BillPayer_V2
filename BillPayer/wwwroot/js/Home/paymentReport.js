var dataTable;
$(document).ready(function () {
    $('#tblData thead tr')
        .clone(true)
        .addClass('filters')
        .appendTo('#tblData thead');
    var url = window.location.search;
    debugger;
    switch (url) {
        case url.includes("toPay") ? url : "Other":
            LoadDataTable("toPay")
            break;
        case url.includes("toReceive") ? url : "Other":
            LoadDataTable("toReceive")
            OnClickDetails(false);
            break;
        case url.includes("received") ? url : "Other":
            LoadDataTable("received")
            OnClickDetails(true);
            break;
        default:
            LoadDataTable()
            break;
    }
    
})
function LoadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Bills/Home/GetAll",
            "type": "POST",
            "contentType": "application/json",
            "data": function (d) {
                debugger;
                d.status = status == undefined ? '' : status;
                return JSON.stringify(d);
            }
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "date", "width": "10%" },
            { "data": "price", "width": "10%" },
            {
                "data": "billSummaryId",
                "render": function (data) {
                    if (status != null) {
                        if (status === "received" || status === "toReceive") {
                            return `
                                <span hidden>${data}</span>
                                <button class="btn btn-primary px-4 py-1 btnDetails">Details</button>
                            `
                        }
                    }
                    return "";

                },
                "width": "15%"
            }
        ],
        "columnDefs": [
            {
                targets: [1],
                render: function (data, type, row) {
                    if (type === 'display') {
                        if (moment(data).format('YYYY') === '0001') {
                            return '';
                        }
                        return moment(data).format('YYYY-MM-DD');
                        //return moment(data).format('YYYY-MM-DD hh:mm A');
                    }
                    return data;
                }
            }
        ],
        orderCellsTop: true,
        ordering: false,
        fixedHeader: true,
        searching: true,
        processing: true,
        serverSide: true,
        sInfo: "Showing _START_ to _END_ of _TOTAL_ entries",
        dom: 'rt<"bottom"<"d-flex justify-content-between"lip>>',
        initComplete: function () {
            var api = this.api();
            $("#tblData_info").addClass("pt-2");
            api
                .columns()
                .eq(0)
                .each(function (colIdx) {
                    var cell = $('.filters th').eq(
                        $(api.column(colIdx).header()).index()
                    );
                    var title = $(cell).text();
                    if (title != "") {
                        $(cell).html('<input type="text" id="' + title.replace(' ', '') + '" style="width:100px;" />');
                    }
                    $(
                        'input',
                        $('.filters th').eq($(api.column(colIdx).header()).index())
                    )
                        .off('keyup change')
                        .on('change', function (e) {
                            // Get the search value
                            $(this).attr('title', $(this).val());
                            var regexr = '({search})';

                            var cursorPosition = this.selectionStart;
                            api
                                .column(colIdx)
                                .search(
                                    this.value != ''
                                        ? regexr.replace('{search}', '(((' + this.value + ')))')
                                        : '',
                                    this.value != '',
                                    this.value == ''
                                )
                                .draw();
                        })
                        .on('keyup', function (e) {
                            e.stopPropagation();

                            $(this).trigger('change');
                            $(this)
                                .focus()[0]
                                .setSelectionRange(cursorPosition, cursorPosition);
                        });
                });
        },
    })
}
function OnClickDetails(paidStatus) {
    $(document).on('click', '.btnDetails', function () {
        debugger;
        var tranId = $(this).closest("td").find("span").text();
        var billSummaryId = 0
        if (tranId != null) {
            billSummaryId = parseInt(tranId)
        }
        $.ajax({
            type: 'GET',
            url: '/Bills/Home/ReceivableDetails',
            data: { billSummaryId: billSummaryId, paid: paidStatus },
            success: function (data) {
                debugger;
                $('#detailsPortal').html("");
                if (data != null) {
                    if (data.success) {
                        var dynamicHtml = `<div class="d-flex justify-content-end">
                            <div>
                                <b><h6>Date : ${moment(data.data.date).format("YYYY-MM-DD")}</h6></b>
                            </div>
                            </div>`
                        dynamicHtml += `<div class="card-body">
                            <table id="tblProductDetails" class="table-bordered table table-striped" style="width:100%!important">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Price</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>`;
                        $('#detailsPortal').html(dynamicHtml);
                        productDataTable = $('#tblProductDetails').DataTable({
                            data: data.data.bhukkadsTotal,
                            columns: [
                                { data: 'bhukkad' },
                                { data: 'total' }
                            ],
                            paging: false,
                            searching: false,
                            info: false,
                            ordering: false
                        });
                        $('#detailsModal').modal('show');
                    } else {
                        toastr.error(data.message)
                    }
                }

            },
            error: function (req, status, error) {
                debugger;
                console.log(status);
                console.log(req);
                console.log(error);
            }
        });
    });
    $(document).on('click', '.paid', function () {
        debugger;
        var tranId = $(this).closest(".paidDiv").find("span").text();
        var bhukkadId = 0
        if (tranId != null) {
            bhukkadId = parseInt(tranId)
        }
        $.ajax({
            type: 'GET',
            url: '/Bills/Home/PaidByUser',
            data: { bhukkadsId: bhukkadId },
            success: function (data) {
                $('#paymentPortal').html("");
                if (data != null) {
                    debugger;
                    if (data.success) {
                        if (data.data != null) {
                            if (data.data.expenses != null) {
                                $("#expenseSpan").text(data.data.expenses.toFixed(2));
                            }
                            if (data.data.totalDue != null) {
                                $("#totalDueSpan").text(data.data.totalDue);
                            }
                            if (data.data.lastPaymentDate != null) {
                                $("#paymentDateSpan").text(data.data.lastPaymentDate);
                            }
                            if (data.data.toReceive != null) {
                                $("#toReceiveSpan").text(data.data.toReceive);
                            }
                        }
                        dataTable.ajax.reload();
                        $('#exampleModal').modal('hide');
                        toastr.success(data.message);
                    } else {
                        if (data.paid) {
                            dataTable.ajax.reload();
                            $('#exampleModal').modal('hide');
                            toastr.error(data.message)
                        } else {
                            toastr.error(data.message)
                        }

                    }
                }
            },
            error: function (req, status, error) {
                console.log(status);
                console.log(req);
                console.log(error);
            }
        });
    });
}