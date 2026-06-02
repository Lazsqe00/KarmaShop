IF DB_ID(N'QuanLyBanGiay') IS NOT NULL
    DROP DATABASE QuanLyBanGiay;
GO

CREATE DATABASE QuanLyBanGiay;
GO

USE QuanLyBanGiay;
GO

CREATE TABLE Voucher (
    MaVoucher VARCHAR(50) NOT NULL PRIMARY KEY,
    SoLuong INT NOT NULL,
    LoaiGiamGia NVARCHAR(20) NULL, 
    HangApDung NVARCHAR(50) NULL,  
    GiamToiDa DECIMAL(18, 2) NOT NULL,
    GiaTriToiThieu DECIMAL(18, 2) NOT NULL DEFAULT 0,
    NgayTao DATE NOT NULL,
    NgayHetHan DATE NOT NULL
);

CREATE TABLE Size (
    MaSize INT IDENTITY PRIMARY KEY,
    TenSize NVARCHAR(50)
);

CREATE TABLE Mau (
    MaMau INT IDENTITY PRIMARY KEY,
    TenMau NVARCHAR(50)
);

CREATE TABLE Loai (
    MaLoai INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(100)
);

CREATE TABLE DongSanPham (
    MaDongSanPham INT IDENTITY PRIMARY KEY,
    MaLoai INT,
    TenDongSanPham NVARCHAR(255),
    GiaBan DECIMAL(18,2),
    MoTa NVARCHAR(MAX),
    FOREIGN KEY (MaLoai) REFERENCES Loai(MaLoai)
);

CREATE TABLE SanPham (
    MaSanPham INT IDENTITY PRIMARY KEY,
    MaDongSanPham INT,
    TenSanPham NVARCHAR(255),
    MaMau INT,
    AnhDaiDien NVARCHAR(255),
    AnhChiTiet NVARCHAR(255),
    TrangThai BIT,
    FOREIGN KEY (MaDongSanPham) REFERENCES DongSanPham(MaDongSanPham),
    FOREIGN KEY (MaMau) REFERENCES Mau(MaMau)
);

CREATE TABLE SanPhamSize (
    MaSanPham INT,
    MaSize INT,
    SoLuong INT,
    PRIMARY KEY (MaSanPham, MaSize),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaSize) REFERENCES Size(MaSize)
);

CREATE TABLE TaiKhoan (
    Email VARCHAR(100) PRIMARY KEY,
    MatKhau VARCHAR(255),
    LoaiTaiKhoan INT
);

CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY PRIMARY KEY,
    TenKhachHang NVARCHAR(100),
    Email VARCHAR(100),
    SoDienThoai VARCHAR(15),
    DiaChi NVARCHAR(255),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    TongChi DECIMAL(18,2),
    FOREIGN KEY (Email) REFERENCES TaiKhoan(Email)
);

CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY PRIMARY KEY,
    TenNhanVien NVARCHAR(100),
    Email VARCHAR(100),
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(15),
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    FOREIGN KEY (Email) REFERENCES TaiKhoan(Email)
);

CREATE TABLE PhuongThucThanhToan (
    MaPTTT INT IDENTITY PRIMARY KEY,
    TenPTTT NVARCHAR(100) NOT NULL     
);

CREATE TABLE PhieuMua (
    MaPhieuMua INT IDENTITY PRIMARY KEY,
    NgayDat DATE,
    MaKhachHang INT,
    MaNhanVien INT,   
    MaPTTT INT,
    MaVoucher VARCHAR(50),
    TinhTrang NVARCHAR(50),
    GhiChu NVARCHAR(MAX),
    TongTien DECIMAL(18,2),
    DiaChiGiaoHang NVARCHAR(255),
    EmailNguoiNhan VARCHAR(100),
    SoDienThoaiNguoiNhan VARCHAR(15),
    TenNguoiNhan NVARCHAR(100),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaVoucher) REFERENCES Voucher(MaVoucher),
    FOREIGN KEY (MaPTTT) REFERENCES PhuongThucThanhToan(MaPTTT)
);

CREATE TABLE ChiTietPhieuMua (
    MaPhieuMua INT,
    MaSanPham INT,
    MaSize INT,
    SoLuong INT,
    DonGia DECIMAL(18,2),
    PRIMARY KEY (MaPhieuMua, MaSanPham, MaSize),
    FOREIGN KEY (MaPhieuMua) REFERENCES PhieuMua(MaPhieuMua),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaSize) REFERENCES Size(MaSize)
);

CREATE TABLE ViVoucher (
    MaLuuVoucher INT IDENTITY PRIMARY KEY, 
    MaKhachHang INT,
    MaVoucher VARCHAR(50),
    TrangThaiSuDung BIT DEFAULT 0, 
    NgayNhan DATETIME DEFAULT GETDATE(),
    NgaySuDung DATETIME NULL, 
    MaPhieuMua INT NULL, 
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaVoucher) REFERENCES Voucher(MaVoucher),
    FOREIGN KEY (MaPhieuMua) REFERENCES PhieuMua(MaPhieuMua) 
);

CREATE TABLE TinhThanh (
    MaTinh VARCHAR(20) PRIMARY KEY,
    TenTinh NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE QuanHuyen (
    MaQuan VARCHAR(20) PRIMARY KEY,
    TenQuan NVARCHAR(100) NOT NULL,
    MaTinh VARCHAR(20) NOT NULL,
    FOREIGN KEY (MaTinh) REFERENCES TinhThanh(MaTinh)
);
GO

CREATE TABLE PhuongXa (
    MaPhuong VARCHAR(20) PRIMARY KEY,
    TenPhuong NVARCHAR(100) NOT NULL,
    MaQuan VARCHAR(20) NOT NULL,
    FOREIGN KEY (MaQuan) REFERENCES QuanHuyen(MaQuan)
);
GO

CREATE TABLE Sodiachi (
    Masodiachi INT IDENTITY PRIMARY KEY,
    MaKhachHang INT NOT NULL,
    Tennguoinhan NVARCHAR(100) NOT NULL,
    Sdtnguoinhan VARCHAR(15) NOT NULL,
    Diachi NVARCHAR(255) NOT NULL,      
    MaPhuong VARCHAR(20) NOT NULL,      
    MaQuan VARCHAR(20) NOT NULL,        
    MaTinh VARCHAR(20) NOT NULL,        
    IsDefault BIT DEFAULT 0,            
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang) ON DELETE CASCADE,
    FOREIGN KEY (MaPhuong) REFERENCES PhuongXa(MaPhuong),
    FOREIGN KEY (MaQuan) REFERENCES QuanHuyen(MaQuan),
    FOREIGN KEY (MaTinh) REFERENCES TinhThanh(MaTinh)
);
GO

CREATE TABLE YeuThich (
    MaYeuThich INT IDENTITY(1,1) PRIMARY KEY,
    Email VARCHAR(100) NOT NULL,
    MaSanPham INT NOT NULL,
    NgayThem DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Email) REFERENCES TaiKhoan(Email),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    CONSTRAINT UQ_YeuThich UNIQUE (Email, MaSanPham)
);
GO
