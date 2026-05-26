using KarmaShop.Models;
using KarmaShop.Models.ViewModels;
using KarmaShop.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Controllers
{
    public class FavouriteProductsController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public FavouriteProductsController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var favourites = _db.YeuThiches
                .Where(y => y.Email == email)
                .Select(y => new FavouriteProductsItem
                {
                    Id = y.MaSanPham,
                    Madongsp = y.MaSanPhamNavigation.MaDongSanPhamNavigation!.MaDongSanPham,
                    Tensp = y.MaSanPhamNavigation.TenSanPham!,
                    Hinhanh = y.MaSanPhamNavigation.AnhDaiDien!,
                    Gia = (decimal)y.MaSanPhamNavigation.MaDongSanPhamNavigation.GiaBan!,
                    Phantramgiam = null
                })
                .ToList();

            return View(favourites);
        }

        public IActionResult RemoveFavouriteProduct(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var favourite = _db.YeuThiches.FirstOrDefault(y => y.MaSanPham == id && y.Email == email);
            if (favourite != null)
            {
                _db.YeuThiches.Remove(favourite);
                _db.SaveChanges();
                TempData["Message"] = "Đã xóa sản phẩm khỏi danh sách yêu thích.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToFavourite(int maSanPham)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var existed = _db.YeuThiches.Any(y => y.Email == email && y.MaSanPham == maSanPham);
            if (existed)
                return Json(new { success = true, message = "Sản phẩm đã có trong danh sách yêu thích" });

            var yeuThich = new YeuThich
            {
                Email = email,
                MaSanPham = maSanPham,
                NgayThem = DateTime.Now
            };

            _db.YeuThiches.Add(yeuThich);
            _db.SaveChanges();

            return Json(new { success = true, message = "Đã thêm vào yêu thích" });
        }
    }
}