using CuoiKi.Areas.admin.Convert;
using CuoiKi.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace CuoiKi.Areas.admin.Controllers
{
    public class HoaDonController : Controller
    {
        // GET: admin/DonHang
        public ActionResult Index()
        {
            if (!Directory.Exists(Server.MapPath("~/App_Data/HoaDon.xml")))
            {
                ConvertHoaDonToXml();
                ConvertNguoiDungToXml();
            }

            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/HoaDon.xml"));

            //lấy danh sách người dùng để lấy tên KH
            XmlDocument xmlNguoiDung = new XmlDocument();
            xmlNguoiDung.Load(Server.MapPath("~/App_Data/NguoiDung.xml"));
            string tenKH = "";
            

                List<HoaDon> List = new List<HoaDon>();
            foreach (XmlElement ele in xml.GetElementsByTagName("HoaDon"))
            {
                HoaDon hoaDon = new HoaDon();
                hoaDon.id = int.Parse(ele.GetAttribute("id"));
                foreach (XmlElement e in xmlNguoiDung.GetElementsByTagName("NguoiDung"))
                {
                    if (e.GetAttribute("id") == ele.GetAttribute("maKH"))
                    {
                        hoaDon.tenKH = e.GetAttribute("ten");
                        break;
                    }
                }
                hoaDon.maKH = int.Parse(ele.GetAttribute("maKH"));
                hoaDon.ngayNhap = DateTime.Parse(ele.GetAttribute("ngayNhap"));
                hoaDon.trangThai = int.Parse(ele.GetAttribute("trangThai"));
                List.Add(hoaDon);
            }
            return View(List);
        }

        public void ConvertHoaDonToXml()
        {
            HoaDonConverter hoaDonConverter = new HoaDonConverter();
            String xml = hoaDonConverter.toXMl();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(Server.MapPath("~/App_Data/HoaDon.xml"));

        }

        public void ConvertNguoiDungToXml()
        {
            NguoiDungConverter nguoiDungConverter = new NguoiDungConverter();
            string xml = nguoiDungConverter.toXMl();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(Server.MapPath("~/App_Data/NguoiDung.xml"));
        }

        public void ConvertChiTietHoaDonToXml()
        {
            ChiTietHoaDonConverter chiTietHoaDonConverter = new ChiTietHoaDonConverter();
            string xml = chiTietHoaDonConverter.toXMl();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(Server.MapPath("~/App_Data/ChiTietHoaDon.xml"));
        }

        public ActionResult Details(int id)
        {
            ConvertChiTietHoaDonToXml();
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/ChiTietHoaDon.xml"));

            XmlDocument xmlDocSP = new XmlDocument();
            xmlDocSP.Load(Server.MapPath("~/App_Data/SanPham.xml"));

            foreach (XmlElement ele in xml.GetElementsByTagName("ChiTietHoaDon"))
            {
                ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon();
                if(ele.GetAttribute("maHD") == id + "")
                {
                    foreach (XmlElement e in xmlDocSP.GetElementsByTagName("SanPham"))
                    {
                        if (e.GetAttribute("id") == ele.GetAttribute("maSP"))
                        {
                            chiTietHoaDon.tenSP = e.GetAttribute("tenSanPham");
                            break;
                        }
                    }
                    chiTietHoaDon.id = int.Parse(ele.GetAttribute("id"));
                    chiTietHoaDon.maHD = id;
                    chiTietHoaDon.maSP = int.Parse(ele.GetAttribute("maSP"));
                    chiTietHoaDon.soLuong = int.Parse(ele.GetAttribute("soLuong"));
                    chiTietHoaDon.giaBan = int.Parse(ele.GetAttribute("giaBan"));
                    chiTietHoaDon.thanhTien = chiTietHoaDon.soLuong * chiTietHoaDon.giaBan;
                    list.Add(chiTietHoaDon);
                }
                
            }
            return View(list);
        }

        public ActionResult Duyet(int id)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/HoaDon.xml"));
            foreach (XmlElement ele in xml.GetElementsByTagName("HoaDon"))
            {
                if (ele.GetAttribute("id") == id + "")
                {
                    if (ele.GetAttribute("trangThai") == "1")
                    {
                        ele.SetAttribute("trangThai", "0");
                    }
                    else
                    {
                        ele.SetAttribute("trangThai", "1");
                    }
                    xml.Save(Server.MapPath("~/App_Data/HoaDon.xml"));
                    UpdateToSQL(id);
                    break;
                }
            }
            return RedirectToAction("Index", "HoaDon");
        }

        void UpdateToSQL(int id)
        {
            DataTable dt = new DataTable();
            string filepath = Server.MapPath("~/App_Data/HoaDon.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(filepath);
            DataView dv = new DataView(ds.Tables[0]);
            dt = dv.Table;
            string sql;
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["id"].ToString() == id + "")
                {
                    sql = "update HoaDon set trangThai = " + dataRow["trangThai"] + " where id = " + id;
                    XmlToSQL.InsertOrUpDateSQL(sql);
                }
            }


        }
    }
}