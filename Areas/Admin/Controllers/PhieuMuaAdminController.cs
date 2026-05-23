using KarmaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhieuMuaAdminController : Controller
    {
        private readonly QuanLyBanGiayContext _db;

        public PhieuMuaAdminController(QuanLyBanGiayContext db)
        {
            _db = db;
        }

        private bool IsAdminOrStaff()
        {
            var loaiTK = HttpContext.Session.GetString("LoaiTK");
            return loaiTK == "1" || loaiTK == "2";
        }

        // GET: Admin/PhieuMuaAdmin
        public IActionResult Index(int page = 1)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            const int pageSize = 10;
            var query = _db.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.MaVoucherNavigation)
                .OrderByDescending(p => p.MaPhieuMua);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(query.Count() / (double)pageSize);
            var list = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(list);
        }

        // GET: Admin/PhieuMuaAdmin/Details/5
        public IActionResult Details(int? id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id == null) return BadRequest();

            var phieu = _db.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.MaVoucherNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaDongSanPhamNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSizeNavigation)
                .FirstOrDefault(p => p.MaPhieuMua == id);

            if (phieu == null) return NotFound();
            return View(phieu);
        }

        // POST: Admin/PhieuMuaAdmin/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");

            var phieu = await _db.PhieuMuas.FindAsync(id);
            if (phieu != null)
            {
                int? maNhanVien = HttpContext.Session.GetInt32("MaNhanVien");
                if (maNhanVien.HasValue)
                    phieu.MaNhanVien = maNhanVien.Value;

                phieu.TinhTrang = "Đang giao hàng";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/PhieuMuaAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");

            var phieu = await _db.PhieuMuas.FindAsync(id);
            if (phieu != null)
            {
                int? maNhanVien = HttpContext.Session.GetInt32("MaNhanVien");
                if (maNhanVien.HasValue)
                    phieu.MaNhanVien = maNhanVien.Value;

                phieu.TinhTrang = "Từ chối";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/PhieuMuaAdmin/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int MaPhieuMua, string TinhTrangMoi, string? GhiChu)
        {
            if (!IsAdminOrStaff())
                return Json(new { success = false, message = "Không có quyền!" });

            var phieu = await _db.PhieuMuas.FindAsync(MaPhieuMua);
            if (phieu == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });

            if (TinhTrangMoi != "Đang giao hàng" && TinhTrangMoi != "Từ chối")
                return Json(new { success = false, message = "Trạng thái không hợp lệ!" });

            phieu.TinhTrang = TinhTrangMoi;
            if (!string.IsNullOrEmpty(GhiChu))
            {
                phieu.GhiChu = (phieu.GhiChu ?? "") +
                    "\n[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "] " + GhiChu;
            }

            int? maNhanVien = HttpContext.Session.GetInt32("MaNhanVien");
            if (maNhanVien.HasValue && maNhanVien.Value > 0)
                phieu.MaNhanVien = maNhanVien.Value;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{MaPhieuMua} thành {TinhTrangMoi}!";
            return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = MaPhieuMua }) });
        }
    }
}
