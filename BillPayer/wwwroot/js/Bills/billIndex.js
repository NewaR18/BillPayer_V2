var dataTable;
$(document).ready(function () {
    $('#tblData thead tr')
        .clone(true)
        .addClass('filters')
        .appendTo('#tblData thead');
    LoadDataTable();
})
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/bills/bill/getbillslist"
        },
        "columns": [
            { "data": "date", "width": "35%" },
            { "data": "grandTotal", "width": "35%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div>
                            <a href="/Bills/Bill/Details?Id=${data}"><i class="bi bi-eye"></i></a> || 
                            <a onClick="Delete('/Bills/Bill/DeleteBillSummary/${data}')"><i class="bi bi-trash"></i></a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ],
        "order": [0, 'desc'],
        "columnDefs": [
            {
                targets: [0],
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
        fixedHeader: true,
        searching: true,
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
                        $(cell).html('<input type="text" id="' + title.replace(' ', '') + '" class="form-control w-50 h-75 border" placeholder="Enter '+title+ '"/>');
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
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                method: "delete",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message)
                    } else {
                        toastr.error(data.message)
                    }
                }
            });
        }
    });
}