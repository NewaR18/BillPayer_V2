var dataTable;
$(document).ready(function () {
    LoadDataTable();
})
function LoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/bills/product/getall"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "rate", "width": "15%" },
            { "data": "quantityType", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div>
                            <a href="/Bills/Product/Edit?Id=${data}"><i class="bi bi-pen"></i></a> || 
                            <a href="/Bills/Product/Details?Id=${data}"><i class="bi bi-eye"></i></a> || 
                            <a onClick="Delete('/Bills/Product/Delete/${data}')"><i class="bi bi-trash"></i></a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
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