using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public decimal? TongChi { get; set; }

    public virtual TaiKhoan? EmailNavigation { get; set; }

    public virtual ICollection<PhieuMua> PhieuMuas { get; set; } = new List<PhieuMua>();

    public virtual ICollection<Sodiachi> Sodiachis { get; set; } = new List<Sodiachi>();

    public virtual ICollection<ViVoucher> ViVouchers { get; set; } = new List<ViVoucher>();
}
