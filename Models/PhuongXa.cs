using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class PhuongXa
{
    public string MaPhuong { get; set; } = null!;

    public string TenPhuong { get; set; } = null!;

    public string MaQuan { get; set; } = null!;

    public virtual QuanHuyen MaQuanNavigation { get; set; } = null!;

    public virtual ICollection<Sodiachi> Sodiachis { get; set; } = new List<Sodiachi>();
}
