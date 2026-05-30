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

        public IActionResult Index(int page = 1)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            const int pageSize = 10;
            var query = _db.Vouchers.OrderBy(v => v.MaVoucher);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages  = (int)Math.Ceiling(query.Count() / (double)pageSize);
            return View(query.Skip((page - 1) * pageSize).Take(pageSize).ToList());
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
        public async Task<IActionResult> Create([Bind("MaVoucher,SoLuong,GiamToiDa,NgayTao,NgayHetHan,GiaTriToiThieu,LoaiGiamGia,HangApDung")] Voucher item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (ModelState.IsValid)
            {
                item.NgayTao ??= DateOnly.FromDateTime(DateTime.Now);
                item.LoaiGiamGia = string.IsNullOrWhiteSpace(item.LoaiGiamGia) ? "Phần trăm" : item.LoaiGiamGia.Trim();
                item.HangApDung = string.IsNullOrWhiteSpace(item.HangApDung) ? "Tất cả" : item.HangApDung.Trim();

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
        public async Task<IActionResult> Edit(string id, [Bind("MaVoucher,SoLuong,GiamToiDa,NgayTao,NgayHetHan,GiaTriToiThieu,LoaiGiamGia,HangApDung")] Voucher item)
        {
            if (!IsAdminOrStaff()) return RedirectToAction("Login", "TaiKhoanAdmin");
            if (id != item.MaVoucher) return BadRequest();
            if (ModelState.IsValid)
            {
                item.LoaiGiamGia = string.IsNullOrWhiteSpace(item.LoaiGiamGia) ? "Phần trăm" : item.LoaiGiamGia.Trim();
                item.HangApDung = string.IsNullOrWhiteSpace(item.HangApDung) ? "Tất cả" : item.HangApDung.Trim();
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
