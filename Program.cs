using KarmaShop.Models;
using KarmaShop.Repository.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuanLyBanGiayContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("KarmaShop"));
});

builder.Services.AddScoped<UserInterface, UserRepository>();


builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");

// ── Data Seeder ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db  = scope.ServiceProvider.GetRequiredService<QuanLyBanGiayContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // 1. Seed Loai nếu chưa có
        if (!db.Loais.Any())
        {
            db.Loais.AddRange(
                new KarmaShop.Models.Loai { TenLoai = "Giày Chạy Bộ" },
                new KarmaShop.Models.Loai { TenLoai = "Giày Thể Thao" },
                new KarmaShop.Models.Loai { TenLoai = "Giày Casual" }
            );
            db.SaveChanges();
        }

        // 2. Seed Màu nếu chưa có (11 màu)
        var mauSeed = new[]
        {
            "Đỏ", "Đen", "Trắng", "Xanh Dương", "Vàng",
            "Xám", "Hồng", "Cam", "Tím", "Xanh Lá", "Be"
        };
        foreach (var ten in mauSeed)
            if (!db.Maus.Any(m => m.TenMau == ten))
                db.Maus.Add(new KarmaShop.Models.Mau { TenMau = ten });
        db.SaveChanges();

        // 3. Seed Size nếu chưa có
        var sizeSeed = new[] { "38", "39", "40", "41", "42", "43", "44", "45" };
        foreach (var ten in sizeSeed)
            if (!db.Sizes.Any(s => s.TenSize == ten))
                db.Sizes.Add(new KarmaShop.Models.Size { TenSize = ten });
        db.SaveChanges();

        // 4. Seed SanPham – logic thống nhất: 1 sản phẩm = 1 cặp (DongSanPham, Mau)
        //    Avatar/ChiTiet xoay vòng theo index màu trong folder.
        //    MaxColors giới hạn số màu tối đa mỗi DongSanPham.
        var maus  = db.Maus.OrderBy(m => m.MaMau).ToList();
        var sizes = db.Sizes.OrderBy(s => s.MaSize).ToList();

        // Map: MaDongSanPham → (Avatars, ChiTiets, MaxColors)
        var folderData = new Dictionary<int, (string[] Avatars, string[] ChiTiets, int MaxColors)>
        {
            // ── Folder 1: 7 avatars, 7 ảnh chi tiết, tối đa 9 màu ──────────
            [1] = (
                Avatars: new[]
                {
                    "1d237d10-9b3d-4bf3-9720-f3c60c2ceb58.png",
                    "60418bef-99a3-4a33-87e5-14a1d342f73b.png",
                    "8dcd940c-e32e-4f9e-abcf-80c89cccae97.png",
                    "9db17a18-2ced-4cac-a3db-5073cc803900.png",
                    "9f67fc47-4e4e-4ad5-baf6-0dcd554d4a6e.png",
                    "b75a43a9-319b-4ee9-9f36-2172f7a15437.png",
                    "e1a568c0-c234-4fdb-8eb0-fb633fadbaa3.png"
                },
                ChiTiets: new[]
                {
                    "01743731-82b5-4e94-9839-576851f7d1e8.png,1792050c-68a0-4f8f-9a04-c21f11f69c1d.png,3a4cab79-f5a5-4657-b7b2-d6f8606f35d4.png",
                    "4f085a8c-da31-4b3d-a74e-583dfbc693ac.png,5c8eabcc-80c6-4ba6-952d-31a4a42b8558.png,5de0c146-6bb8-4554-9729-c883f0a52faf.png",
                    "64f13f6c-a06d-4e87-b8ce-512fd65f6262.png,65d17158-540f-46fc-8c60-da9b2cb2aa9a.png,6862850a-7426-4339-9cb8-eca48d282ba9.png",
                    "70a870fb-f867-4d64-ae35-ac29300277b4.png,72bafd19-6fa5-44ba-a351-15828ab68e4b.png,776516f7-39e0-431c-aae2-65a9e0104f19.png",
                    "77e4f8fb-b502-444d-afa4-b1c378be46e5.png,8134af04-4cf1-47e0-a20a-b1c0dfac4eb3.png,8652e4b1-0ba4-47c4-abb5-fc7a0d479240.png",
                    "d51829d6-f437-4c56-be5d-ce6711dfc161.png,da8c0542-d856-458d-8fea-6838f0cabf94.png,e5e157b4-2774-4942-a267-5b757a74728c.png",
                    "e8014f4e-b52b-434a-8368-fb862a4bb374.png,ece90a42-1b83-4606-a0d7-2ec3305b7304.png"
                },
                MaxColors: 9
            ),
            // ── Folder 11: 1 avatar, tối đa 4 màu ──────────────────────────
            [11] = (
                Avatars: new[] { "6857db27-d081-4e03-ba45-3a069ac8ac4b.png" },
                ChiTiets: new[] { "529712c7-6ad4-4ed7-a3cb-67c615df021f.png,94c5633e-963a-4405-b325-6b8ec9a62974.png" },
                MaxColors: 4
            ),
            // ── Folder 12: 2 avatars, tối đa 5 màu ─────────────────────────
            [12] = (
                Avatars: new[]
                {
                    "0767ad11-fe09-4047-8b2e-5c2decbc340b.png",
                    "9ffe3f9b-ae73-4513-9db7-ab30b283819c.png"
                },
                ChiTiets: new[]
                {
                    "19bcd5f9-ef8e-4fd8-9474-7c14d4c130b7.png,1a763d6a-861c-400c-888b-9a8d9adecefb.png",
                    "5e17d2fe-14e6-4f70-9356-be140fdb4312.png,a81b289e-ee05-4ea4-96f7-2a8b537fb5ce.png"
                },
                MaxColors: 5
            ),
            // ── Folder 13: 1 avatar, tối đa 4 màu ──────────────────────────
            [13] = (
                Avatars: new[] { "ca699d86-2f91-4d6e-81d2-159556649bbe.png" },
                ChiTiets: new[] { "34dd8f16-6fce-4c4d-8baa-538ff96317e5.png,a7307524-91a6-4cd5-9466-7cca4c75dae2.png" },
                MaxColors: 4
            ),
            // ── Folder 14: 2 avatars, tối đa 5 màu ─────────────────────────
            [14] = (
                Avatars: new[]
                {
                    "18cc413a-0e87-41c7-a98e-cd2eb2f63b97.png",
                    "22077d46-4f70-4a66-a7d9-f489e432f53d.png"
                },
                ChiTiets: new[]
                {
                    "1d235cb5-0f4c-4c51-b8c8-c89396af4301.png,22963f5c-3c89-4d8f-9816-9e5fc75f4fa3.png",
                    "9c0a89d6-9521-43d6-b2d6-2f5a35d5de6c.png,a45cfcaf-520d-4a97-972f-8ffba5988655.png"
                },
                MaxColors: 5
            ),
            // ── Folder 15: 1 avatar, tối đa 3 màu ──────────────────────────
            [15] = (
                Avatars: new[] { "a8d73abc-8ecb-4c04-ab07-37188320198a.png" },
                ChiTiets: new[] { "4ac1f7df-6830-453a-a5e5-dbfe8578e6a4.png,84c3d2ce-3efb-4350-95c3-d03ac4d72464.png" },
                MaxColors: 3
            ),
            // ── Folder 16: 1 avatar, tối đa 3 màu ──────────────────────────
            [16] = (
                Avatars: new[] { "e13b9e8c-e7a8-4f5b-b8a3-f9b462ac1f9c.png" },
                ChiTiets: new[] { "6086666b-1cfc-4d86-b070-31e3968a1da4.png,ef0e82c4-836a-4d5f-9376-1ed54cfd403b.png" },
                MaxColors: 3
            ),
            // ── Folder 17: 3 avatars, tối đa 5 màu ─────────────────────────
            [17] = (
                Avatars: new[]
                {
                    "26ac6f0f-9cf8-410d-98b7-52601505485f.png",
                    "73ff7135-62f9-4361-929a-7b58ca0a8798.png",
                    "ad280bad-c640-4a5c-a0f1-3a590c1571ac.png"
                },
                ChiTiets: new[]
                {
                    "18dcb5ed-f2bd-494d-801b-7cdac1ba3fed.png,2c44a9e5-243c-4828-80f9-fd0590be39ac.png",
                    "457f796f-3bb4-4c85-a182-9aa49142ee6b.png,6c6b5e13-9af6-480c-b7ae-e6befb6f553a.png",
                    "dfabafe3-2a6c-4313-a027-c0e9ebb49ff2.png,e05ecf23-c778-4083-b3a7-d81495464645.png"
                },
                MaxColors: 5
            )
        };

        // Vòng lặp thống nhất: 1 sản phẩm = 1 cặp (MaDongSanPham, MaMau)
        foreach (var folder in folderData)
        {
            int maDsp = folder.Key;
            if (!db.DongSanPhams.Any(d => d.MaDongSanPham == maDsp))
                continue; // DongSanPham không tồn tại → bỏ qua

            var dsp      = db.DongSanPhams.First(d => d.MaDongSanPham == maDsp);
            var avatars  = folder.Value.Avatars;
            var chiTiets = folder.Value.ChiTiets;
            int maxColors = folder.Value.MaxColors;
            int seeded   = 0;

            for (int ci = 0; ci < maus.Count && seeded < maxColors; ci++)
            {
                var mau = maus[ci];

                // Kiểm tra cặp (DongSanPham, Mau) – bỏ qua nếu đã có
                if (db.SanPhams.Any(sp => sp.MaDongSanPham == maDsp && sp.MaMau == mau.MaMau))
                    continue;

                // Avatar và ảnh chi tiết xoay vòng theo thứ tự màu hiện tại
                int idx = seeded % avatars.Length;
                var sp  = new KarmaShop.Models.SanPham
                {
                    MaDongSanPham = maDsp,
                    TenSanPham    = $"{dsp.TenDongSanPham} {mau.TenMau}".ToUpper(),
                    MaMau         = mau.MaMau,
                    AnhDaiDien    = avatars[idx],
                    AnhChiTiet    = chiTiets[idx % chiTiets.Length],
                    TrangThai     = seeded % 2 == 0
                };
                db.SanPhams.Add(sp);
                db.SaveChanges();

                foreach (var size in sizes.Take(6))
                {
                    db.SanPhamSizes.Add(new KarmaShop.Models.SanPhamSize
                    {
                        MaSanPham = sp.MaSanPham,
                        MaSize    = size.MaSize,
                        SoLuong   = Random.Shared.Next(5, 25)
                    });
                }
                db.SaveChanges();
                seeded++;
            }
        }
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Lỗi khi seed dữ liệu.");
    }
}
// ────────────────────────────────────────────────────────────────────────────

app.Run();
