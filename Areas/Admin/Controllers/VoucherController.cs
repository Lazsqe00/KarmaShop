using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public VoucherController(QuanLyBanGiayContext db)
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
            return View(_db.Vouchers.ToList());
        }

        public IActionResult Details(string? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Vouchers.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVoucher,SoLuong,GiamToiDa,NgayTao,NgayHetHan")] Voucher item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (ModelState.IsValid)
            {
                _db.Vouchers.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Edit(string? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Vouchers.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaVoucher,SoLuong,GiamToiDa,NgayTao,NgayHetHan")] Voucher item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id != item.MaVoucher) return BadRequest();
            if (ModelState.IsValid)
            {
                _db.Update(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Delete(string? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Vouchers.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            var item = await _db.Vouchers.FindAsync(id);
            if (item != null)
            {
                _db.Vouchers.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
