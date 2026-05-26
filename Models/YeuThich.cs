using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class YeuThich
{
    public int MaYeuThich { get; set; }

    public string Email { get; set; } = null!;

    public int MaSanPham { get; set; }

    public DateTime? NgayThem { get; set; }

    public virtual TaiKhoan EmailNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
