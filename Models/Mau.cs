using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class Mau
{
    public int MaMau { get; set; }

    public string? TenMau { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
