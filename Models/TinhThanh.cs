using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class TinhThanh
{
    public string MaTinh { get; set; } = null!;

    public string TenTinh { get; set; } = null!;

    public virtual ICollection<QuanHuyen> QuanHuyens { get; set; } = new List<QuanHuyen>();

    public virtual ICollection<Sodiachi> Sodiachis { get; set; } = new List<Sodiachi>();
}
