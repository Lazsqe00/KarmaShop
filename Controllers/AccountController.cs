using KarmaShop.Models;
using KarmaShop.Models.ViewModels;
using KarmaShop.Repository.User;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyBanGiayContext db;
        UserInterface userRepo;
        public AccountController(QuanLyBanGiayContext db, UserInterface userRepo)
        {
            this.db = db;
            this.userRepo = userRepo;
        }

       
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Email") != null &&
                HttpContext.Session.GetString("LoaiTK") == "0")
                return RedirectToAction("Index", "Home");

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Register(RegViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (userRepo.CheckEmailExists(model.Email))
            {
                ModelState.AddModelError("Email", "Đã tồn tại email");
                return View(model);
            }

            try
            {
                var taiKhoan = new TaiKhoan
                {
                    Email = model.Email,
                    MatKhau = model.MatKhau,
                    LoaiTaiKhoan = 0
                };

                var khachHang = new KhachHang
                {
                    TenKhachHang = model.TenKhachHang,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai ?? "",
                    GioiTinh = model.GioiTinh ?? "Nam",   
                    NgaySinh = model.NgaySinh,
                    TongChi = 0,
                    DiaChi = ""
                };

                userRepo.Register(taiKhoan, khachHang);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi đăng ký: " + ex.Message);
                return View(model);
            }
        }

        
        public IActionResult Login(string backToPage = "")
        {
            if (HttpContext.Session.GetString("Email") != null &&
                HttpContext.Session.GetString("LoaiTK") == "0")
                return RedirectToAction("Index", "Home");

           


            if (!string.IsNullOrEmpty(backToPage))
                ViewBag.backToPage = backToPage;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string backToPage = "")
        {
            if (!ModelState.IsValid)
                return View(model);

            
            var user = db.TaiKhoans.FirstOrDefault(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

            if (user != null)
            {
                
                if (user.LoaiTaiKhoan == 1) 
                {
                    var nv = db.NhanViens.FirstOrDefault(n => n.Email == user.Email);
                    string tenNV = nv?.TenNhanVien ?? "Nhân viên";

                    SetUserSession(user.Email, user.LoaiTaiKhoan, tenNV);

                    
                    return RedirectToAction("Index", "ThuNgan", new { area = "NhanVien" });
                }
                else if (user.LoaiTaiKhoan == 0) 
                {
                    var kh = db.KhachHangs.FirstOrDefault(k => k.Email == user.Email);
                    string tenKH = kh?.TenKhachHang ?? user.Email;

                    SetUserSession(user.Email, user.LoaiTaiKhoan, tenKH);

                    if (!string.IsNullOrEmpty(backToPage))
                        return Redirect(backToPage);

                    return RedirectToAction("Index", "Home");
                }
            }

           
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            if (!string.IsNullOrEmpty(backToPage))
                ViewBag.backToPage = backToPage;

            return View(model);
        }

       
        private void SetUserSession(string email, int? loaiTaiKhoan, string? tenKhachHang)
        {
            HttpContext.Session.SetString("Email", email);
            HttpContext.Session.SetString("LoaiTK", loaiTaiKhoan.ToString()!);

            string displayName = email;

            if (loaiTaiKhoan == 1) 
            {
                var nhanVien = db.NhanViens.FirstOrDefault(nv => nv.Email == email);
                if (nhanVien != null) displayName = nhanVien.TenNhanVien;
            }

            if (!string.IsNullOrEmpty(tenKhachHang))
            {
                string[] parts = tenKhachHang.Trim().Split(' ');
                HttpContext.Session.SetString("TenKhachHang", parts[^1]); 
            }
            else
            {
                HttpContext.Session.SetString("TenKhachHang", email);
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult HoSoNguoiDung()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("HoSoNguoiDung") });

            var customer = userRepo.GetProfile(email);
            if (customer == null)
                return RedirectToAction("Index", "Home");

            decimal tongChi = userRepo.GetTongChi(email);
            ViewBag.TongChi = tongChi;

            return View(customer);
        }

        [HttpPost]
        public IActionResult CapNhatHoSo(KhachHang model)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Chưa đăng nhập" });

            bool success = userRepo.UpdateProfile(email, model);

            return Json(new { success, message = success ? "Cập nhật thành công" : "Cập nhật thất bại" });
        }

        public IActionResult LichSuDatHang()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("DangNhap");

            var lichSu = db.PhieuMuas
                            .Where(p => p.MaKhachHangNavigation!.Email == email) 
                            .OrderByDescending(p => p.NgayDat)
                            .ToList();

            return PartialView("_PartialLichSuDatHang", lichSu);
        }
    }
}
