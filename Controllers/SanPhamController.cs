using KarmaShop.Models;
using KarmaShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QuanLyBanGiayContext _context;

        public SanPhamController(QuanLyBanGiayContext context)
        {
            _context = context;
        }

        public IActionResult SanPhamTheoLoai(int? maLoai, string? maMau, int sortGia = 0,
            decimal minPrice = 0, decimal maxPrice = 10000000, int page = 1, string? searchString = null)
        {
            const int pageSize = 9;

            var query = _context.SanPhams
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string kw = searchString.Trim().ToLower();
                query = query.Where(s =>
                    (s.MaDongSanPhamNavigation != null && s.MaDongSanPhamNavigation.TenDongSanPham != null &&
                     s.MaDongSanPhamNavigation.TenDongSanPham.ToLower().Contains(kw)) ||
                    (s.TenSanPham != null && s.TenSanPham.ToLower().Contains(kw)));
            }

            if (maLoai.HasValue && maLoai.Value > 0)
                query = query.Where(s => s.MaDongSanPhamNavigation != null && s.MaDongSanPhamNavigation.MaLoai == maLoai.Value);

            if (!string.IsNullOrEmpty(maMau) && int.TryParse(maMau, out int maMauInt))
                query = query.Where(s => s.MaMau == maMauInt);

            if (minPrice > 0 || maxPrice < 10000000)
                query = query.Where(s => s.MaDongSanPhamNavigation != null &&
                                         s.MaDongSanPhamNavigation.GiaBan >= minPrice &&
                                         s.MaDongSanPhamNavigation.GiaBan <= maxPrice);

            if (sortGia == 1)
                query = query.OrderBy(s => s.MaDongSanPhamNavigation != null ? s.MaDongSanPhamNavigation.GiaBan : 0);
            else if (sortGia == 2)
                query = query.OrderByDescending(s => s.MaDongSanPhamNavigation != null ? s.MaDongSanPhamNavigation.GiaBan : 0);
            else
                query = query.OrderBy(s => s.MaSanPham);

            int total = query.Count();
            ViewBag.TotalCount  = total;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(total / (double)pageSize);

            var sanPhams = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = sanPhams.Select(s => new SanPhamHomeViewModel
            {
                sp           = s,
                TenSanPhamFull = $"{s.MaDongSanPhamNavigation?.TenDongSanPham} {s.MaMauNavigation?.TenMau}".Trim(),
                GiaHienThi   = s.MaDongSanPhamNavigation?.GiaBan
            }).ToList();

            ViewBag.LoaiList = _context.Loais
                .Select(l => new SelectListItem { Value = l.MaLoai.ToString(), Text = l.TenLoai })
                .ToList();

            ViewBag.MauList = _context.Maus
                .Select(m => new SelectListItem { Value = m.MaMau.ToString(), Text = m.TenMau })
                .ToList();

            ViewBag.TenLoai = maLoai.HasValue
                ? _context.Loais.FirstOrDefault(l => l.MaLoai == maLoai.Value)?.TenLoai ?? "Tất cả"
                : "Tất cả";

            ViewBag.maLoai       = maLoai?.ToString() ?? "";
            ViewBag.maMau1       = maMau ?? "";
            ViewBag.sortGia1     = sortGia;
            ViewBag.minPrice1    = minPrice;
            ViewBag.maxPrice1    = maxPrice;
            ViewBag.SearchString = searchString ?? "";

            return View(model);
        }

        public IActionResult HienThiSanPham(int madongsanpham, int masp)
        {
            var sanPham = _context.SanPhams
                .Include(s => s.MaMauNavigation)
                .FirstOrDefault(s => s.MaSanPham == masp);

            if (sanPham == null)
                return NotFound();

            var dongSanPham = _context.DongSanPhams
                .FirstOrDefault(d => d.MaDongSanPham == madongsanpham);

            if (dongSanPham == null)
                return NotFound();

            var sanPhamSizes = _context.SanPhamSizes
                .Include(sps => sps.MaSizeNavigation)
                .Where(sps => sps.MaSanPham == masp)
                .OrderBy(sps => sps.MaSize)
                .ToList();

            var model = new SanPhamViewModel
            {
                SanPham = sanPham,
                DongSanPham = dongSanPham,
                SanPhams = new List<SanPham>(),
                TenSize = sanPhamSizes.Select(sps => sps.MaSizeNavigation?.TenSize ?? "").ToList(),
                SoLuongTon = sanPhamSizes.Select(sps => sps.SoLuong ?? 0).ToList(),
                MaSize = sanPhamSizes.Select(sps => sps.MaSize).ToList()
            };

            ViewBag.CurrentMaSP = masp;

            var hotSanPhams = _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .Take(10)
                .ToList();

            ViewBag.HotProducts = hotSanPhams.Select(s => new SanPhamHomeViewModel
            {
                sp = s,
                TenSanPhamFull = $"{s.MaDongSanPhamNavigation?.TenDongSanPham} {s.MaMauNavigation?.TenMau}".Trim(),
                GiaHienThi = s.MaDongSanPhamNavigation?.GiaBan
            }).ToList();

            // Sản phẩm cùng dòng (màu khác)
            var relatedProducts = _context.SanPhams
                .Include(s => s.MaMauNavigation)
                .Include(s => s.MaDongSanPhamNavigation)
                .Where(s => s.MaDongSanPham == madongsanpham && s.MaSanPham != masp)
                .OrderBy(s => s.MaSanPham)
                .ToList();

            model.SanPhams = relatedProducts;

            return View(model);
        }
    }
}
