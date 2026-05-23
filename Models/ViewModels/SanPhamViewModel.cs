namespace KarmaShop.Models.ViewModels
{
    public class SanPhamViewModel
    {
        public SanPham SanPham { get; set; } = null!;
        public DongSanPham DongSanPham { get; set; } = null!;
        public List<SanPham> SanPhams { get; set; } = new();
        public List<string> TenSize { get; set; } = new();
        public List<int> SoLuongTon { get; set; } = new();
        public List<int> MaSize { get; set; } = new();
    }
}
