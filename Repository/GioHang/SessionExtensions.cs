using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace KarmaShop.Repository.GioHang;
public static class SessionExtensions
{
    // Hàm lưu đối tượng vào Session (chuyển sang JSON string)
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    // Hàm lấy đối tượng từ Session (giải mã từ JSON string)
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}