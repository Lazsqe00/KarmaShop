using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarmaShop.Models;

namespace KarmaShop.Controllers
{
    public class TrackingOrderController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public TrackingOrderController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TrackingDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return Content("<div class='alert alert-danger text-center'>Vui lòng nhập Mã đơn hàng!</div>");

            var order = _db.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaDongSanPhamNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaMauNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSizeNavigation)
                .FirstOrDefault(p => p.MaPhieuMua.ToString() == orderId);

            if (order == null)
                return Content("<div class='alert alert-warning text-center'>Không tìm thấy đơn hàng với mã này!</div>");

            return PartialView("_PartialOrderTrackingDetail", order);
        }
    }
}