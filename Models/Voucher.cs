using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class Voucher
{
    public string MaVoucher { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal? GiamToiDa { get; set; }

    public DateOnly? NgayTao { get; set; }

    public DateOnly? NgayHetHan { get; set; }

    public virtual ICollection<PhieuMua> PhieuMuas { get; set; } = new List<PhieuMua>();

    public virtual ICollection<ViVoucher> ViVouchers { get; set; } = new List<ViVoucher>();
}
