using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class ViVoucher
{
    public int MaLuuVoucher { get; set; }

    public int? MaKhachHang { get; set; }

    public string? MaVoucher { get; set; }

    public bool? TrangThaiSuDung { get; set; }

    public DateTime? NgayNhan { get; set; }

    public DateTime? NgaySuDung { get; set; }

    public int? MaPhieuMua { get; set; }

    public virtual KhachHang? MaKhachHangNavigation { get; set; }

    public virtual PhieuMua? MaPhieuMuaNavigation { get; set; }

    public virtual Voucher? MaVoucherNavigation { get; set; }
}
