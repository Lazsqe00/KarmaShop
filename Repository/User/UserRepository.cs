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


    }
}
