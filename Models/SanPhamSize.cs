using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class SanPhamSize
{
    public int MaSanPham { get; set; }

    public int MaSize { get; set; }

    public int? SoLuong { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;

    public virtual Size MaSizeNavigation { get; set; } = null!;
}
