using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KarmaShop.Models;

public partial class QuanLyBanGiayContext : DbContext
{
    public QuanLyBanGiayContext()
    {
    }

    public QuanLyBanGiayContext(DbContextOptions<QuanLyBanGiayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietPhieuMua> ChiTietPhieuMuas { get; set; }

    public virtual DbSet<DongSanPham> DongSanPhams { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<Mau> Maus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuMua> PhieuMuas { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }

    public virtual DbSet<PhuongXa> PhuongXas { get; set; }

    public virtual DbSet<QuanHuyen> QuanHuyens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamSize> SanPhamSizes { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Sodiachi> Sodiachis { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TinhThanh> TinhThanhs { get; set; }

    public virtual DbSet<ViVoucher> ViVouchers { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<YeuThich> YeuThiches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-2D858Q81;Initial Catalog=QuanLyBanGiay;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietPhieuMua>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuMua, e.MaSanPham, e.MaSize }).HasName("PK__ChiTietP__30A053436B607305");

            entity.ToTable("ChiTietPhieuMua");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaPhieuMuaNavigation).WithMany(p => p.ChiTietPhieuMuas)
                .HasForeignKey(d => d.MaPhieuMua)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPh__MaPhi__31EC6D26");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuMuas)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPh__MaSan__32E0915F");

            entity.HasOne(d => d.MaSizeNavigation).WithMany(p => p.ChiTietPhieuMuas)
                .HasForeignKey(d => d.MaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPh__MaSiz__33D4B598");
        });

        modelBuilder.Entity<DongSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDongSanPham).HasName("PK__DongSanP__6F57F00871C7A807");

            entity.ToTable("DongSanPham");

            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenDongSanPham).HasMaxLength(255);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.DongSanPhams)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK__DongSanPh__MaLoa__182C9B23");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E53B98DAC3");

            entity.ToTable("KhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
            entity.Property(e => e.TongChi).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.Email)
                .HasConstraintName("FK__KhachHang__Email__24927208");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__Loai__730A57599FE8BC38");

            entity.ToTable("Loai");

            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<Mau>(entity =>
        {
            entity.HasKey(e => e.MaMau).HasName("PK__Mau__3A5BBB7D0FCF05A5");

            entity.ToTable("Mau");

            entity.Property(e => e.TenMau).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA474097CAF2");

            entity.ToTable("NhanVien");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenNhanVien).HasMaxLength(100);

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.Email)
                .HasConstraintName("FK__NhanVien__Email__276EDEB3");
        });

        modelBuilder.Entity<PhieuMua>(entity =>
        {
            entity.HasKey(e => e.MaPhieuMua).HasName("PK__PhieuMua__13ABA0E64D4876E0");

            entity.ToTable("PhieuMua");

            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(255);
            entity.Property(e => e.EmailNguoiNhan)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaPttt).HasColumnName("MaPTTT");
            entity.Property(e => e.MaVoucher).HasMaxLength(20);
            entity.Property(e => e.SoDienThoaiNguoiNhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.TinhTrang).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.PhieuMuas)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__PhieuMua__MaKhac__2C3393D0");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.PhieuMuas)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__PhieuMua__MaNhan__2D27B809");

            entity.HasOne(d => d.MaPtttNavigation).WithMany(p => p.PhieuMuas)
                .HasForeignKey(d => d.MaPttt)
                .HasConstraintName("FK__PhieuMua__MaPTTT__2F10007B");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.PhieuMuas)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__PhieuMua__MaVouc__2E1BDC42");
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaPttt).HasName("PK__PhuongTh__B30A2802FD0AD6F9");

            entity.ToTable("PhuongThucThanhToan");

            entity.Property(e => e.MaPttt).HasColumnName("MaPTTT");
            entity.Property(e => e.TenPttt)
                .HasMaxLength(100)
                .HasColumnName("TenPTTT");
        });

        modelBuilder.Entity<PhuongXa>(entity =>
        {
            entity.HasKey(e => e.MaPhuong).HasName("PK__PhuongXa__F7B9B544CB34478C");

            entity.ToTable("PhuongXa");

            entity.Property(e => e.MaPhuong)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaQuan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenPhuong).HasMaxLength(100);

            entity.HasOne(d => d.MaQuanNavigation).WithMany(p => p.PhuongXas)
                .HasForeignKey(d => d.MaQuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhuongXa__MaQuan__59063A47");
        });

        modelBuilder.Entity<QuanHuyen>(entity =>
        {
            entity.HasKey(e => e.MaQuan).HasName("PK__QuanHuye__60AB417D97E96A76");

            entity.ToTable("QuanHuyen");

            entity.Property(e => e.MaQuan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaTinh)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenQuan).HasMaxLength(100);

            entity.HasOne(d => d.MaTinhNavigation).WithMany(p => p.QuanHuyens)
                .HasForeignKey(d => d.MaTinh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuanHuyen__MaTin__5629CD9C");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D3E1291DE");

            entity.ToTable("SanPham");

            entity.Property(e => e.AnhChiTiet).HasMaxLength(255);
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.TenSanPham).HasMaxLength(255);

            entity.HasOne(d => d.MaDongSanPhamNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDongSanPham)
                .HasConstraintName("FK__SanPham__MaDongS__1B0907CE");

            entity.HasOne(d => d.MaMauNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaMau)
                .HasConstraintName("FK__SanPham__MaMau__1BFD2C07");
        });

        modelBuilder.Entity<SanPhamSize>(entity =>
        {
            entity.HasKey(e => new { e.MaSanPham, e.MaSize }).HasName("PK__SanPhamS__30BF3A534F398B51");

            entity.ToTable("SanPhamSize");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.SanPhamSizes)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamSi__MaSan__1ED998B2");

            entity.HasOne(d => d.MaSizeNavigation).WithMany(p => p.SanPhamSizes)
                .HasForeignKey(d => d.MaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPhamSi__MaSiz__1FCDBCEB");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.MaSize).HasName("PK__Size__A787E7ED342A424D");

            entity.ToTable("Size");

            entity.Property(e => e.TenSize).HasMaxLength(50);
        });

        modelBuilder.Entity<Sodiachi>(entity =>
        {
            entity.HasKey(e => e.Masodiachi).HasName("PK__Sodiachi__91CCE4D714B050BC");

            entity.ToTable("Sodiachi");

            entity.Property(e => e.Diachi).HasMaxLength(255);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.MaPhuong)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaQuan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaTinh)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sdtnguoinhan)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Tennguoinhan).HasMaxLength(100);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.Sodiachis)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__Sodiachi__MaKhac__5CD6CB2B");

            entity.HasOne(d => d.MaPhuongNavigation).WithMany(p => p.Sodiachis)
                .HasForeignKey(d => d.MaPhuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sodiachi__MaPhuo__5DCAEF64");

            entity.HasOne(d => d.MaQuanNavigation).WithMany(p => p.Sodiachis)
                .HasForeignKey(d => d.MaQuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sodiachi__MaQuan__5EBF139D");

            entity.HasOne(d => d.MaTinhNavigation).WithMany(p => p.Sodiachis)
                .HasForeignKey(d => d.MaTinh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sodiachi__MaTinh__5FB337D6");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__TaiKhoan__A9D10535DD1ED0C6");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TinhThanh>(entity =>
        {
            entity.HasKey(e => e.MaTinh).HasName("PK__TinhThan__4CC544800DBED82F");

            entity.ToTable("TinhThanh");

            entity.Property(e => e.MaTinh)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenTinh).HasMaxLength(100);
        });

        modelBuilder.Entity<ViVoucher>(entity =>
        {
            entity.HasKey(e => e.MaLuuVoucher).HasName("PK__ViVouche__EA68C11355C9B0CE");

            entity.ToTable("ViVoucher");

            entity.Property(e => e.MaVoucher).HasMaxLength(20);
            entity.Property(e => e.NgayNhan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgaySuDung).HasColumnType("datetime");
            entity.Property(e => e.TrangThaiSuDung).HasDefaultValue(false);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.ViVouchers)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__ViVoucher__MaKha__38996AB5");

            entity.HasOne(d => d.MaPhieuMuaNavigation).WithMany(p => p.ViVouchers)
                .HasForeignKey(d => d.MaPhieuMua)
                .HasConstraintName("FK__ViVoucher__MaPhi__3A81B327");

            entity.HasOne(d => d.MaVoucherNavigation).WithMany(p => p.ViVouchers)
                .HasForeignKey(d => d.MaVoucher)
                .HasConstraintName("FK__ViVoucher__MaVou__398D8EEE");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.MaVoucher).HasName("PK__Voucher__0AAC5B11A28C2A8D");

            entity.ToTable("Voucher");

            entity.Property(e => e.MaVoucher).HasMaxLength(20);
            entity.Property(e => e.GiaTriToiThieu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiamToiDa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HangApDung).HasMaxLength(50);
            entity.Property(e => e.LoaiGiamGia).HasMaxLength(20);
        });

        modelBuilder.Entity<YeuThich>(entity =>
        {
            entity.HasKey(e => e.MaYeuThich).HasName("PK__YeuThich__B9007E4C4EEAD004");

            entity.ToTable("YeuThich");

            entity.HasIndex(e => new { e.Email, e.MaSanPham }, "UQ_YeuThich").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.YeuThiches)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__YeuThich__Email__71D1E811");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.YeuThiches)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__YeuThich__MaSanP__72C60C4A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
