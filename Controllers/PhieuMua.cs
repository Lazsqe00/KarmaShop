using KarmaShop.Models;
using KarmaShop.Repositories;
using KarmaShop.Repository.Cart;
using KarmaShop.Repository.PhieuThu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace KarmaShop.Controllers
{
    public class PhieuMuaController : Controller
    {
        private readonly CartInterface _cartRepo;
        private readonly VoucherInterface _voucherRepo;
        private readonly OrderInterface _orderRepo;

        public PhieuMuaController(CartInterface cartRepo, VoucherInterface voucherRepo, OrderInterface orderRepo)
        {
            _cartRepo = cartRepo;
            _voucherRepo = voucherRepo;
            _orderRepo = orderRepo;
        }

        public IActionResult ThanhToan()
        {
            var cart = _cartRepo.GetCartItems();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ChiTietGioHang", "GioHang");
            }

            string email = HttpContext.Session.GetString("Email") ?? "";
            var kh = _orderRepo.GetKhachHangByEmail(email);

            decimal total = cart.Sum(item => (item.DonGia ?? 0) * (item.SoLuong ?? 0));

            
            decimal tongChi = kh?.TongChi ?? 0;
            string hangThanhVien = "Thành Viên";
            decimal phanTramGiamHang = 0;

            if (tongChi >= 8000000) { hangThanhVien = "Kim Cương"; phanTramGiamHang = 0.12m; }
            else if (tongChi >= 5000000) { hangThanhVien = "Bạch Kim"; phanTramGiamHang = 0.10m; }
            else if (tongChi >= 3000000) { hangThanhVien = "Vàng"; phanTramGiamHang = 0.05m; }
            else if (tongChi >= 500000) { hangThanhVien = "Bạc"; phanTramGiamHang = 0m; }

            
            decimal giamGiaHang = 0;
            if (hangThanhVien == "Vàng" && total >= 350000) giamGiaHang = total * phanTramGiamHang;
            else if (hangThanhVien == "Bạch Kim" && total >= 550000) giamGiaHang = total * phanTramGiamHang;
            else if (hangThanhVien == "Kim Cương" && total >= 800000) giamGiaHang = total * phanTramGiamHang;

            ViewBag.Cart = cart;
            ViewBag.TongTien = total;
            ViewBag.KhachHang = kh;
            ViewBag.HangThanhVien = hangThanhVien;
            ViewBag.GiamGiaHang = giamGiaHang;
            ViewBag.PhuongThucTT = _orderRepo.GetPhuongThucThanhToans();
            ViewBag.DanhSachVoucher = _voucherRepo.GetActiveVouchers();

            var phieuMua = new PhieuMua
            {
                TenNguoiNhan = kh?.TenKhachHang,
                SoDienThoaiNguoiNhan = kh?.SoDienThoai,
                DiaChiGiaoHang = kh?.DiaChi,
                EmailNguoiNhan = kh?.Email,
                MaKhachHang = kh?.MaKhachHang,
                TongTien = total - giamGiaHang // Tạm tính sau khi trừ chiết khấu hạng thành viên
            };

            return View(phieuMua);
        }

        [HttpPost]
        public JsonResult TinhToanGiamGia(string maVoucher, decimal tongTamTinh, decimal giamGiaHang)
        {
            decimal giamGiaVoucher = 0;
            string msg = "Không áp dụng voucher.";
            bool status = true;

            if (!string.IsNullOrEmpty(maVoucher))
            {
                var voucher = _voucherRepo.GetVoucherByCode(maVoucher);
                if (voucher == null)
                {
                    return Json(new { success = false, message = "Voucher không tồn tại hoặc hết hạn!" });
                }
                giamGiaVoucher = (voucher.GiamToiDa ?? 0);
                msg = $"Áp dụng thành công! Giảm {giamGiaVoucher:#,##0}₫ từ Voucher.";
            }

            decimal tongThanhToanMoi = Math.Max(0, tongTamTinh - giamGiaHang - giamGiaVoucher);

            return Json(new
            {
                success = status,
                giamHang = giamGiaHang,
                giamVoucher = giamGiaVoucher,
                thanhTienMoi = tongThanhToanMoi,
                message = msg
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(PhieuMua model)
        {
            var cart = _cartRepo.GetCartItems();
            if (!cart.Any()) return RedirectToAction("Index", "Home");

            _orderRepo.BeginTransaction();
            try
            {
                if (!string.IsNullOrEmpty(model.MaVoucher))
                {
                    _voucherRepo.GiamSoLuongVoucher(model.MaVoucher);
                }

                model.NgayDat = DateOnly.FromDateTime(DateTime.Now);
                model.TinhTrang = "Chờ xác nhận";

                _orderRepo.SaveOrder(model, cart);
                _orderRepo.CommitTransaction();

                _cartRepo.ClearCart();

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("XacNhan", new { id = model.MaPhieuMua });
            }
            catch (Exception ex)
            {
                _orderRepo.RollbackTransaction();
                ModelState.AddModelError("", "Lỗi đặt hàng: " + ex.Message);

                ViewBag.Cart = cart;
                ViewBag.PhuongThucTT = _orderRepo.GetPhuongThucThanhToans();
                return View(model);
            }
        }

        public IActionResult XacNhan(int id)
        {
            var donHang = _orderRepo.GetOrder(id);
            return View(donHang);
        }
    }
}