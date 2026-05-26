using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class Sodiachi
{
    public int Masodiachi { get; set; }

    public int MaKhachHang { get; set; }

    public string Tennguoinhan { get; set; } = null!;

    public string Sdtnguoinhan { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public string MaPhuong { get; set; } = null!;

    public string MaQuan { get; set; } = null!;

    public string MaTinh { get; set; } = null!;

    public bool? IsDefault { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual PhuongXa MaPhuongNavigation { get; set; } = null!;

    public virtual QuanHuyen MaQuanNavigation { get; set; } = null!;

    public virtual TinhThanh MaTinhNavigation { get; set; } = null!;
}
