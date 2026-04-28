using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class ChiTietPhieuMua
{
    public int MaPhieuMua { get; set; }

    public int MaSanPham { get; set; }

    public int MaSize { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public virtual PhieuMua MaPhieuMuaNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;

    public virtual Size MaSizeNavigation { get; set; } = null!;
}
