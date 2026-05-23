using KarmaShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KarmaShop.Repository.Cart
{
    public class ProductRepository : ProductInterface
    {
        private readonly QuanLyBanGiayContext _db;

        public ProductRepository(QuanLyBanGiayContext context)
        {
            _db = context;
        }

        public async Task<SanPham> GetProduct(int maSanPham)
        {
            return await _db.SanPhams
                             .Include(x => x.MaDongSanPhamNavigation)
                             .FirstOrDefaultAsync(x => x.MaSanPham == maSanPham);
        }

        public async Task<SanPhamSize> GetProductSize(int maSanPham, int maSize)
        {
            return await _db.SanPhamSizes
                             .Include(x => x.MaSizeNavigation)
                             .FirstOrDefaultAsync(x => x.MaSanPham == maSanPham && x.MaSize == maSize);
        }

        public int GetTonKho(int maSanPham, int maSize)
        {
            return _db.SanPhamSizes
                       .FirstOrDefault(x => x.MaSanPham == maSanPham && x.MaSize == maSize)?.SoLuong ?? 0;
        }
    }
}

