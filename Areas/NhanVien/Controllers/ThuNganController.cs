using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarmaShop.Models;

[Area("NhanVien")]
public class ThuNganController : Controller
{
    private readonly QuanLyBanGiayContext _context;
    public ThuNganController(QuanLyBanGiayContext context) => _context = context;

   
    public async Task<IActionResult> Index(string keyword, int? maLoai)
    {
        
        ViewBag.Loais = await _context.Loais.ToListAsync();

        var query = _context.SanPhams
            .Include(s => s.MaDongSanPhamNavigation) 
            .Include(s => s.SanPhamSizes)           
            
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(s => s.TenSanPham.Contains(keyword) || s.MaSanPham.ToString() == keyword);

        var model = await query.ToListAsync();

        
        return View(model);
    }

    
    public async Task<IActionResult> KhachHang(string search)
    {
        var query = _context.KhachHangs.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(k => k.SoDienThoai.Contains(search) || k.TenKhachHang.Contains(search));

        return View(await query.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> LuuKhachHang(KhachHang kh)
    {
        _context.KhachHangs.Add(kh);
        await _context.SaveChangesAsync();
        return RedirectToAction("KhachHang");
    }

    public async Task<IActionResult> LapHoaDon()
    {
        ViewBag.SanPhams = await _context.SanPhams.ToListAsync();
        ViewBag.PTTT = await _context.PhuongThucThanhToans.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TaoHoaDon([FromBody] PhieuMua hoadon)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            hoadon.NgayDat = DateOnly.FromDateTime(DateTime.Now);
            hoadon.TinhTrang = "Chờ thanh toán";

            _context.PhieuMuas.Add(hoadon);
            await _context.SaveChangesAsync(); 

            foreach (var ct in hoadon.ChiTietPhieuMuas)
            {
                var kho = await _context.SanPhamSizes.FirstOrDefaultAsync(x => x.MaSanPham == ct.MaSanPham && x.MaSize == ct.MaSize);
                if (kho != null)
                {
                    if (kho.SoLuong < ct.SoLuong) return Json(new { success = false, msg = "Hết hàng!" });
                    kho.SoLuong -= ct.SoLuong; 
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Json(new { success = true });
        }
        catch { return Json(new { success = false }); }
    }

    public async Task<IActionResult> DanhSachHoaDon(string search, string status, DateOnly? date)
    {
        var query = _context.PhieuMuas.Include(p => p.MaKhachHangNavigation).Include(p => p.MaPtttNavigation).AsQueryable();

        if (!string.IsNullOrEmpty(search)) query = query.Where(p => p.MaPhieuMua.ToString() == search || p.MaKhachHangNavigation.TenKhachHang.Contains(search));
        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.TinhTrang == status);
        if (date.HasValue) query = query.Where(p => p.NgayDat == date);

        var list = await query.OrderByDescending(x => x.MaPhieuMua).ToListAsync();

        ViewBag.Tong = list.Count;
        ViewBag.DaTT = list.Count(x => x.TinhTrang == "Đã thanh toán");
        ViewBag.ChoTT = list.Count(x => x.TinhTrang == "Chờ thanh toán");
        ViewBag.DaHuy = list.Count(x => x.TinhTrang == "Đã hủy");

        return View(list);
    }

   
    public async Task<IActionResult> XuatHoaDon(string id)
    {
        if (string.IsNullOrEmpty(id)) return View();

       
        string cleanId = id.ToUpper().Replace("HD", "").Replace("KH", "").Trim();

        if (int.TryParse(cleanId, out int maSo))
        {
            var hoadon = await _context.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.ChiTietPhieuMuas).ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaPhieuMua == maSo);
            return View(hoadon);
        }
        return View();
    }

    
    public async Task<IActionResult> ThanhToan()
    {
        ViewBag.DonChoDuyet = await _context.PhieuMuas.CountAsync(x => x.TinhTrang == "Chờ duyệt");
        var list = await _context.PhieuMuas.Where(x => x.TinhTrang == "Chờ thanh toán").Include(p => p.MaKhachHangNavigation).ToListAsync();
        return View(list);
    }


    public async Task<IActionResult> ChiTietSanPham(int id)
    {
        var sp = await _context.SanPhams
            .Include(s => s.MaDongSanPhamNavigation)
            .Include(s => s.MaMauNavigation)
            .Include(s => s.SanPhamSizes).ThenInclude(sz => sz.MaSizeNavigation)
            .FirstOrDefaultAsync(m => m.MaSanPham == id);
        if (sp == null) return NotFound();
        return PartialView("_ChiTietSanPham", sp);
    }

    public async Task<IActionResult> ChiTietKhachHang(int id)
    {
        var kh = await _context.KhachHangs
            .Include(k => k.PhieuMuas)
            .FirstOrDefaultAsync(m => m.MaKhachHang == id);
        if (kh == null) return NotFound();
        return View(kh);
    }
}