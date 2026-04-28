using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string? TenLoai { get; set; }

    public virtual ICollection<DongSanPham> DongSanPhams { get; set; } = new List<DongSanPham>();
}
