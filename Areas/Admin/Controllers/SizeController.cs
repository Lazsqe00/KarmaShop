using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public SizeController(QuanLyBanGiayContext db)
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
            return View(_db.Sizes.ToList());
        }

        public IActionResult Details(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Sizes.Find(id);
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
        public async Task<IActionResult> Create([Bind("MaSize,TenSize")] Size item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (ModelState.IsValid)
            {
                _db.Sizes.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Edit(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Sizes.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSize,TenSize")] Size item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id != item.MaSize) return BadRequest();
            if (ModelState.IsValid)
            {
                _db.Update(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Delete(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Sizes.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            var item = await _db.Sizes.FindAsync(id);
            if (item != null)
            {
                _db.Sizes.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
