using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class TaiKhoan
{
    public string Email { get; set; } = null!;

    public string? MatKhau { get; set; }

    public int? LoaiTaiKhoan { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
