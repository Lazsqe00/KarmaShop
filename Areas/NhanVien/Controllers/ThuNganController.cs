using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarmaShop.Models;


[Area("NhanVien")]
public class ThuNganController : Controller
{
    private readonly QuanLyBanGiayContext _context;
    public ThuNganController(QuanLyBanGiayContext context) => _context = context;


    public async Task<IActionResult> Index(string keyword, int? maLoai, int? maMau, string sizeName, int page = 1)
    {
        const int pageSize = 8;
        if (page < 1) page = 1;

        ViewBag.Loais = await _context.Loais.ToListAsync();
        ViewBag.Maus = await _context.Maus.ToListAsync();
        ViewBag.Sizes = await _context.Sizes.Select(z => z.TenSize).Distinct().ToListAsync();

        var query = _context.SanPhams
            .Include(s => s.MaMauNavigation)
            .Include(s => s.MaDongSanPhamNavigation).ThenInclude(d => d.MaLoaiNavigation)
            .Include(s => s.SanPhamSizes).ThenInclude(sz => sz.MaSizeNavigation)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(s => s.TenSanPham.Contains(keyword) || s.MaSanPham.ToString() == keyword);
        }

        if (maLoai.HasValue)
        {
            query = query.Where(s => s.MaDongSanPhamNavigation != null &&
                                     s.MaDongSanPhamNavigation.MaLoai == maLoai.Value);
        }

        if (maMau.HasValue)
        {
            query = query.Where(s => s.MaMau == maMau.Value);
        }

        if (!string.IsNullOrEmpty(sizeName))
        {
            query = query.Where(s => s.SanPhamSizes.Any(sz => sz.MaSizeNavigation != null &&
                                                              sz.MaSizeNavigation.TenSize == sizeName));
        }


        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page > totalPages && totalPages > 0) page = totalPages;

        var model = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Keyword = keyword;
        ViewBag.CurrentMaLoai = maLoai;
        ViewBag.CurrentMaMau = maMau;
        ViewBag.CurrentSizeName = sizeName;

        return View(model);
    }

    private string LayHangThanhVien(decimal? tongChi)
    {
        
        decimal soTien = tongChi ?? 0;

        if (soTien >= 8000000) return "Kim Cương";
        if (soTien >= 5000000) return "Bạch Kim";
        if (soTien >= 3000000) return "Vàng";
        if (soTien >= 500000) return "Bạc";
        return "Thành Viên";
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuKhachHang(KhachHang kh)
    {
        if (ModelState.IsValid)
        {
          
            if (kh.TongChi == null)
            {
                kh.TongChi = 0;
            }

            _context.KhachHangs.Add(kh);
            await _context.SaveChangesAsync();

            return RedirectToAction("KhachHang");
        }

        return RedirectToAction("KhachHang");
    }
    [HttpGet]
    public async Task<IActionResult> KhachHang(string search, string hang, int page = 1)
    {
        int pageSize = 10;
        var query = _context.KhachHangs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchLower = search.Trim().ToLower();
            int? maTimKiem = null;
            if (searchLower.StartsWith("kh") && int.TryParse(searchLower.Substring(2), out int id))
                maTimKiem = id;
            else if (int.TryParse(searchLower, out int idThuong))
                maTimKiem = idThuong;

            query = query.Where(k =>
                (maTimKiem.HasValue && k.MaKhachHang == maTimKiem.Value) ||
                k.TenKhachHang.ToLower().Contains(searchLower) ||
                (k.SoDienThoai != null && k.SoDienThoai.Contains(searchLower)) ||
                (k.Email != null && k.Email.ToLower().Contains(searchLower))
            );
        }

        if (!string.IsNullOrEmpty(hang))
        {
            query = query.Where(k =>
                (hang == "Kim Cương" && k.TongChi >= 8000000) ||
                (hang == "Bạch Kim" && k.TongChi >= 5000000 && k.TongChi < 8000000) ||
                (hang == "Vàng" && k.TongChi >= 3000000 && k.TongChi < 5000000) ||
                (hang == "Bạc" && k.TongChi >= 500000 && k.TongChi < 3000000) ||
                (hang == "Thành Viên" && (k.TongChi == null || k.TongChi < 500000))
            );
        }

        int totalItems = await query.CountAsync();
        var danhSach = await query
            .OrderByDescending(k => k.MaKhachHang)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentSearch = search;
        ViewBag.CurrentHang = hang;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(danhSach);
    }

    public async Task<IActionResult> ChiTietKhachHang(int id)
    {
        var kh = await _context.KhachHangs
            .Include(k => k.PhieuMuas)
            .FirstOrDefaultAsync(m => m.MaKhachHang == id);

        if (kh == null) return NotFound();

        ViewBag.HangHienTai = LayHangThanhVien(kh.TongChi);

        return View(kh);
    }


    public async Task<IActionResult> LapHoaDon()
    {

        ViewBag.SanPhams = await _context.SanPhams
    .Include(s => s.MaMauNavigation)
    .Include(s => s.MaDongSanPhamNavigation)
    .Include(s => s.SanPhamSizes)
            .ThenInclude(sz => sz.MaSizeNavigation)
    .ToListAsync();

        ViewBag.PTTT = await _context.PhuongThucThanhToans.ToListAsync();
        ViewBag.KhachHangs = await _context.KhachHangs.OrderBy(k => k.TenKhachHang).ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TaoHoaDon([FromBody] PhieuMua hoadon)
    {
        if (hoadon == null || hoadon.ChiTietPhieuMuas == null || !hoadon.ChiTietPhieuMuas.Any())
        {
            return Json(new { success = false, msg = "Dữ liệu hóa đơn trống hoặc không hợp lệ!" });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            hoadon.NgayDat = DateOnly.FromDateTime(DateTime.Now);
            hoadon.TinhTrang = "Chờ thanh toán";

            decimal tongTienDonHang = 0;

            foreach (var ct in hoadon.ChiTietPhieuMuas)
            {
                var kho = await _context.SanPhamSizes.FirstOrDefaultAsync(x => x.MaSanPham == ct.MaSanPham && x.MaSize == ct.MaSize);
                if (kho == null)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, msg = $"Sản phẩm mã SP: {ct.MaSanPham} không tồn tại kích cỡ này!" });
                }

                if (kho.SoLuong < ct.SoLuong)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, msg = $"Sản phẩm hoặc Size này đã hết hàng (Tồn kho: {kho.SoLuong})!" });
                }

                if (ct.DonGia == null || ct.DonGia == 0)
                {
                    var sanPhamWithDong = await _context.SanPhams
                        .Include(s => s.MaDongSanPhamNavigation) 
                        .FirstOrDefaultAsync(s => s.MaSanPham == ct.MaSanPham);

                    if (sanPhamWithDong != null && sanPhamWithDong.MaDongSanPhamNavigation != null)
                    {
                        ct.DonGia = sanPhamWithDong.MaDongSanPhamNavigation.GiaBan ?? 0;
                    }
                    else
                    {
                        ct.DonGia = 0;
                    }
                }

             
                tongTienDonHang += (ct.SoLuong ?? 0) * (ct.DonGia ?? 0);

                
                kho.SoLuong -= ct.SoLuong;
            }
            hoadon.TongTien = tongTienDonHang;

            if (hoadon.MaKhachHang.HasValue)
            {
                var khachHang = await _context.KhachHangs.FindAsync(hoadon.MaKhachHang.Value);
                if (khachHang != null)
                {
                    khachHang.TongChi = (khachHang.TongChi ?? 0) + tongTienDonHang;
                }
            }

            _context.PhieuMuas.Add(hoadon);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, msg = "Lỗi hệ thống: " + ex.Message });
        }
    }

    public async Task<IActionResult> DanhSachHoaDon(string search, string status, DateOnly? date)
    {
       
        var baseQuery = _context.PhieuMuas.AsQueryable();

        ViewBag.Tong = await baseQuery.CountAsync();
        ViewBag.DaTT = await baseQuery.CountAsync(x => x.TinhTrang == "Đã thanh toán");
        ViewBag.ChoTT = await baseQuery.CountAsync(x => x.TinhTrang == "Chờ thanh toán");
        ViewBag.DaHuy = await baseQuery.CountAsync(x => x.TinhTrang == "Đã hủy");

       
        var query = _context.PhieuMuas
            .Include(p => p.MaKhachHangNavigation)
            .Include(p => p.MaPtttNavigation)
            .Include(p => p.ChiTietPhieuMuas)
            .AsQueryable();

        
        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();

            
            if (search.StartsWith("HD", StringComparison.OrdinalIgnoreCase) && int.TryParse(search.Substring(2), out int maPhieu))
            {
                query = query.Where(p => p.MaPhieuMua == maPhieu);
            }
            else if (int.TryParse(search, out int maGoc)) 
            {
                query = query.Where(p => p.MaPhieuMua == maGoc);
            }
            else 
            {
                query = query.Where(p => p.MaKhachHangNavigation.TenKhachHang.Contains(search));
            }
        }

        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.TinhTrang == status);
        if (date.HasValue) query = query.Where(p => p.NgayDat == date);

        var list = await query.OrderByDescending(x => x.MaPhieuMua).ToListAsync();

        foreach (var item in list)
        {
            if (item.TongTien == null || item.TongTien == 0)
            {
                item.TongTien = item.ChiTietPhieuMuas.Sum(ct => (ct.SoLuong ?? 0) * (ct.DonGia ?? 0));
            }
        }

        return View(list);
    }


    [HttpGet]
    public async Task<IActionResult> XuatHoaDon(string id)
    {
        if (string.IsNullOrEmpty(id)) return View();

        string cleanId = id.ToUpper().Replace("HD", "").Replace("KH", "").Trim();

        if (int.TryParse(cleanId, out int maSo))
        {
            var hoadon = await _context.PhieuMuas
    .Include(p => p.MaKhachHangNavigation)
    .Include(p => p.MaPtttNavigation)
    .Include(p => p.ChiTietPhieuMuas)
        .ThenInclude(ct => ct.MaSanPhamNavigation)
            .ThenInclude(sp => sp.MaDongSanPhamNavigation)
    .Include(p => p.ChiTietPhieuMuas)
        .ThenInclude(ct => ct.MaSanPhamNavigation)
            .ThenInclude(sp => sp.MaMauNavigation)
    .Include(p => p.ChiTietPhieuMuas)
        .ThenInclude(ct => ct.MaSizeNavigation)
    .FirstOrDefaultAsync(m => m.MaPhieuMua == maSo);

            return View(hoadon);
        }
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> ThanhToan()
    {
        
        ViewBag.DonChoDuyet = await _context.PhieuMuas.CountAsync(x => x.TinhTrang == "Chờ duyệt");

        var list = await _context.PhieuMuas
            .Where(x => x.TinhTrang == "Chờ thanh toán")
            .Include(p => p.MaKhachHangNavigation)
            .Include(p => p.MaPtttNavigation)
            .Include(p => p.ChiTietPhieuMuas)
                .ThenInclude(ct => ct.MaSanPhamNavigation)
                    .ThenInclude(sp => sp.MaDongSanPhamNavigation)
                        .ThenInclude(dsp => dsp.MaLoaiNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaMauNavigation) 
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSizeNavigation)
            .ToListAsync();

        return View(list);
    }
    [HttpPost]
    public async Task<IActionResult> CapNhatDonHang(int id, string trangThai)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var phieuMua = await _context.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaDongSanPhamNavigation)
                .FirstOrDefaultAsync(p => p.MaPhieuMua == id);

            if (phieuMua == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hóa đơn này!" });
            }

            if (trangThai == "Đã thanh toán")
            {
                phieuMua.TinhTrang = "Đã thanh toán";

                if (phieuMua.MaKhachHangNavigation != null)
                {
                    var khachHang = phieuMua.MaKhachHangNavigation;

                    decimal tongTienHang = 0;
                    foreach (var ct in phieuMua.ChiTietPhieuMuas)
                    {
                        decimal gia = (decimal)(ct.DonGia ?? ct.MaSanPhamNavigation?.MaDongSanPhamNavigation?.GiaBan ?? 0);
                        int sl = ct.SoLuong ?? 0;
                        tongTienHang += (gia * sl);
                    }

                    decimal tc = khachHang.TongChi ?? 0m;
                    string hangHienTai = "Thành Viên";
                    if (tc >= 8000000m) hangHienTai = "Kim Cương";
                    else if (tc >= 5000000m) hangHienTai = "Bạch Kim";
                    else if (tc >= 3000000m) hangHienTai = "Vàng";
                    else if (tc >= 500000m) hangHienTai = "Bạc";


                    decimal chietKhau = 0m;
                    if (hangHienTai == "Vàng" && tongTienHang >= 350000m) chietKhau = tongTienHang * 0.05m;
                    else if (hangHienTai == "Bạch Kim" && tongTienHang >= 550000m) chietKhau = tongTienHang * 0.10m;
                    else if (hangHienTai == "Kim Cương" && tongTienHang >= 800000m) chietKhau = tongTienHang * 0.12m;

                    decimal soTienThucTeThanhToan = tongTienHang - chietKhau;

                    phieuMua.TongTien = soTienThucTeThanhToan;

                    khachHang.TongChi = (khachHang.TongChi ?? 0m) + soTienThucTeThanhToan;

                    _context.KhachHangs.Update(khachHang);
                }
            }
            else if (trangThai == "Đã hủy")
            {
                phieuMua.TinhTrang = "Đã hủy";
            }
            else
            {
                return Json(new { success = false, message = "Trạng thái không hợp lệ!" });
            }

            _context.PhieuMuas.Update(phieuMua);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new { success = true, message = "Cập nhật đơn hàng thành công!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
        }
    }

    public async Task<IActionResult> ChiTietSanPham(int id)
    {
        var sp = await _context.SanPhams
            .Include(s => s.MaDongSanPhamNavigation).ThenInclude(d => d.MaLoaiNavigation)
            .Include(s => s.MaMauNavigation)
            .Include(s => s.SanPhamSizes).ThenInclude(sz => sz.MaSizeNavigation)
            .FirstOrDefaultAsync(m => m.MaSanPham == id);

        if (sp == null) return NotFound();

        if (sp.SanPhamSizes != null)
        {
            sp.SanPhamSizes = sp.SanPhamSizes.OrderBy(sz => sz.MaSize).ToList();
        }

        ViewBag.CurrentMaSP = id;
        return PartialView("_ChiTietSanPham", sp);
    }

}