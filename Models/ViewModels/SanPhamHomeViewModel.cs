namespace KarmaShop.Models.ViewModels
{
    public class SanPhamHomeViewModel
    {
        public SanPham sp { get; set; } = null!;
        public string TenSanPhamFull { get; set; } = "";
        public decimal? GiaHienThi { get; set; }
    }
}
