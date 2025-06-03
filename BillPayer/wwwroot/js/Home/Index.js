var dataTable;
var productDataTable;
$(document).ready(function () {
    LoadDataTable();
    OnClickPay();
    OnClickDashboardElements();
})
function OnClickDashboardElements() {
    $(document).on('click', '.divExpenses', function () {
        window.location.href = "/Bills/Home/PaymentReport?status=expenses";
    });
    $(document).on('click', '.divToReceive', function () {
        window.location.href = "/Bills/Home/PaymentReport?status=toReceive";
    });
    $(document).on('click', '.divPaymentDue', function () {
        window.location.href = "/Bills/Home/PaymentReport?status=toPay";
    });
}
function OnClickPay() {
    $(document).on('click', '.payDetails', function () {
        debugger;
        var tranId = $(this).closest("td").find("span").text();
        var bhukkadId = 0
        if (tranId != null) {
            bhukkadId = parseInt(tranId)
        }
        $.ajax({
            type: 'GET',
            url: '/Bills/Home/PayDetails',
            data: { bhukkadsId: bhukkadId },
            success: function (data) {
                $('#paymentPortal').html("");
                let qrURL = "";
                if (data != null) {
                    if (data.success) {
                        debugger;
                        qrURL = `{"eSewa_id":"${data.data.esewaPhone}","name":"${data.data.esewaName}"}`;
                        //var qrURL2 = '<div id="qrCodeData" data-url="{&quot;eSewa_id&quot;:&quot;9861141726&quot;,&quot;name&quot;:&quot;Sudip Shrestha&quot;}"></div>'
                        var dynamicHtml = `<div class="d-flex justify-content-evenly"><div>`;
                        dynamicHtml += `<h4>To  ${data.data.name}</h4><h5>Esewa Name : ${data.data.esewaName}</h5><h5>Phone Number : ${data.data.esewaPhone}</h5><h5 class="mt-2">Total : ${data.data.totalOfPerson}</h5></div>`;
                        dynamicHtml += `<div><div id="qrCode"></div><div id="qrCodeData" data-url='${qrURL}'></div></div></div>`;
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
                        dynamicHtml += `<div class="d-flex justify-content-center mt-2 paidDiv">
                            <span hidden>${data.data.bhukkadsId}</span>
                            <button class="btn btn-primary px-4 py-1 paid">Paid</button>
                        </div>
                        `
                        $('#paymentPortal').html(dynamicHtml);
                        const uri = document.getElementById("qrCodeData").getAttribute('data-url');
                        new QRCode(document.getElementById("qrCode"),
                            {
                                text: uri,
                                width: 150,
                                height: 150
                            });
                        productDataTable = $('#tblProductDetails').DataTable({
                            data: data.data.productListOfBhukkad,
                            columns: [
                                { data: 'name' },
                                { data: 'amount' }
                            ],
                            paging: false,
                            searching: false,
                            info: false,
                            ordering: false
                        });
                        $('#exampleModal').modal('show');
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
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Bills/Home/GetBhukkadList"
        },
        "columns": [
            { "data": "date", "width": "15%" },
            { "data": "price", "width": "15%" },
            {
                "data": "bhukkadsId",
                "render": function (data) {
                    return `
                       <span hidden>${data}</span>
                       <button class="btn btn-primary px-4 py-1 payDetails">Pay</button>
                    `
                },
                "width": "15%"
            }
        ],
        "columnDefs": [
            {
                targets: 0,
                render: function (data, type, row) {
                    if (type === 'display') {
                        if (moment(data).format('YYYY') === '0001') {
                            return '';
                        }
                        return moment(data).format('YYYY-MM-DD');
                    }
                    return data;
                }
            }
        ],
        dom: 'rt',
    })
}
$.ajax({
    type: "GET",
    url: "/bills/home/getLineTrend",
    dataType: "json",
    success: function (data) {
        if (data.success) {
            if (data.data.length > 0) {
                var chartData = {
                    labels: [],
                    datasets: [{
                        label: 'Deposit Transaction',
                        backgroundColor: "rgba(75, 192, 192, 0.1)",
                        borderColor: "rgb(75, 192, 192)",
                        borderWidth: 2,
                        pointRadius: 2,
                        data: []
                    }]
                };
                for (var i = 0; i < data.data.length; i++) {
                    const dateStr = data.data[i].date.slice(5);
                    const [month, day] = dateStr.split('-');
                    const date = new Date(0, parseInt(month, 10) - 1, parseInt(day, 10));
                    const formattedDate = date.toLocaleString('en-US', { month: 'short', day: 'numeric' });
                    chartData.labels.push(formattedDate);
                    chartData.datasets[0].data.push(data.data[i].amount);
                }
                var ctx = document.getElementById("myChart").getContext("2d");
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: chartData,
                    options: {
                        scales: {
                            xAxes: [{
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Date'
                                },
                                gridLines: {
                                    drawBorder: false,
                                },

                            }],
                            yAxes: [{
                                scaleLabel: {
                                    display: false,
                                    labelString: 'Amount'
                                },
                                gridLines: {
                                    drawBorder: false,
                                },
                                ticks: {
                                    callback: function (value, index, ticks) {
                                        return 'Rs ' + value;
                                    }
                                }
                            }]
                        },
                        legend: {
                            display: false
                        },
                        tooltips: {
                            enabled: true,
                            intersect: false,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    var label = '';
                                    label += 'Rs ';
                                    label += tooltipItem.yLabel == '0' ? '0' : tooltipItem.yLabel;
                                    return label;
                                }
                            },
                            bodyFontFamily: 'Verdana',
                            bodyFontSize: 12,
                            bodyFontWeight: '400',
                            bodyFontColor: 'white',
                            backgroundColor: 'rgba(1,41,112,0.7)'
                        }
                    }
                });
            }
        }
    },
    error: function (req, status, error) {
        console.log(status);
        console.log(req);
        console.log(error);
    }
});