using CuoiKi.Areas.admin.Convert;
using CuoiKi.Areas.admin.Models;
using CuoiKi.Common;
using System.Web.Mvc;
using System.Xml;

namespace CuoiKi.Areas.admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: admin/Login
        public ActionResult Index()
        {
            ConvertToXml();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NguoiDung user)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~/App_Data/NguoiDung.xml"));
            int result = 0;
            if(user.tenTK == null || user.matKhau == null)
            {
                ModelState.AddModelError("", "Vui lòng nhập tài khoản và mật khẩu");
                return View("Index");
            }
            foreach (XmlElement ele in xmlDoc.GetElementsByTagName("NguoiDung"))
            {
                if (ele.GetAttribute("tenTaiKhoan").ToString().Equals(user.tenTK) && ele.GetAttribute("matKhau").ToString().Equals(Encryptor.EncryptMD5(user.matKhau)))
                {
                    result = 1;
                }
            }
                if (result == 1)
                {
                    ModelState.AddModelError("", "Đăng nhập thành công");
                    Session.Add(Constants.USER_SESSION, user);
                    return RedirectToAction("Index", "Home"); //tạo url chuyển hướng đến trang Home
                }
                else
                {
                    if (result == -1)
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa");
                    }
                    else
                        ModelState.AddModelError("", "Tài khoản hoặc mật khẩu sai");
                }
            return View("Index");
        }

        public void ConvertToXml()
        {
            NguoiDungConverter nguoiDungConverter = new NguoiDungConverter();
            string xml = nguoiDungConverter.toXMl();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(Server.MapPath("~/App_Data/NguoiDung.xml"));
        }
    }
}