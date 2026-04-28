using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class PhuongThucThanhToan
{
    public int MaPttt { get; set; }

    public string TenPttt { get; set; } = null!;

    public virtual ICollection<PhieuMua> PhieuMuas { get; set; } = new List<PhieuMua>();
}
