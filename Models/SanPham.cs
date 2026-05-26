using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public int? MaDongSanPham { get; set; }

    public string? TenSanPham { get; set; }

    public int? MaMau { get; set; }

    public string? AnhDaiDien { get; set; }

    public string? AnhChiTiet { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<ChiTietPhieuMua> ChiTietPhieuMuas { get; set; } = new List<ChiTietPhieuMua>();

    public virtual DongSanPham? MaDongSanPhamNavigation { get; set; }

    public virtual Mau? MaMauNavigation { get; set; }

    public virtual ICollection<SanPhamSize> SanPhamSizes { get; set; } = new List<SanPhamSize>();

    public virtual ICollection<YeuThich> YeuThiches { get; set; } = new List<YeuThich>();
}
