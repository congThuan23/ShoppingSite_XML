using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuoiKi.Areas.admin.Models
{
    public class ThongKeKhachHangMuaNhieu
    {

        [StringLength(200)]
        [DisplayName("Tên Khách Hàng")]
        public string ten { get; set; }

        [DisplayName("Số Lần Mua Hàng")]
        public int soLanMuaHang { get; set; }

    }
}
