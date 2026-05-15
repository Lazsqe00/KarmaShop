using KarmaShop.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace KarmaShop.Repository.Cart
{
    public class CartRepository : CartInterface
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "Cart";

        public CartRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<ChiTietPhieuMua> GetCartItems()
        {
            var jsonString = Session.GetString(CartSessionKey);
            return jsonString == null ? new List<ChiTietPhieuMua>() : JsonSerializer.Deserialize<List<ChiTietPhieuMua>>(jsonString);
        }

        public void SaveCart(List<ChiTietPhieuMua> cart)
        {
            var jsonString = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, jsonString);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }
    }
}
