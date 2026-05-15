using KarmaShop.Models;
using KarmaShop.Repositories;
using KarmaShop.Repository.Cart;
using KarmaShop.Repository.PhieuThu;
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

builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<UserInterface, UserRepository>();

builder.Services.AddScoped<ProductInterface, ProductRepository>();
builder.Services.AddScoped<CartInterface, CartRepository>();

builder.Services.AddScoped<VoucherInterface, VoucherRepository>();
builder.Services.AddScoped<OrderInterface, OrderRepository>();

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
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");


app.Run();
