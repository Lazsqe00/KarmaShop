namespace KarmaShop.Areas.Admin.ViewModels
{
    public class ThongKeDoanhThu
    {
        public List<DoanhThuTheoThang> SaleByMonths { get; set; } = new();
        public List<DoanhThuTheoSanPham> SaleByProducts { get; set; } = new();
    }
}
