using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamAdminController : Controller
    {
        private readonly QuanLyBanGiayContext _db;
        private readonly IWebHostEnvironment _env;

        public SanPhamAdminController(QuanLyBanGiayContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        // GET: Admin/SanPhamAdmin
        public IActionResult Index()
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            var sanPhams = _db.SanPhams
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .ToList();
            return View(sanPhams);
        }

        // GET: Admin/SanPhamAdmin/Details/5
        public IActionResult Details(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();

            var sanPham = _db.SanPhams
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .Include(s => s.SanPhamSizes)
                    .ThenInclude(ss => ss.MaSizeNavigation)
                .FirstOrDefault(s => s.MaSanPham == id);

            if (sanPham == null) return NotFound();
            return View(sanPham);
        }

        // GET: Admin/SanPhamAdmin/Create
        public IActionResult Create()
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            RepopulateViewBags(null);
            return View();
        }

        // POST: Admin/SanPhamAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham,
            IFormFile? AnhDaiDienFile,
            IEnumerable<IFormFile>? AnhChiTietFile,
            List<SanPhamSize>? Sizes)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");

            // Xử lý ảnh đại diện
            if (AnhDaiDienFile != null && AnhDaiDienFile.Length > 0)
            {
                string ext = Path.GetExtension(AnhDaiDienFile.FileName).ToLower();
                string newFileName = Guid.NewGuid() + ext;
                string avatarFolder = Path.Combine(_env.WebRootPath, "img",
                    sanPham.MaDongSanPham.ToString()!, "Avatar");

                Directory.CreateDirectory(avatarFolder);
                string path = Path.Combine(avatarFolder, newFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await AnhDaiDienFile.CopyToAsync(stream);

                sanPham.AnhDaiDien = newFileName;
            }
            else
            {
                ModelState.AddModelError("", "Vui lòng chọn ảnh đại diện.");
                RepopulateViewBags(sanPham);
                return View(sanPham);
            }

            // Xử lý ảnh chi tiết
            if (AnhChiTietFile != null && AnhChiTietFile.Any())
            {
                var anhList = new List<string>();
                string chiTietFolder = Path.Combine(_env.WebRootPath, "img",
                    sanPham.MaDongSanPham.ToString()!, "ChiTietAnh");
                Directory.CreateDirectory(chiTietFolder);

                foreach (var file in AnhChiTietFile)
                {
                    if (file != null && file.Length > 0)
                    {
                        string ext = Path.GetExtension(file.FileName).ToLower();
                        string newFileName = Guid.NewGuid() + ext;
                        string path = Path.Combine(chiTietFolder, newFileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        anhList.Add(newFileName);
                    }
                }
                sanPham.AnhChiTiet = string.Join(",", anhList);
            }

            // Kiểm tra Size
            if (Sizes == null || !Sizes.Any(s => s.MaSize > 0 && s.SoLuong >= 0))
            {
                ModelState.AddModelError("", "Phải thêm ít nhất một size hợp lệ.");
                RepopulateViewBags(sanPham);
                return View(sanPham);
            }

            _db.SanPhams.Add(sanPham);
            await _db.SaveChangesAsync();

            bool hasValidSize = false;
            foreach (var size in Sizes)
            {
                if (size.MaSize > 0 && size.SoLuong >= 0)
                {
                    size.MaSanPham = sanPham.MaSanPham;
                    _db.SanPhamSizes.Add(size);
                    hasValidSize = true;
                }
            }

            if (!hasValidSize)
            {
                _db.SanPhams.Remove(sanPham);
                await _db.SaveChangesAsync();
                ModelState.AddModelError("", "Phải thêm ít nhất một size hợp lệ.");
                RepopulateViewBags(sanPham);
                return View(sanPham);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/SanPhamAdmin/Delete/5
        public IActionResult Delete(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();

            var sanPham = _db.SanPhams
                .Include(s => s.MaDongSanPhamNavigation)
                .Include(s => s.MaMauNavigation)
                .FirstOrDefault(s => s.MaSanPham == id);

            if (sanPham == null) return NotFound();
            return View(sanPham);
        }

        // POST: Admin/SanPhamAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");

            var sanPham = await _db.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                var sizes = _db.SanPhamSizes.Where(x => x.MaSanPham == id).ToList();
                if (sizes.Any())
                    _db.SanPhamSizes.RemoveRange(sizes);

                _db.SanPhams.Remove(sanPham);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void RepopulateViewBags(SanPham? sp)
        {
            ViewBag.MaDongSanPham = new SelectList(_db.DongSanPhams, "MaDongSanPham", "TenDongSanPham", sp?.MaDongSanPham);
            ViewBag.MaMau = new SelectList(_db.Maus, "MaMau", "TenMau", sp?.MaMau);
            ViewBag.Sizes = new SelectList(_db.Sizes, "MaSize", "TenSize");
        }
    }
}
