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
    public class SanPhamController : Controller
    {
        // GET: admin/SanPham
        public ActionResult Index(string SearchString)
        {
            if (!Directory.Exists(Server.MapPath("~/App_Data/SanPham.xml")))
            {
                ConvertSanPhamToXml();
                ConvertDanhMucToXml();
            }
            

            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/SanPham.xml"));

            XmlDocument xmlDM = new XmlDocument();
            xmlDM.Load(Server.MapPath("~/App_Data/DanhMuc.xml"));

            List<SanPham> List = new List<SanPham>();
            foreach (XmlElement ele in xml.GetElementsByTagName("SanPham"))
            {
                if (ele.GetAttribute("tinhTrang") == "1")
                {
                    SanPham sanPham = new SanPham();
                    foreach (XmlElement e in xmlDM.GetElementsByTagName("DanhMuc"))
                    {
                        if(e.GetAttribute("id")== ele.GetAttribute("maDM"))
                        {
                            sanPham.tenDanhMuc = e.GetAttribute("tenDanhMuc");
                        }
                    }
                        
                    sanPham.id = ele.GetAttribute("id");
                    sanPham.tenSanPham = ele.GetAttribute("tenSanPham");
                    sanPham.soLuong = int.Parse(ele.GetAttribute("soLuong"));
                    sanPham.gia = int.Parse(ele.GetAttribute("gia"));
                    sanPham.maDM = ele.GetAttribute("maDM");
                    sanPham.anh = ele.GetAttribute("hinhAnh");

                    List.Add(sanPham);
                }
                
            }
            List<SanPham> ListFind = new List<SanPham>();
            if (SearchString != null)
            {
                foreach (SanPham sp in List)
                {
                    if (sp.tenSanPham.ToLower().Contains(SearchString.ToLower()))
                    {
                        ListFind.Add(sp);
                    }
                }
                return View(ListFind);
            }
            
            return View(List);
        }

        public void ConvertSanPhamToXml()
        {
            SanPhamConverter quyenConverter = new SanPhamConverter();
            String xml = quyenConverter.toXMl();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(Server.MapPath("~/App_Data/SanPham.xml"));

        }
        public void setViewBag(string i = null)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/App_Data/DanhMuc.xml"));
            var listDM = xdoc.GetElementsByTagName("DanhMuc");
            List<DanhMuc> List = new List<DanhMuc>();
            foreach (XmlElement ele in listDM)
            {
                DanhMuc danhMuc = new DanhMuc();
                danhMuc.id = ele.GetAttribute("id").ToString();
                danhMuc.tenDanhMuc = ele.GetAttribute("tenDanhMuc").ToString();
                List.Add(danhMuc);
            }
            ViewBag.danhMuc = new SelectList(List, "id", "tenDanhMuc", i);
        }
        public ActionResult Create()
        {
            setViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(SanPham sp, HttpPostedFileBase file)
        {
            setViewBag();
            //if (ModelState.IsValid)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(Server.MapPath("~/App_Data/SanPham.xml"));
                XmlElement ele = xdoc.CreateElement("SanPham");
                ele.SetAttribute("id", "");
                ele.SetAttribute("tenSanPham", sp.tenSanPham);
                ele.SetAttribute("maDM", sp.maDM);
                ele.SetAttribute("gia", "" + sp.gia);
                ele.SetAttribute("soLuong", "" + sp.soLuong);
                ele.SetAttribute("tinhTrang", "1");

                string returnImagePath = string.Empty;
                string fileName, fileExtension, imaageSavePath, name = "";
                if (file != null && file.ContentLength > 0)
                {
                    
                    fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    fileExtension = Path.GetExtension(file.FileName);

                    imaageSavePath = Server.MapPath("~/uploadedImages/") + fileName + fileExtension;
                    //Save file
                    file.SaveAs(imaageSavePath);
                    name = fileName + fileExtension;
                    
                }
                ele.SetAttribute("hinhAnh", name);
                xdoc.DocumentElement.AppendChild(ele);
                xdoc.Save(Server.MapPath("~/App_Data/SanPham.xml"));
                ToSQL();
                return RedirectToAction("Index", "SanPham");
            }
            return View();
        }

        public void ToSQL()
        {
            DataTable dt = new DataTable();
            string filepath = Server.MapPath("~/App_Data/SanPham.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(filepath);
            DataView dv = new DataView(ds.Tables[0]);
            dt = dv.Table;
            string sql;
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["id"] == "")
                {
                    sql = "insert into SanPham(tenSanPham, maDM, soLuong, gia, tinhTrang, hinhAnh) values ('" + dataRow["tenSanPham"] + "', "+ dataRow["maDM"] +","+ dataRow["soLuong"] + ","+ dataRow["gia"] + "," + dataRow["tinhTrang"]+ ",'" + dataRow["hinhAnh"] + "')";
                    XmlToSQL.InsertOrUpDateSQL(sql);
                }
            }
        }

        public ActionResult Edit(string id)
        {
            //chuyển size sang xml;
            ConvertSizeToXml();
            //lấy size của sp
            XmlDocument xdocSize = new XmlDocument();
            xdocSize.Load(Server.MapPath("~/App_Data/Size.xml"));
            List<Size> ListSize = new List<Size>();
            foreach (XmlElement ele in xdocSize.GetElementsByTagName("Size"))
            {
                if(ele.GetAttribute("maSP") == id)
                {
                    Size size = new Size();
                    size.id = int.Parse(ele.GetAttribute("id"));
                    size.idSP = int.Parse(ele.GetAttribute("maSP"));
                    size.size = int.Parse(ele.GetAttribute("size"));
                    size.tinhTrang = int.Parse(ele.GetAttribute("tinhTrang"));
                    ListSize.Add(size);
                }
                    
                

            }
            setViewBag();
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/App_Data/SanPham.xml"));
            SanPham model = new SanPham();
            foreach(XmlElement ele in xdoc.GetElementsByTagName("SanPham")){
                if (ele.GetAttribute("id") == id)
                {
                    model.id = id;
                    model.maDM = ele.GetAttribute("maDM");
                    model.tenSanPham = ele.GetAttribute("tenSanPham");
                    model.gia = int.Parse(ele.GetAttribute("gia"));
                    model.soLuong = int.Parse(ele.GetAttribute("soLuong"));

                }
            }
            model.listSize = new List<int>();
            foreach(Size size in ListSize)
            {
                if(size.tinhTrang == 1)
                    model.listSize.Add(size.size);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SanPham sp)
        {
            setViewBag();
            if (ModelState.IsValid)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(Server.MapPath("~/App_Data/SanPham.xml"));
                foreach (XmlElement ele in xdoc.GetElementsByTagName("SanPham"))
                {
                    if (ele.GetAttribute("id") == sp.id)
                    {
                        ele.SetAttribute("tenSanPham", sp.tenSanPham);
                        ele.SetAttribute("maDM", sp.maDM);
                        ele.SetAttribute("gia", sp.gia + "");
                        ele.SetAttribute("soLuong", sp.soLuong + "");
                        xdoc.Save(Server.MapPath("~/App_Data/SanPham.xml"));
                        UpdateToSQL(sp.id);
                        break;
                    }
                }
                var listSize = sp.listSize;
                insertSize(int.Parse(sp.id), listSize);
                return RedirectToAction("Index", "SanPham");
            }
            return View();
        }

        public void UpdateToSQL(string id)
        {
            DataTable dt = new DataTable();
            string filepath = Server.MapPath("~/App_Data/SanPham.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(filepath);
            DataView dv = new DataView(ds.Tables[0]);
            dt = dv.Table;
            string sql;
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["id"].ToString() == id)
                {
                    sql = "update SanPham set tenSanPham = '"+ dataRow["tenSanPham"] + "', maDM = "+ dataRow["maDM"] + ", soLuong = "+ dataRow["soLuong"] + ", gia = "+ dataRow["gia"] + ", tinhTrang = " + dataRow["tinhTrang"] + "where id = " + id;
                    XmlToSQL.InsertOrUpDateSQL(sql);
                }
            }
        }

        

        public void ConvertSizeToXml()
        {
            SizeConverter sizeConverter = new SizeConverter();
            String xml = sizeConverter.toXMl();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(Server.MapPath("~/App_Data/Size.xml"));

        }
        //thêm size
        public void insertSize(int maSP, List<int> listSize)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/App_Data/Size.xml"));
            //lấy size đã có
            List<Size> sizes = new List<Size>();
            string strSize = "";
            foreach (XmlElement ele in xdoc.GetElementsByTagName("Size"))
            {
                if(ele.GetAttribute("maSP") == "" + maSP)
                {
                    Size size = new Size();
                    size.id = int.Parse(ele.GetAttribute("id"));
                    size.idSP = int.Parse(ele.GetAttribute("maSP"));
                    size.size = int.Parse(ele.GetAttribute("size"));
                    size.tinhTrang = int.Parse(ele.GetAttribute("tinhTrang"));
                    sizes.Add(size);
                    strSize += " " + size.size; 
                }
            }

            foreach (var item in listSize)
            {
                if (!strSize.Contains(item+""))
                {
                    XmlElement ele = xdoc.CreateElement("Size");
                    ele.SetAttribute("id", "");
                    ele.SetAttribute("maSP", maSP + "");
                    ele.SetAttribute("size", item + "");
                    ele.SetAttribute("tinhTrang", "1");
                    xdoc.DocumentElement.AppendChild(ele);
                }
               
            }

            //bỏ cọn size
            foreach(var item in sizes)
            {
                if (!listSize.Contains(item.size))
                {
                    foreach (XmlElement ele in xdoc.GetElementsByTagName("Size"))
                    {
                        if (ele.GetAttribute("id") == item.id+"")
                        {
                            ele.SetAttribute("tinhTrang", "0");
                            xdoc.Save(Server.MapPath("~/App_Data/Size.xml"));
                            UpdateSizeToSQL(item.id+"");
                            break;
                        }
                    }
                }
            }
            xdoc.Save(Server.MapPath("~/App_Data/Size.xml"));
            sizeToSQL();
        }

        public void sizeToSQL()
        {
            DataTable dt = new DataTable();
            string filepath = Server.MapPath("~/App_Data/Size.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(filepath);
            DataView dv = new DataView(ds.Tables[0]);
            dt = dv.Table;
            string sql;
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["id"].ToString() == "")
                {
                    sql = "insert into Size(maSP, size, tinhTrang) values (" + dataRow["maSP"] + ", " + dataRow["size"]+ ", " + dataRow["tinhTrang"] + ")";
                    XmlToSQL.InsertOrUpDateSQL(sql);
                }
            }
        }

        public void UpdateSizeToSQL(string id)
        {
            DataTable dt = new DataTable();
            string filepath = Server.MapPath("~/App_Data/Size.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(filepath);
            DataView dv = new DataView(ds.Tables[0]);
            dt = dv.Table;
            string sql;
            foreach (DataRow dataRow in dt.Rows)
            {
                if (dataRow["id"].ToString() == id)
                {
                    sql = "update Size set tinhTrang = " + dataRow["tinhTrang"] + "where id = " + id;
                    XmlToSQL.InsertOrUpDateSQL(sql);
                }
            }
        }

        public void ConvertDanhMucToXml()
        {
            DanhMucConverter quyenConverter = new DanhMucConverter();
            String xml = quyenConverter.toXMl();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(Server.MapPath("~/App_Data/DanhMuc.xml"));

        }

        [HttpPost]
        public JsonResult TimKiemAjax(string keyword)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath("~/App_Data/SanPham.xml"));

            XmlDocument xmlDM = new XmlDocument();
            xmlDM.Load(Server.MapPath("~/App_Data/DanhMuc.xml"));

            List<SanPham> List = new List<SanPham>();
            foreach (XmlElement ele in xml.GetElementsByTagName("SanPham"))
            {
                if (ele.GetAttribute("tinhTrang") == "1")
                {
                    SanPham sanPham = new SanPham();
                    foreach (XmlElement e in xmlDM.GetElementsByTagName("DanhMuc"))
                    {
                        if (e.GetAttribute("id") == ele.GetAttribute("maDM"))
                        {
                            sanPham.tenDanhMuc = e.GetAttribute("tenDanhMuc");
                        }
                    }

                    sanPham.id = ele.GetAttribute("id");
                    sanPham.tenSanPham = ele.GetAttribute("tenSanPham");
                    sanPham.soLuong = int.Parse(ele.GetAttribute("soLuong"));
                    sanPham.gia = int.Parse(ele.GetAttribute("gia"));
                    sanPham.maDM = ele.GetAttribute("maDM");
                    sanPham.anh = ele.GetAttribute("hinhAnh");

                    List.Add(sanPham);
                }

            }
            List<SanPham> ListFind = new List<SanPham>();
            if (keyword != null)
            {
                foreach (SanPham sp in List)
                {
                    if (sp.tenSanPham.ToLower().Contains(keyword.ToLower()))
                    {
                        ListFind.Add(sp);
                    }
                }
            }
            return Json(new
            {
                list = ListFind,
                message = ListFind.Count() > 0 ? "" : "Không tìm thấy sản phẩm nào!"
            });
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/App_Data/SanPham.xml"));
            foreach (XmlElement ele in xdoc.GetElementsByTagName("SanPham"))
            {
                if (ele.GetAttribute("id") == id)
                {
                    ele.SetAttribute("tinhTrang", "0");
                    xdoc.Save(Server.MapPath("~/App_Data/SanPham.xml"));
                    UpdateToSQL(id);
                    return Json(new
                    {
                        error = false,
                        message = "Xóa thành công!"
                    });
                }
            }

            return Json(new
            {
                error = true,
            });
        }
    }
}