using KarmaShop.Models;
using System.Collections.Generic;

namespace KarmaShop.Repository.Cart
{
    public interface CartInterface
    {
        List<ChiTietPhieuMua> GetCartItems();
        void SaveCart(List<ChiTietPhieuMua> cart);
        void ClearCart();
    }
}
