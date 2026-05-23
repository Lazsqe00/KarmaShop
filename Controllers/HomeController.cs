using System.Diagnostics;
using KarmaShop.Models;
using KarmaShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuanLyBanGiayContext _context;

        public HomeController(ILogger<HomeController> logger, QuanLyBanGiayContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hotSanPhams = await _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .Take(10)
                .ToListAsync();

            ViewBag.HotProducts = hotSanPhams.Select(s => new SanPhamHomeViewModel
            {
                sp = s,
                TenSanPhamFull = $"{s.MaDongSanPhamNavigation?.TenDongSanPham} {s.MaMauNavigation?.TenMau}".Trim(),
                GiaHienThi = s.MaDongSanPhamNavigation?.GiaBan
            }).ToList();

            var exclusiveSanPhams = await _context.SanPhams
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .Take(5)
                .ToListAsync();

            ViewBag.ExclusiveProducts = exclusiveSanPhams.Select(s => new SanPhamHomeViewModel
            {
                sp = s,
                TenSanPhamFull = $"{s.MaDongSanPhamNavigation?.TenDongSanPham} {s.MaMauNavigation?.TenMau}".Trim().ToUpper(),
                GiaHienThi = s.MaDongSanPhamNavigation?.GiaBan
            }).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
