using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarmaShop.Models;
using KarmaShop.Repository.GioHang; // Đảm bảo bạn đã có Helper Session.Get/Set ở đây
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KarmaShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly QuanLyBanGiayContext db;

        public GioHangController(QuanLyBanGiayContext context)
        {
            db = context;
        }

        // Lấy danh sách giỏ hàng từ Session
        private List<ChiTietPhieuMua> GetCartItems()
        {
            // Sử dụng extension method Get đã viết sẵn trong Repository
            return HttpContext.Session.Get<List<ChiTietPhieuMua>>("Cart") ?? new List<ChiTietPhieuMua>();
        }

        // Lưu danh sách giỏ hàng vào Session
        private void SaveCart(List<ChiTietPhieuMua> cart)
        {
            HttpContext.Session.Set("Cart", cart);
        }

        // GET: Xem chi tiết giỏ hàng
        public IActionResult ChiTietGioHang()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ
        public async Task<IActionResult> ThemVaoGio(int maSanPham, int maSize)
        {
            // Load sản phẩm kèm theo bảng Dòng sản phẩm để lấy đơn giá
            var sp = await db.SanPhams
                             .Include(x => x.MaDongSanPhamNavigation)
                             .FirstOrDefaultAsync(x => x.MaSanPham == maSanPham);

            // Load thông tin Size để lấy tên size và kiểm tra tồn kho
            var spSize = await db.SanPhamSizes
                                 .Include(x => x.MaSizeNavigation)
                                 .FirstOrDefaultAsync(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (sp == null || spSize == null)
                return RedirectToAction("Index", "Home");

            var cart = GetCartItems();
            // Tìm sản phẩm trong giỏ dựa trên cặp ID Sản phẩm + ID Size
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                // Kiểm tra nếu số lượng trong giỏ chưa vượt quá tồn kho
                if (item.SoLuong < (spSize.SoLuong ?? 0))
                    item.SoLuong++;
            }
            else
            {
                // Tạo mới item và gán thủ công dữ liệu vào Navigation để View có thể truy cập
                cart.Add(new ChiTietPhieuMua
                {
                    MaSanPham = sp.MaSanPham,
                    MaSize = maSize,
                    SoLuong = 1,
                    DonGia = sp.MaDongSanPhamNavigation?.GiaBan ?? 0,

                    // Gán thông tin hiển thị (Quan trọng vì Session không tự load Navigation)
                    MaSanPhamNavigation = new SanPham
                    {
                        TenSanPham = sp.TenSanPham,
                        AnhDaiDien = sp.AnhDaiDien
                    },
                    MaSizeNavigation = new Size
                    {
                        TenSize = spSize.MaSizeNavigation?.TenSize
                    }
                });
            }

            SaveCart(cart);
            return RedirectToAction("ChiTietGioHang");
        }

        // Tăng số lượng
        public IActionResult IncreaseOne(int maSanPham, int maSize)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                // Truy vấn DB lấy tồn kho thực tế hiện tại
                var tonKho = db.SanPhamSizes
                               .FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize)?.SoLuong ?? 0;

                if (item.SoLuong < tonKho)
                {
                    item.SoLuong++;
                }
            }

            SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        // Giảm số lượng
        public IActionResult DecreaseOne(int maSanPham, int maSize)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                item.SoLuong--;
                if (item.SoLuong <= 0)
                {
                    cart.Remove(item);
                }
            }

            SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        // Xóa sản phẩm khỏi giỏ
        public IActionResult DeleteItem(int maSanPham, int maSize)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        // Xóa sạch giỏ hàng
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("ChiTietGioHang");
        }
    }
}