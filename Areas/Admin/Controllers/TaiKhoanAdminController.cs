using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanAdminController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public TaiKhoanAdminController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        // GET: Admin/TaiKhoanAdmin/Login
        [HttpGet]
        public IActionResult Login()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            if (!string.IsNullOrEmpty(loaiTK) && (loaiTK == "1" || loaiTK == "2"))
                return RedirectToAction("Index", "HomeAdmin");

            return View();
        }

        // POST: Admin/TaiKhoanAdmin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(TaiKhoan model)
        {
            var user = _db.TaiKhoans.FirstOrDefault(x =>
                x.Email == model.Email &&
                x.MatKhau == model.MatKhau &&
                (x.LoaiTaiKhoan == 1 || x.LoaiTaiKhoan == 2));

            if (user != null)
            {
                // Xóa session cũ (khách hàng) rồi set session admin
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("EmailAdmin", user.Email);
                HttpContext.Session.SetString("LoaiTK", user.LoaiTaiKhoan.ToString()!);

                var nv = _db.NhanViens.FirstOrDefault(n => n.Email == user.Email);
                if (nv != null)
                    HttpContext.Session.SetInt32("MaNhanVien", nv.MaNhanVien);

                return RedirectToAction("Index", "HomeAdmin");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng, hoặc tài khoản không có quyền Admin.";
            return View(model);
        }

        // GET: Admin/TaiKhoanAdmin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "TaiKhoanAdmin");
        }
    }
}
