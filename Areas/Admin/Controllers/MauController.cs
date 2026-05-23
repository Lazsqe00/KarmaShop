using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MauController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public MauController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        public IActionResult Index(int page = 1)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            const int pageSize = 10;
            var query = _db.Maus.OrderBy(m => m.MaMau);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(query.Count() / (double)pageSize);
            return View(query.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public IActionResult Details(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Maus.Find(id);
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
        public async Task<IActionResult> Create([Bind("MaMau,TenMau")] Mau item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (ModelState.IsValid)
            {
                _db.Maus.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Edit(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.Maus.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMau,TenMau")] Mau item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id != item.MaMau) return BadRequest();
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
            var item = _db.Maus.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            var item = await _db.Maus.FindAsync(id);
            if (item != null)
            {
                _db.Maus.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
