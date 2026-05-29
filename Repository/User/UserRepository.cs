using KarmaShop.Models;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Repository.User
{
    public class UserRepository : UserInterface
    {
        QuanLyBanGiayContext db;

        public UserRepository(QuanLyBanGiayContext db)
        {
            this.db = db;
        }

        public TaiKhoan? GetUser(string email, string password)
        {
            return db.TaiKhoans.FirstOrDefault(t => t.Email == email
            && t.MatKhau == password
            && t.LoaiTaiKhoan == 0);
        }

        public bool CheckEmailExists(string email)
        {
            return db.TaiKhoans.Any(t => t.Email == email);
        }

        public void Register(TaiKhoan tk, KhachHang kh)
        {
            db.TaiKhoans.Add(tk);
            db.KhachHangs.Add(kh);
            db.SaveChanges();
        }

        public KhachHang GetProfile(string email)
        {
            return db.KhachHangs.FirstOrDefault(k => k.Email == email)!;
        }


        public bool UpdateProfile(string email, KhachHang kh)
        {
            var user = db.KhachHangs.FirstOrDefault(k => k.Email == email);
            if (user == null) return false;

            user.TenKhachHang = kh.TenKhachHang;
            user.SoDienThoai = kh.SoDienThoai;
            user.GioiTinh = kh.GioiTinh;
            user.NgaySinh = kh.NgaySinh;
            user.DiaChi = kh.DiaChi;

            db.SaveChanges();
            return true;
        }

        public decimal GetTongChi(string email)
        {
            return db.PhieuMuas
                      .Where(p => p.MaKhachHangNavigation!.Email == email && p.TinhTrang == "Đã giao hàng") 
                      .Sum(p => (decimal?)p.TongTien) ?? 0;
        }

        public List<PhieuMua> GetOrderHistory(string email)
        {
            return db.PhieuMuas
                     .Include(p => p.MaKhachHangNavigation)
                     .Where(p => p.MaKhachHangNavigation.Email == email)
                     .OrderByDescending(p => p.NgayDat)
                     .ToList();
        }

        public PhieuMua? GetOrderDetail(int id, string email)
        {
            return db.PhieuMuas
                .Include(p => p.MaKhachHangNavigation)
                .Include(p => p.MaPtttNavigation)
                .Include(p => p.MaVoucherNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaDongSanPhamNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                        .ThenInclude(sp => sp.MaMauNavigation)
                .Include(p => p.ChiTietPhieuMuas)
                    .ThenInclude(ct => ct.MaSizeNavigation)
                .FirstOrDefault(p => p.MaPhieuMua == id
                                  && p.MaKhachHangNavigation.Email == email);
        }

        public List<Sodiachi> GetAddressBook(string email)
        {
            return db.Sodiachis
                     .Include(s => s.MaTinhNavigation)
                     .Include(s => s.MaQuanNavigation)
                     .Include(s => s.MaPhuongNavigation)
                     .Where(s => s.MaKhachHangNavigation.Email == email)
                     .OrderByDescending(s => s.IsDefault)
                     .ToList();
        }
    }
}
