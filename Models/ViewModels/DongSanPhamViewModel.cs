namespace KarmaShop.Models.ViewModels
{
    public class DongSanPhamViewModel
    {
        public int MaDongSanPham { get; set; }
        public string TenDongSanPham { get; set; } = "";
        public decimal? GiaBan { get; set; }
        public List<SanPham> SanPhams { get; set; } = new();
    }
}
