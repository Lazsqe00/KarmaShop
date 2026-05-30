using KarmaShop.Models;
using KarmaShop.Repositories;
using KarmaShop.Repository.Cart;
using KarmaShop.Repository.PhieuThu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace KarmaShop.Controllers
{
    public class PhieuMuaController : Controller
    {
        private readonly CartInterface _cartRepo;
        private readonly VoucherInterface _voucherRepo;
        private readonly OrderInterface _orderRepo;
        private readonly QuanLyBanGiayContext _db;
        public PhieuMuaController(CartInterface cartRepo, VoucherInterface voucherRepo, OrderInterface orderRepo, QuanLyBanGiayContext db)
        {
            _cartRepo = cartRepo;
            _voucherRepo = voucherRepo;
            _orderRepo = orderRepo;
            _db = db;
        }

        public IActionResult ThanhToan()
        {
            var cart = _cartRepo.GetCartItems();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ChiTietGioHang", "GioHang");
            }

            foreach (var item in cart)
            {
                var sp = _db.SanPhams
                    .Include(x => x.MaDongSanPhamNavigation)
                    .FirstOrDefault(x => x.MaSanPham == item.MaSanPham);

                item.MaSanPhamNavigation = sp;
            }

            string email = HttpContext.Session.GetString("Email") ?? "";
            var kh = _orderRepo.GetKhachHangByEmail(email);

            if (kh == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

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

            var defaultAddress = _orderRepo.GetDefaultAddress(kh.MaKhachHang);

            var phieuMua = new PhieuMua
            {
                MaKhachHang = kh.MaKhachHang,
                TongTien = total - giamGiaHang,
                NgayDat = DateOnly.FromDateTime(DateTime.Now),
                TinhTrang = "Chờ xác nhận",

                TenNguoiNhan = defaultAddress?.Tennguoinhan ?? kh.TenKhachHang,
                SoDienThoaiNguoiNhan = defaultAddress?.Sdtnguoinhan ?? kh.SoDienThoai,
                DiaChiGiaoHang = defaultAddress?.Diachi ?? kh.DiaChi,
                EmailNguoiNhan = kh.Email
            };

            ViewBag.Cart = cart;
            ViewBag.TongTien = total;
            ViewBag.KhachHang = kh;
            ViewBag.HangThanhVien = hangThanhVien;
            ViewBag.GiamGiaHang = giamGiaHang;
            ViewBag.PhuongThucTT = _orderRepo.GetPhuongThucThanhToans();
            ViewBag.DefaultAddress = defaultAddress;
            ViewBag.DanhSachVoucher = _voucherRepo.GetActiveVouchers();

            return View(phieuMua);
        }

        [HttpPost]
        public JsonResult TinhToanGiamGia(string maVoucher, decimal tongTamTinh, decimal giamGiaHang)
        {
            decimal giamGiaVoucher = 0;
            string msg = "Không áp dụng voucher.";
            bool status = true;

            if (!string.IsNullOrWhiteSpace(maVoucher))
            {
                var voucher = _voucherRepo.GetVoucherByCode(maVoucher);
                if (voucher == null)
                {
                    return Json(new { success = false, message = "Voucher không tồn tại hoặc hết hạn!" });
                }

                if (string.Equals(voucher.LoaiGiamGia, "Phần trăm", StringComparison.OrdinalIgnoreCase))
                {
                    giamGiaVoucher = Math.Round(tongTamTinh * (voucher.GiamToiDa / 100m), 0);
                    msg = $"Áp dụng thành công! Giảm {voucher.GiamToiDa:#,##0.##}% ({giamGiaVoucher:#,##0}₫) từ Voucher.";
                }
                else
                {
                    giamGiaVoucher = voucher.GiamToiDa;
                    msg = $"Áp dụng thành công! Giảm {giamGiaVoucher:#,##0}₫ từ Voucher.";
                }
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
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán";
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartRepo.GetCartItems();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _orderRepo.BeginTransaction();


                var khachHang = _db.KhachHangs.FirstOrDefault(k => k.Email == email);
                if (khachHang == null)
                {
                    _orderRepo.RollbackTransaction();
                    return RedirectToAction("Login", "Account");
                }

                model.MaKhachHang = khachHang.MaKhachHang;
                model.NgayDat = DateOnly.FromDateTime(DateTime.Now);

                var pttt = model.MaPttt.HasValue
                    ? _orderRepo.GetPhuongThucThanhToanById(model.MaPttt.Value)
                    : null;

                if (pttt != null && pttt.TenPttt.Contains("Chuyển khoản", StringComparison.OrdinalIgnoreCase))
                {
                    
                    model.TinhTrang = "Đã thanh toán";
                }
                else
                {
                    
                    model.TinhTrang = "Chờ thanh toán";
                }

                // Xử lý voucher
                if (!string.IsNullOrEmpty(model.MaVoucher))
                {
                    var voucherResult = _voucherRepo.GiamSoLuongVoucher(model.MaVoucher);

                    if (!voucherResult)
                    {
                        _orderRepo.RollbackTransaction();
                        TempData["Error"] = "Mã voucher không hợp lệ hoặc đã hết lượt sử dụng!";
                        return RedirectToAction("ThanhToan");
                    }
                }

                _orderRepo.SaveOrder(model, cart);

                _orderRepo.CommitTransaction();
                _cartRepo.ClearCart();

                TempData["SuccessMessage"] = "Đặt hàng thành công!";

                if (pttt != null && pttt.TenPttt.Contains("Chuyển khoản", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("XacNhanChuyenKhoan", new { maPhieuMua = model.MaPhieuMua });
                }

                return RedirectToAction("XacNhan", new { id = model.MaPhieuMua });
            }
            catch (Exception ex)
            {
                _orderRepo.RollbackTransaction();
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ThanhToan");
            }
        }

        public IActionResult XacNhanChuyenKhoan(int maPhieuMua)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var donHang = _db.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .FirstOrDefault(p => p.MaPhieuMua == maPhieuMua &&
                                     p.MaKhachHangNavigation.Email == email);

            if (donHang == null)
                return RedirectToAction("Index", "Home");

            ViewBag.MaDonHang = donHang.MaPhieuMua;
            ViewBag.TongTien = donHang.TongTien;
            return View(donHang);
        }

        public IActionResult XacNhan(int id)
        {
            var donHang = _orderRepo.GetOrder(id);
            return View(donHang);
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SePayWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                Console.WriteLine("=== SEPAY WEBHOOK RECEIVED ===");
                Console.WriteLine(body);

                if (string.IsNullOrWhiteSpace(body))
                    return Ok(new { success = true });

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = System.Text.Json.JsonSerializer.Deserialize<Models.ViewModels.SePayWebhookModel>(body, options);

                if (data == null)
                    return Ok(new { success = true });

                Console.WriteLine($"Content nhận được: '{data.content}'");

                string content = data.content?.Trim().ToUpper() ?? "";

                Console.WriteLine($"Content nhận được: '{data.content}'");
                if (content.Contains("KMHD"))
                {
                    int startIndex = content.IndexOf("KMHD") + 4;  
                    string idString = new string(content.Substring(startIndex).TakeWhile(char.IsDigit).ToArray());

                    Console.WriteLine($"Parsed Order ID: '{idString}'");

                    if (int.TryParse(idString, out int maPhieuMua) && maPhieuMua > 0)
                    {
                        Console.WriteLine($"Đang xử lý đơn hàng #{maPhieuMua}");

                        var donHang = await _db.PhieuMuas
                            .FirstOrDefaultAsync(p => p.MaPhieuMua == maPhieuMua);

                        if (donHang != null)
                        {
                            Console.WriteLine($"Tìm thấy đơn hàng. Trạng thái hiện tại: '{donHang.TinhTrang}'");

                            if (donHang.TinhTrang?.Trim() == "Chờ thanh toán")
                            {
                                donHang.TinhTrang = "Chờ lấy hàng";
                                await _db.SaveChangesAsync();
                                Console.WriteLine($"✅ CẬP NHẬT THÀNH CÔNG ĐƠN HÀNG #{maPhieuMua}");
                            }
                            else
                            {
                                Console.WriteLine("⚠️ Đơn hàng không ở trạng thái Chờ thanh toán");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"❌ Không tìm thấy đơn hàng #{maPhieuMua} trong database");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Không parse được số đơn hàng");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Không tìm thấy KMHD trong nội dung chuyển khoản");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== WEBHOOK ERROR ===");
                Console.WriteLine(ex.Message);
                return Ok(new { success = true });
            }
        }

        [HttpGet]
        public JsonResult KiemTraTrangThaiDonHang(int id)
        {
            var donHang = _db.PhieuMuas.FirstOrDefault(p => p.MaPhieuMua == id);

            return Json(new
            {
                isPaid = donHang?.TinhTrang?.Trim() == "Chờ lấy hàng",
                status = donHang?.TinhTrang ?? "Không tìm thấy"
            });
        }

        public async Task<IActionResult> GoiYVoucher(decimal tongTien = 0)
        {
      
            string email = HttpContext.Session.GetString("Email") ?? "";

            if (string.IsNullOrEmpty(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập để xem voucher"
                });
            }

            var khachHang = await _db.KhachHangs
                .FirstOrDefaultAsync(k => k.Email == email);

            if (khachHang == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy khách hàng"
                });
            }

            // ==================== XÉT HẠNG ====================

            decimal tongChi = khachHang.TongChi ?? 0;

            string hangThanhVien = "Thường";
            int capHang = 1;

            if (tongChi >= 20000000)
            {
                hangThanhVien = "Kim Cương";
                capHang = 4;
            }
            else if (tongChi >= 10000000)
            {
                hangThanhVien = "Vàng";
                capHang = 3;
            }
            else if (tongChi >= 5000000)
            {
                hangThanhVien = "Bạc";
                capHang = 2;
            }

            var today = DateOnly.FromDateTime(DateTime.Now);

            // ==================== LỌC VOUCHER ====================

            var vouchers = await _db.Vouchers
                .Where(v => v.SoLuong > 0)

                .Where(v =>
                    v.NgayTao <= today &&
                    v.NgayHetHan >= today)

                .Where(v => v.GiaTriToiThieu <= tongTien)

                .Where(v =>
                    (v.HangApDung == "Thường" && capHang >= 1) ||
                    (v.HangApDung == "Bạc" && capHang >= 2) ||
                    (v.HangApDung == "Vàng" && capHang >= 3) ||
                    (v.HangApDung == "Kim Cương" && capHang >= 4) ||
                    (v.HangApDung == "Tất cả")
                )
                .ToListAsync();

            //LOGIC SẮP XẾP CỦA T
            vouchers = vouchers
                .OrderByDescending(v =>
                {
                    if (v.LoaiGiamGia == "Phần trăm")
                    {
                        return (v.GiamToiDa / 100m) * tongTien;
                    }
                    else
                    {
                        return v.GiamToiDa;
                    }
                })
                .Take(8)
                .ToList();

            // DEBUG
            Console.WriteLine("Số voucher tìm được: " + vouchers.Count);

            var result = vouchers.Select(v => new
            {
                maVoucher = v.MaVoucher,

                loaiGiamGia = v.LoaiGiamGia,

                giamGiaText = v.LoaiGiamGia == "Phần trăm"
                    ? $"-{v.GiamToiDa}%"
                    : $"-{v.GiamToiDa.ToString("#,##0")}₫",

                hangApDung = v.HangApDung,

                dieuKien = $"Đơn tối thiểu {v.GiaTriToiThieu:#,##0}₫",

                hanSuDung = v.NgayHetHan?.ToString("dd/MM/yyyy")
            }).ToList();

            return Json(new
            {
                success = true,
                hangThanhVien = hangThanhVien,
                tongChi = tongChi,
                vouchers = result
            });
        }           
    }
}