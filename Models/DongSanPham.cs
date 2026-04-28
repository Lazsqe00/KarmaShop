using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class DongSanPham
{
    public int MaDongSanPham { get; set; }

    public int? MaLoai { get; set; }

    public string? TenDongSanPham { get; set; }

    public decimal? GiaBan { get; set; }

    public string? MoTa { get; set; }

    public virtual Loai? MaLoaiNavigation { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
