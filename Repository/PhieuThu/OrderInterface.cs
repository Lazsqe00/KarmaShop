using KarmaShop.Models;
using System.Collections.Generic;

namespace KarmaShop.Repository.PhieuThu
{
    public interface OrderInterface
    {
        KhachHang GetKhachHangByEmail(string email);
        List<PhuongThucThanhToan> GetPhuongThucThanhToans();
        PhieuMua GetOrder(int id);

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void SaveOrder(PhieuMua order, List<ChiTietPhieuMua> cartItems);

        Sodiachi GetDefaultAddress(int maKhachHang);
        PhuongThucThanhToan GetPhuongThucThanhToanById(int maPTTT); 
    }
}