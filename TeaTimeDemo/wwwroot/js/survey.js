var dataTable;
$(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/survey/getall'  // 使用 AJAX 獲取所有 Notes 的資料
        },
        "fixedHeader": true,
        "columns": [
            { data: 'categoryName', "width": "10%" },  // 新增類別欄位
            { data: 'title', "width": "15%" },
            { data: 'stationName', "width": "10%" },
            { data: 'description', "width": "15%" },
            { data: 'questionNum', "width": "5%" }, // 顯示頁數
            { data: 'isPublished', "width": "5%", "render": function (data) { return data ? '是' : '否'; } }, // 顯示是否發佈
            { data: 'createTime', "width": "10%", "render": function (data) { return new Date(data).toLocaleString(); } }, // 顯示創立時間
            { data: 'completeTime', "width": "10%", "render": function (data) { return data ? new Date(data).toLocaleString() : '未完成'; } }, // 顯示完成時間
            { data: 'jobName', "width": "10%" },
            {
                data: 'id',
                "render": function (data, type, row) {
                    var publishButton = row.isPublished
                        ? `<button class="btn btn-secondary mx-2" onclick="togglePublish(${data})">取消發佈</button>`
                        : `<button class="btn btn-secondary mx-2" onclick="togglePublish(${data})">發佈</button>`;

                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/survey/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> 編輯
                            </a>
                            <button class="btn btn-danger mx-2" onClick=Delete('/admin/survey/delete/${data}')>
                                <i class="bi bi-trash-fill"></i> 刪除
                            </button>
                            ${publishButton}
                        </div>`;
                },
                "width": "20%"
            }
        ]
    });
}

// 切換是否發佈的狀態
function togglePublish(id) {
    $.ajax({
        url: `/admin/survey/togglepublish/${id}`,
        type: 'POST',
        success: function (data) {
            if (data.success) {
                dataTable.ajax.reload();  // 刷新表格
                toastr.success(data.message);
            } else {
                toastr.error(data.message);
            }
        }
    });
}

function Delete(url) {
    Swal.fire({
        title: "確定刪除?",
        text: "您將無法恢復此狀態！",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "是的, 刪除它!",
        cancelButtonText: "取消"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
