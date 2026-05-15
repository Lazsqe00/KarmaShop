using KarmaShop.Areas.Admin.ViewModels;
using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public HomeAdminController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Kiểm tra đăng nhập admin
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            if (string.IsNullOrEmpty(loaiTK) || (loaiTK != "1" && loaiTK != "2"))
                return RedirectToAction("Login", "TaiKhoanAdmin");

            int thangHienTai = DateTime.Now.Month;

            // Tổng sản phẩm đã bán trong tháng hiện tại
            int tongSanPhamDaBan = _db.ChiTietPhieuMuas
                .Where(ct => ct.MaPhieuMuaNavigation.NgayDat != null
                          && ((DateOnly)ct.MaPhieuMuaNavigation.NgayDat!).Month == thangHienTai)
                .Sum(ct => (int?)ct.SoLuong) ?? 0;

            // Tổng đơn hàng trong tháng hiện tại
            int tongDonHang = _db.PhieuMuas
                .Count(pm => pm.NgayDat != null
                          && ((DateOnly)pm.NgayDat!).Month == thangHienTai);

            int tongNhanVien = 0;
            if (loaiTK == "2")
                tongNhanVien = _db.NhanViens.Count();

            int tongKhachHang = _db.KhachHangs.Count();

            ViewBag.TongSanPham = tongSanPhamDaBan;
            ViewBag.TongDonHang = tongDonHang;
            ViewBag.TongNhanVien = tongNhanVien;
            ViewBag.TongKhachHang = tongKhachHang;

            // Thống kê doanh thu (thay thế @Html.Action không có trong Core)
            var thongKe = new ThongKeDoanhThu();

            thongKe.SaleByMonths = _db.PhieuMuas
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

            thongKe.SaleByProducts = _db.ChiTietPhieuMuas
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

            ViewBag.ThongKe = thongKe;

            return View();
        }
    }
}
