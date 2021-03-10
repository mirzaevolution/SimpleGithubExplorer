$(document).ready(function () {
    datatables.init();
});

var datatables = {
    init: function () {
        try {
            var datatable = $("#tableIssue").DataTable({ scrollX: true });
        } catch (error) {
            console.log(error);
        }
    }
}