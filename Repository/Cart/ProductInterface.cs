using KarmaShop.Models;

namespace KarmaShop.Repository.Cart
{
    public interface ProductInterface
    {
        Task<SanPham> GetProduct(int maSanPham);
        Task<SanPhamSize> GetProductSize(int maSanPham, int maSize);
        int GetTonKho(int maSanPham, int maSize);
    }
}
