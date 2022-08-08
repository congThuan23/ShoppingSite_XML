
function RenderDanhMuc(list) {
    let html = "";

    if (list == null) return "";

    let i = 1;
    list.forEach(item => {
        html += `
            <tr id="dm_${item.id}">
                <td>${i++}</td>
                <td class="text-center">
                    ${item.tenDanhMuc}
                </td>
                <td class="text-end">
                    <a href="/admin/DanhMuc/Edit?id=${item.id}" class="edit-btn custom-btn">
                        <i class="bi bi-pencil-fill"></i>
                    </a>
                    <button class="remove-btn custom-btn" onclick="removeRow(${item.id},'/Admin/DanhMuc/Delete', 'dm_${item.id}')">
                        <i class="bx bx-x"></i>
                    </button>
                </td>
            </tr>
        `;
    });
    return html;
}

function RenderSanPham(list) {
    let html = "";

    if (list == null) return "";

    let i = 1;
    list.forEach(item => {
        html += `
            <tr id="sp_@${item.id}">
                <td>
                    <div class="d-flex align-items-center position-relative stt" data-stt="${i++}">
                        <div>
                            <img class="custom-img" src="/uploadedImages/${item.anh}" />
                        </div>
                        <p class="font-bold ms-3 mb-0">${item.tenSanPham}</p>
                    </div>

                </td>
                <td class="text-center">
                    ${item.tenDanhMuc}
                </td>
                <td class="text-center">
                   ${item.soLuong}
                </td>
                <td class="text-center">
                    ${numeral(item.gia).format("0,0")} ₫
                </td>
                <td class="text-end">
                    <div class="d-flex justify-content-center">
                        <a href="/admin/SanPham/Edit?id=${item.id}" class="edit-btn custom-btn">
                            <i class="bi bi-pencil-fill"></i>
                        </a>
                        <button class="remove-btn custom-btn" onclick="removeRow(${item.id},'/Admin/SanPham/Delete', 'sp_@${item.id}')">
                            <i class="bx bx-x"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    });
    return html;
}

function RenderNguoiDung(list) {
    let html = "";

    if (list == null) return "";

    let i = 1;
    list.forEach(item => {
        html += `
            <tr>
                <td>
                    <span class="font-bold position-relative stt" data-stt="${i}">${item.ten}</span>
                </td>

                <td>
                    ${item.tenTK}
                </td>
                <td class="col-1">
                    <span class="password"> ${item.matKhau}</span>
                </td>

                <td class="text-center">
                    <span class="quyen"> ${item.quyen}</span>

                </td>
                <td class="text-center">
                    <span class=" ${item.trangThai ? "actived" : "locked"}"> ${item.trangThai ? "Hoạt động" : "Khoá"}</span>
                </td>

                <td class="text-center">
                    <a href="/admin/NguoiDung/Edit?id=${item.id}" class="edit-btn custom-btn">
                        <i class="bi bi-pencil-fill"></i>
                    </a>
                </td>
            </tr>
        `;
    });
    return html;
}

function Search(idInput, url, idNullFeedback, idTable, renderFunc) {
    const input = document.getElementById(idInput);
    const nullFeedBack = document.getElementById(idNullFeedback);
    const table = document.getElementById(idTable);

    if (input == null) return;
    if (table == null) return;

    const tbody = table.querySelector('tbody');

    input.onkeyup = function () {
        let keyword = this.value.trim();

        setTimeout(() => {
            let text = this.value.trim();
            if (text == keyword) {
                $.ajax({
                    type: 'post',
                    datatype: 'JSON',
                    data: { keyword: keyword },
                    url: url,
                    success: function (result) {
                        tbody.innerHTML = renderFunc(result.list);
                        nullFeedBack.innerText = result.message;
                    }
                })
            }
        }, 200);
    }
}

Search('search-danhmuc', '/Admin/DanhMuc/TimKiemAjax', 'danhmuc-null', 'danhmuc-table', RenderDanhMuc);
Search('search-sanpham', '/Admin/SanPham/TimKiemAjax', 'sanpham-null', 'sanpham-table', RenderSanPham);
Search('search-nguoidung', '/Admin/NguoiDung/TimKiemAjax', 'nguoidung-null', 'nguoidung-table', RenderNguoiDung);

function removeRow(id, url, rowId) {
    if (confirm('Bạn có muốn xoá không?')) {
        $.ajax({
            type: 'post',
            datatype: 'JSON',
            data: { id: id },
            url: url,
            success: function (result) {
                if (result.error == false) {
                    document.getElementById(rowId).remove();
                } else {
                    alert('Xoá lỗi. Vui lòng thử lại');
                }
            }
        })
    }
}