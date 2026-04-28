using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class Size
{
    public int MaSize { get; set; }

    public string? TenSize { get; set; }

    public virtual ICollection<ChiTietPhieuMua> ChiTietPhieuMuas { get; set; } = new List<ChiTietPhieuMua>();

    public virtual ICollection<SanPhamSize> SanPhamSizes { get; set; } = new List<SanPhamSize>();
}
