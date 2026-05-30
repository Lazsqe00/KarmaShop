using KarmaShop.Models;
using KarmaShop.Models.ViewModels;
using KarmaShop.Repository.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        // GET: /TaiKhoan_65130478/DangKy
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Email") != null &&
                HttpContext.Session.GetString("LoaiTK") == "0")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /TaiKhoan_65130478/DangKy
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

        // GET: /Account/DangNhap
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


            var user = userRepo.GetUser(model.Email, model.MatKhau);

            if (user != null)
            {
                // Chặn đăng nhập admin từ trang khách
                if (user.LoaiTaiKhoan == 2)
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                    if (!string.IsNullOrEmpty(backToPage))
                        ViewBag.backToPage = backToPage;
                    return View(model);
                }

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

        public IActionResult ProfileUser()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", new { returnUrl = Url.Action("ProfileUser") });

            var customer = userRepo.GetProfile(email);
            if (customer == null)
                return RedirectToAction("Index", "Home");

            ViewBag.TongChi = userRepo.GetTongChi(email);
            return View(customer);
        }

        public IActionResult ProfilePartial()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Content("<div class='alert alert-danger'>Bạn chưa đăng nhập</div>");

            var model = userRepo.GetProfile(email);
            ViewBag.TongChi = userRepo.GetTongChi(email);

            return PartialView("_PartialProfile", model);
        }

        public IActionResult OrderHistory()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Content("<div class='alert alert-danger'>Bạn chưa đăng nhập</div>");

            try
            {
                var lichSu = userRepo.GetOrderHistory(email);

                return PartialView("_PartialOrderHistory", lichSu);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Content($"<div class='alert alert-danger'>Lỗi tải lịch sử đơn hàng: {ex.Message}</div>");
            }
        }
        public IActionResult OrderDetail(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Content("<div class='alert alert-danger text-center'>Bạn chưa đăng nhập!</div>");

            var order = userRepo.GetOrderDetail(id, email);
            if (order == null)
                return Content("<div class='alert alert-warning text-center'>Không tìm thấy đơn hàng hoặc bạn không có quyền xem.</div>");

            return PartialView("_PartialOrderDetail", order);
        }


        // ====================== ADDRESS BOOK ======================
        public IActionResult AddressBook()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Content("<div class='alert alert-danger'>Bạn chưa đăng nhập</div>");

            var addresses = userRepo.GetAddressBook(email);

            return PartialView("_PartialAddressBook", addresses);
        }


        public JsonResult GetTinhThanh()
        {
            var data = db.TinhThanhs.Select(t => new { value = t.MaTinh, text = t.TenTinh }).ToList();
            return Json(data);
        }

        public JsonResult GetQuanByTinh(string maTinh)
        {
            var data = db.QuanHuyens.Where(q => q.MaTinh == maTinh)
                                   .Select(q => new { value = q.MaQuan, text = q.TenQuan }).ToList();
            return Json(data);
        }

        public JsonResult GetPhuongByQuan(string maQuan)
        {
            var data = db.PhuongXas.Where(p => p.MaQuan == maQuan)
                                   .Select(p => new { value = p.MaPhuong, text = p.TenPhuong }).ToList();
            return Json(data);
        }


        [HttpPost]
        public JsonResult AddAddress(Sodiachi model)
        {
            var email = HttpContext.Session.GetString("Email");
            var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == email);

            if (khachHang == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            model.MaKhachHang = khachHang.MaKhachHang;

            if (model.IsDefault == true)
            {
                db.Sodiachis.Where(s => s.MaKhachHang == model.MaKhachHang).ToList()
                            .ForEach(s => s.IsDefault = false);
            }

            db.Sodiachis.Add(model);
            db.SaveChanges();

            return Json(new { success = true, message = "Thêm địa chỉ thành công" });
        }
    

    [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {             
                var diaChi = await db.Sodiachis.FindAsync(id);
                db.Sodiachis.Remove(diaChi);
                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa địa chỉ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateProfile(KhachHang model)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Chưa đăng nhập" });
    
            bool success = userRepo.UpdateProfile(email, model);
    
            return Json(new { success, message = success ? "Cập nhật thành công" : "Cập nhật thất bại" });
        }
            
        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Chưa đăng nhập!" });
            }
            
            var kh = db.KhachHangs.FirstOrDefault(k => k.Email == email);
            
            if (kh == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng!" });
            }
            
            int userId = kh.MaKhachHang;
            
            var oldDefault = db.Sodiachis
                .Where(x => x.MaKhachHang == userId && x.IsDefault == true)
                .ToList();
            
            foreach (var item in oldDefault)
            {
                item.IsDefault = false;
            }
            
            var address = await db.Sodiachis
                .FirstOrDefaultAsync(x => x.Masodiachi == id && x.MaKhachHang == userId);
            
            if (address == null)
                return Json(new { success = false, message = "Không tìm thấy địa chỉ!" });
            
            address.IsDefault = true;
            
            await db.SaveChangesAsync();
            
            return Json(new { success = true });
        }
    }

}
