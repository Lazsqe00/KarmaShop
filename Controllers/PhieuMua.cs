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

            ViewBag.Cart = cart;
            ViewBag.TongTien = total;
            ViewBag.KhachHang = kh;
            ViewBag.PhuongThucTT = _orderRepo.GetPhuongThucThanhToans();
            ViewBag.DanhSachVoucher = _voucherRepo.GetActiveVouchers();

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
            var voucher = _voucherRepo.GetVoucherByCode(maVoucher);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher không tồn tại hoặc hết hạn" });
            }

            decimal giamGia = (voucher.GiamToiDa ?? 0);
            decimal thanhTienMoi = Math.Max(0, tongTamTinh - giamGia);

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