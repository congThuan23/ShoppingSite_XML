using CuoiKi.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Text;

namespace CuoiKi.Areas.admin.Controllers
{
    public class ThongKeController : Controller
    {
        SqlDataAdapter da;
        SqlDataAdapter daHDChuaDuyet;
        SqlDataAdapter daHDDaDuyet;
        SqlDataAdapter daTopKHMuaNhieu;

        String xml;

        [HttpGet]
        // GET: admin/ThongKe
        public ActionResult Index()
        {

            string sql = "select  distinct s.id, s.tenSanPham, sum(c.soLuong) " +
            "from SanPham as s, ChiTietHoaDon as c where(s.id = c.maSP)" +
            
            "group by c.maSP, s.id, s.tenSanPham " +
            "having sum(c.soLuong) in (select distinct top 5 sum(soLuong) from ChiTietHoaDon group by maSP order by sum(soLuong) desc)" +
            "order by sum(c.soLuong) desc";
            string sqlHDChuaDuyet = "select count(HoaDon.id) as soLuong from HoaDon where trangThai = 0";
            string sqlHDDaDuyet = "select count(HoaDon.id) as soLuong from HoaDon where trangThai = 1";
            string sqlKHMuaNhieu = "select distinct top 3 Count(h.maKH) as soLanMua, n.ten from NguoiDung as n, HoaDon as h where(n.id = h.maKH) and n.maQuyen = 3 group by n.ten order by soLanMua desc";

            SqlConnection con = Connect.connect();
            con.Open();
            da = new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);


            List<ThongKeSanPhamBanChay> list = new List<ThongKeSanPhamBanChay>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows.Count > 0)
                {
                    var thongke = new ThongKeSanPhamBanChay();
                    thongke.maGiay = dt.Rows[i].ItemArray[0].ToString().Trim();
                    thongke.tenGiay = dt.Rows[i].ItemArray[1].ToString().Trim();
                    thongke.soLuongBan = Int32.Parse(dt.Rows[i].ItemArray[2].ToString().Trim());

                    list.Add(thongke);
                }
            }

            // đếm đơn hàng chưa duyệt
            daHDChuaDuyet = new SqlDataAdapter(sqlHDChuaDuyet, con);
            DataTable dtHDChuaDuyet = new DataTable();
            daHDChuaDuyet.Fill(dtHDChuaDuyet);
            int soHDChuaDuyet = 0;

            for (int i = 0; i < dtHDChuaDuyet.Rows.Count; i++)
            {
                soHDChuaDuyet = Int32.Parse(dtHDChuaDuyet.Rows[i].ItemArray[0].ToString());
            }

            ViewBag.HDChuaDuyet = soHDChuaDuyet.ToString();

            // Đếm đơn hàng đã duyệt
            daHDDaDuyet = new SqlDataAdapter(sqlHDDaDuyet, con);
            DataTable dtHDDaDuyet = new DataTable();
            daHDDaDuyet.Fill(dtHDDaDuyet);
            int soHDDaDuyet = 0;

            for (int i = 0; i < dtHDDaDuyet.Rows.Count; i++)
            {
                soHDDaDuyet = Int32.Parse(dtHDDaDuyet.Rows[i].ItemArray[0].ToString());
            }

            ViewBag.HDDaDuyet = soHDDaDuyet.ToString();

            // top Khách hàng mua nhiều
            daTopKHMuaNhieu = new SqlDataAdapter(sqlKHMuaNhieu, con);
            DataTable dtTopKHMuaNhieu = new DataTable();
            daTopKHMuaNhieu.Fill(dtTopKHMuaNhieu);

            ViewBag.name1 = dtTopKHMuaNhieu.Rows[0].ItemArray[1].ToString().Trim();
            ViewBag.num1 = dtTopKHMuaNhieu.Rows[0].ItemArray[0].ToString().Trim();

            if (dtTopKHMuaNhieu.Rows[1].ItemArray[0] != null)
            {
                ViewBag.name2 = dtTopKHMuaNhieu.Rows[1].ItemArray[1].ToString().Trim();
                ViewBag.num2 = dtTopKHMuaNhieu.Rows[1].ItemArray[0].ToString().Trim();
            }
            else
            {
                ViewBag.name2 = "";
                ViewBag.num2 = "0";
            }

            if (dtTopKHMuaNhieu.Rows[2].ItemArray[0] != null)
            {
                ViewBag.name3 = dtTopKHMuaNhieu.Rows[2].ItemArray[1].ToString().Trim();
                ViewBag.num3 = dtTopKHMuaNhieu.Rows[2].ItemArray[0].ToString().Trim();
            }
            else
            {
                ViewBag.name3 = "";
                ViewBag.num3 = "0";
            }

            return View(list);
        }

    }
}