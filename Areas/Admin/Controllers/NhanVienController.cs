using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public NhanVienController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("LoaiTK") == "2";
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        // GET: Admin/NhanVien
        public IActionResult Index(string? tuKhoa)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");

            var danhSach = _db.NhanViens.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string kw = tuKhoa.Trim().ToLower();
                danhSach = danhSach.Where(nv =>
                    (nv.TenNhanVien != null && nv.TenNhanVien.ToLower().Contains(kw)) ||
                    (nv.Email != null && nv.Email.ToLower().Contains(kw)) ||
                    (nv.SoDienThoai != null && nv.SoDienThoai.Contains(kw)) ||
                    nv.MaNhanVien.ToString().Contains(kw));
            }

            ViewBag.TuKhoa = tuKhoa;
            return View(danhSach.ToList());
        }

        // GET: Admin/NhanVien/Details/5
        public IActionResult Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            if (id == null) return BadRequest();
            var nv = _db.NhanViens.Find(id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        // GET: Admin/NhanVien/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            return View();
        }

        // POST: Admin/NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhanVien,TenNhanVien,Email,DiaChi,SoDienThoai,GioiTinh,NgaySinh")] NhanVien nv)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");

            if (ModelState.IsValid)
            {
                var existingAccount = _db.TaiKhoans.FirstOrDefault(t => t.Email == nv.Email);
                if (existingAccount != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng cho tài khoản khác.");
                    return View(nv);
                }

                _db.NhanViens.Add(nv);
                await _db.SaveChangesAsync();

                var taiKhoan = new TaiKhoan
                {
                    Email = nv.Email!,
                    MatKhau = "123",
                    LoaiTaiKhoan = 1
                };
                _db.TaiKhoans.Add(taiKhoan);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(nv);
        }

        // GET: Admin/NhanVien/Edit/5
        public IActionResult Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            if (id == null) return BadRequest();
            var nv = _db.NhanViens.Find(id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        // POST: Admin/NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNhanVien,TenNhanVien,Email,DiaChi,SoDienThoai,GioiTinh,NgaySinh")] NhanVien nv)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            if (id != nv.MaNhanVien) return BadRequest();
            if (ModelState.IsValid)
            {
                _db.Update(nv);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nv);
        }

        // GET: Admin/NhanVien/Delete/5
        public IActionResult Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            if (id == null) return BadRequest();
            var nv = _db.NhanViens.Find(id);
            if (nv == null) return NotFound();
            return View(nv);
        }

        // POST: Admin/NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "HomeAdmin");
            var nv = await _db.NhanViens.FindAsync(id);
            if (nv != null)
            {
                _db.NhanViens.Remove(nv);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
