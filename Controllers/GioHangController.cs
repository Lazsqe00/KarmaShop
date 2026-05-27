using KarmaShop.Models;
using KarmaShop.Repository.Cart;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KarmaShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ProductInterface _productRepo;
        private readonly CartInterface _cartRepo;

        public GioHangController(ProductInterface productRepo, CartInterface cartRepo)
        {
            _productRepo = productRepo;
            _cartRepo = cartRepo;
        }

        public IActionResult ChiTietGioHang()
        {
            var cart = _cartRepo.GetCartItems();
            return View(cart);
        }

        public async Task<IActionResult> ThemVaoGio(int maSanPham, int maSize)
        {
            var sp = await _productRepo.GetProduct(maSanPham);
            var spSize = await _productRepo.GetProductSize(maSanPham, maSize);

            if (sp == null || spSize == null)
                return RedirectToAction("Index", "Home");

            var cart = _cartRepo.GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                if (item.SoLuong < (spSize.SoLuong ?? 0))
                    item.SoLuong++;
            }
            else
            {
                cart.Add(new ChiTietPhieuMua
                {
                    MaSanPham = sp.MaSanPham,
                    MaSize = maSize,
                    SoLuong = 1,
                    DonGia = sp.MaDongSanPhamNavigation?.GiaBan ?? 0,

                    MaSanPhamNavigation = new SanPham
                    {
                        MaDongSanPham = sp.MaDongSanPham,
                        TenSanPham = sp.TenSanPham,
                        AnhDaiDien = sp.AnhDaiDien
                    },
                    MaSizeNavigation = new Size
                    {
                        TenSize = spSize.MaSizeNavigation?.TenSize
                    }
                });
            }

            _cartRepo.SaveCart(cart);
            return RedirectToAction("ChiTietGioHang");
        }

        public IActionResult IncreaseOne(int maSanPham, int maSize)
        {
            var cart = _cartRepo.GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                var tonKho = _productRepo.GetTonKho(maSanPham, maSize);

                if (item.SoLuong < tonKho)
                {
                    item.SoLuong++;
                }
            }

            _cartRepo.SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        public IActionResult DecreaseOne(int maSanPham, int maSize)
        {
            var cart = _cartRepo.GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                item.SoLuong--;
                if (item.SoLuong <= 0)
                {
                    cart.Remove(item);
                }
            }

            _cartRepo.SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        public IActionResult DeleteItem(int maSanPham, int maSize)
        {
            var cart = _cartRepo.GetCartItems();
            var item = cart.FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize);

            if (item != null)
            {
                cart.Remove(item);
            }

            _cartRepo.SaveCart(cart);
            return PartialView("_PartialChiTietGioHang", cart);
        }

        public IActionResult ClearCart()
        {
            _cartRepo.ClearCart();
            return RedirectToAction("ChiTietGioHang");
        }
    }
}