using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarmaShop.Models;
using KarmaShop.Repository.GioHang; // Đảm bảo bạn đã có Helper Session ở đây
using System.Linq;

namespace KarmaShop.Controllers
{
    public class PhieuMuaController : Controller
    {
        private readonly QuanLyBanGiayContext db;

        public PhieuMuaController(QuanLyBanGiayContext context)
        {
            db = context;
        }

        // Lấy giỏ hàng từ Session
        private List<ChiTietPhieuMua> GetCartItems()
        {
            return HttpContext.Session.Get<List<ChiTietPhieuMua>>("Cart") ?? new List<ChiTietPhieuMua>();
        }

        // GET: ThanhToan
        public IActionResult ThanhToan()
        {
            var cart = GetCartItems();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ChiTietGioHang", "GioHang");
            }

            // Lấy thông tin khách hàng từ Session Email (giả định bạn đã lưu khi Login)
            string email = HttpContext.Session.GetString("Email") ?? "";
            var kh = db.KhachHangs.FirstOrDefault(x => x.Email == email);

            decimal total = cart.Sum(item => (item.DonGia ?? 0) * (item.SoLuong ?? 0));

            // Truyền dữ liệu sang View
            ViewBag.Cart = cart;
            ViewBag.TongTien = total;
            ViewBag.KhachHang = kh;
            ViewBag.PhuongThucTT = db.PhuongThucThanhToans.ToList();

            // Lấy danh sách Voucher còn hạn
            var today = DateOnly.FromDateTime(DateTime.Now);
            ViewBag.DanhSachVoucher = db.Vouchers
                .Where(v => v.NgayTao <= today && v.NgayHetHan >= today && v.SoLuong > 0)
                .ToList();

            // Khởi tạo model đơn hàng mặc định
            var phieuMua = new PhieuMua
            {
                TenNguoiNhan = kh?.TenKhachHang,
                SoDienThoaiNguoiNhan = kh?.SoDienThoai,
                DiaChiGiaoHang = kh?.DiaChi,
                EmailNguoiNhan = kh?.Email,
                MaKhachHang = kh?.MaKhachHang,
                TongTien = total
            };

            return View(phieuMua);
        }

        [HttpPost]
        public JsonResult KiemTraVoucher(string maVoucher, decimal tongTamTinh)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var voucher = db.Vouchers
                .FirstOrDefault(v => v.MaVoucher == maVoucher
                                  && v.NgayTao <= today
                                  && v.NgayHetHan >= today
                                  && v.SoLuong > 0);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher không tồn tại hoặc hết hạn" });
            }

            decimal giamGia = (voucher.GiamToiDa ?? 0);
            // Lưu ý: Nếu GiamToiDa là số tiền thì dùng luôn, nếu là % thì tính toán lại:
            // decimal giamGia = tongTamTinh * (voucher.GiamToiDa ?? 0) / 100;

            decimal thanhTienMoi = tongTamTinh - giamGia;
            if (thanhTienMoi < 0) thanhTienMoi = 0;

            return Json(new
            {
                success = true,
                giamGia = giamGia,
                thanhTienMoi = thanhTienMoi,
                message = $"Áp dụng thành công! Giảm {giamGia:#,##0}₫"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(PhieuMua model)
        {
            var cart = GetCartItems();
            if (!cart.Any()) return RedirectToAction("Index", "Home");

            // Bắt đầu Transaction
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // 1. Kiểm tra và cập nhật Voucher
                if (!string.IsNullOrEmpty(model.MaVoucher))
                {
                    var v = db.Vouchers.FirstOrDefault(x => x.MaVoucher == model.MaVoucher);
                    if (v != null && v.SoLuong > 0)
                    {
                        v.SoLuong -= 1; // Giảm số lượng voucher
                    }
                }

                // 2. Tạo phiếu mua
                model.NgayDat = DateOnly.FromDateTime(DateTime.Now);
                model.TinhTrang = "Chờ xác nhận";

                db.PhieuMuas.Add(model);
                await db.SaveChangesAsync(); // Lưu để lấy MaPhieuMua

                // 3. Lưu chi tiết và Trừ tồn kho
                foreach (var item in cart)
                {
                    var chiTiet = new ChiTietPhieuMua
                    {
                        MaPhieuMua = model.MaPhieuMua,
                        MaSanPham = item.MaSanPham,
                        MaSize = item.MaSize,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    };
                    db.ChiTietPhieuMuas.Add(chiTiet);

                    // Trừ tồn kho trong bảng SanPhamSize
                    var stock = db.SanPhamSizes.FirstOrDefault(s => s.MaSanPham == item.MaSanPham && s.MaSize == item.MaSize);
                    if (stock == null || stock.SoLuong < item.SoLuong)
                    {
                        throw new Exception($"Sản phẩm mã {item.MaSanPham} không đủ hàng.");
                    }
                    stock.SoLuong -= item.SoLuong;
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                // 4. Xóa giỏ hàng
                HttpContext.Session.Remove("Cart");

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("XacNhan", new { id = model.MaPhieuMua });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Lỗi đặt hàng: " + ex.Message);

                // Load lại dữ liệu cho View nếu lỗi
                ViewBag.Cart = cart;
                ViewBag.PhuongThucTT = db.PhuongThucThanhToans.ToList();
                return View(model);
            }
        }

        public IActionResult XacNhan(int id)
        {
            var donHang = db.PhieuMuas
                .Include(p => p.ChiTietPhieuMuas)
                .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefault(p => p.MaPhieuMua == id);
            return View(donHang);
        }
    }
}