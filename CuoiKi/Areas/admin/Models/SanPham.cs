using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CuoiKi.Areas.admin.Models
{
    public class SanPham
    {
        public string id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string tenSanPham {get; set;}

        [Required(ErrorMessage = "Yêu cầu nhập số lượng")]
        [Display(Name = "Số lượng")]
        public int soLuong { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập đơn giá")]
        [Display(Name = "Đơn giá")]
        public int gia   { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn loại sản phẩm")]
        [Display(Name = "Mã loại sản phẩm")]
        public string maDM { get; set; }

        [Display(Name = "Loại sản phẩm")]
        public string tenDanhMuc { get; set; }

        public int tinhTrang { get; set; }

        [Display(Name = "Size")]
        public List<int> listSize { get; set; }

        [Display(Name = "Hình ảnh")]
        public string anh { get; set; }
    }
}