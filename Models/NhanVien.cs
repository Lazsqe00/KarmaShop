using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class NhanVien
{
    public int MaNhanVien { get; set; }

    public string? TenNhanVien { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public virtual TaiKhoan? EmailNavigation { get; set; }

    public virtual ICollection<PhieuMua> PhieuMuas { get; set; } = new List<PhieuMua>();
}
