using System;
using System.Collections.Generic;

namespace KarmaShop.Models;

public partial class PhieuMua
{
    public int MaPhieuMua { get; set; }

    public DateOnly? NgayDat { get; set; }

    public int? MaKhachHang { get; set; }

    public int? MaNhanVien { get; set; }

    public int? MaPttt { get; set; }

    public string? MaVoucher { get; set; }

    public string? TinhTrang { get; set; }

    public string? GhiChu { get; set; }

    public decimal? TongTien { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? EmailNguoiNhan { get; set; }

    public string? SoDienThoaiNguoiNhan { get; set; }

    public string? TenNguoiNhan { get; set; }

    public virtual ICollection<ChiTietPhieuMua> ChiTietPhieuMuas { get; set; } = new List<ChiTietPhieuMua>();

    public virtual KhachHang? MaKhachHangNavigation { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }

    public virtual PhuongThucThanhToan? MaPtttNavigation { get; set; }

    public virtual Voucher? MaVoucherNavigation { get; set; }

    public virtual ICollection<ViVoucher> ViVouchers { get; set; } = new List<ViVoucher>();
}
