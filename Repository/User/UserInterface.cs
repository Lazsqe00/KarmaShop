using KarmaShop.Models;

namespace KarmaShop.Repository.User
{
    public interface UserInterface
    {
        TaiKhoan? GetUser(string email, string password);
        bool CheckEmailExists(string email);

        void Register(TaiKhoan tk, KhachHang kh);

        KhachHang GetProfile(string email);
        bool UpdateProfile(string email, KhachHang kh);
        decimal GetTongChi(string email);
    }
}
