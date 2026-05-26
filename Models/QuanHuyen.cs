using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class QuanHuyen
{
    public string MaQuan { get; set; } = null!;

    public string TenQuan { get; set; } = null!;

    public string MaTinh { get; set; } = null!;

    public virtual TinhThanh MaTinhNavigation { get; set; } = null!;

    public virtual ICollection<PhuongXa> PhuongXas { get; set; } = new List<PhuongXa>();

    public virtual ICollection<Sodiachi> Sodiachis { get; set; } = new List<Sodiachi>();
}
