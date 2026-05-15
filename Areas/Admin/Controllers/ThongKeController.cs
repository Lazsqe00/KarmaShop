using KarmaShop.Areas.Admin.ViewModels;
using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public ThongKeController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        public IActionResult Index()
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");

            var model = new ThongKeDoanhThu();

            model.SaleByMonths = _db.PhieuMuas
                .Where(pm => pm.NgayDat != null && pm.TongTien != null)
                .AsEnumerable()
                .GroupBy(pm => ((DateOnly)pm.NgayDat!).Month)
                .Select(g => new DoanhThuTheoThang
                {
                    Month = g.Key,
                    Sale = g.Sum(x => x.TongTien!.Value)
                })
                .OrderBy(x => x.Month)
                .ToList();

            model.SaleByProducts = _db.ChiTietPhieuMuas
                .Include(ct => ct.MaSanPhamNavigation)
                .Where(ct => ct.SoLuong != null && ct.DonGia != null)
                .AsEnumerable()
                .GroupBy(ct => ct.MaSanPhamNavigation?.TenSanPham ?? "Không rõ")
                .Select(g => new DoanhThuTheoSanPham
                {
                    ProductName = g.Key,
                    Sales = g.Sum(x => x.SoLuong!.Value * x.DonGia!.Value)
                })
                .ToList();

            return View(model);
        }
    }
}
