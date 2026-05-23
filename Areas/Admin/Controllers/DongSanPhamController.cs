using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DongSanPhamController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public DongSanPhamController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        // GET: Admin/DongSanPham
        public IActionResult Index(int page = 1)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            const int pageSize = 10;
            var query = _db.DongSanPhams.Include(d => d.MaLoaiNavigation).OrderBy(d => d.MaDongSanPham);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(query.Count() / (double)pageSize);
            var list = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(list);
        }

        // GET: Admin/DongSanPham/Details/5
        public IActionResult Details(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.DongSanPhams.Include(d => d.MaLoaiNavigation).FirstOrDefault(d => d.MaDongSanPham == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: Admin/DongSanPham/Create
        public IActionResult Create()
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            ViewBag.MaLoai = new SelectList(_db.Loais, "MaLoai", "TenLoai");
            return View();
        }

        // POST: Admin/DongSanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDongSanPham,MaLoai,TenDongSanPham,GiaBan,MoTa")] DongSanPham item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (ModelState.IsValid)
            {
                _db.DongSanPhams.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLoai = new SelectList(_db.Loais, "MaLoai", "TenLoai", item.MaLoai);
            return View(item);
        }

        // GET: Admin/DongSanPham/Edit/5
        public IActionResult Edit(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.DongSanPhams.Find(id);
            if (item == null) return NotFound();
            ViewBag.MaLoai = new SelectList(_db.Loais, "MaLoai", "TenLoai", item.MaLoai);
            return View(item);
        }

        // POST: Admin/DongSanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDongSanPham,MaLoai,TenDongSanPham,GiaBan,MoTa")] DongSanPham item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id != item.MaDongSanPham) return BadRequest();
            if (ModelState.IsValid)
            {
                _db.Update(item);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLoai = new SelectList(_db.Loais, "MaLoai", "TenLoai", item.MaLoai);
            return View(item);
        }

        // GET: Admin/DongSanPham/Delete/5
        public IActionResult Delete(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();
            var item = _db.DongSanPhams.Include(d => d.MaLoaiNavigation).FirstOrDefault(d => d.MaDongSanPham == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Admin/DongSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            var item = await _db.DongSanPhams.FindAsync(id);
            if (item != null)
            {
                _db.DongSanPhams.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
