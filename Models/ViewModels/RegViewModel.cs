using System.ComponentModel.DataAnnotations;

namespace KarmaShop.Models.ViewModels
{
    public class RegViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = "";

        [Required]
        public string TenKhachHang { get; set; } = "";

        public string? SoDienThoai { get; set; }
        public string? GioiTinh { get; set; }
        public DateOnly? NgaySinh { get; set; }
    }
}
