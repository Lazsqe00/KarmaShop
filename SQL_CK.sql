IF DB_ID(N'QuanLyBanGiay') IS NOT NULL
    DROP DATABASE QuanLyBanGiay;
GO

CREATE DATABASE QuanLyBanGiay;
GO

USE QuanLyBanGiay;
GO

/* ===============================
   VOUCHER
   =============================== */
CREATE TABLE Voucher (
    MaVoucher NVARCHAR(20) PRIMARY KEY, 
    SoLuong INT NOT NULL,
    GiamToiDa DECIMAL(5,2), 
    NgayTao DATE,
    NgayHetHan DATE
);

/* ===============================
   SIZE
   =============================== */
CREATE TABLE Size (
    MaSize INT IDENTITY PRIMARY KEY,
    TenSize NVARCHAR(50)
);

/* ===============================
   MAU
   =============================== */
CREATE TABLE Mau (
    MaMau INT IDENTITY PRIMARY KEY,
    TenMau NVARCHAR(50)
);

/* ===============================
   LOAI:  
   =============================== */
CREATE TABLE Loai (
    MaLoai INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(100)
);

/* ===============================
   DONG SAN PHAM
   =============================== */
CREATE TABLE DongSanPham (
    MaDongSanPham INT IDENTITY PRIMARY KEY,
    MaLoai INT,
    TenDongSanPham NVARCHAR(255),
    GiaBan DECIMAL(18,2),
    MoTa NVARCHAR(MAX),
    FOREIGN KEY (MaLoai) REFERENCES Loai(MaLoai)
);

/* ===============================
   SAN PHAM
   =============================== */
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

/* ===============================
   SAN PHAM - SIZE
   =============================== */
CREATE TABLE SanPhamSize (
    MaSanPham INT,
    MaSize INT,
    SoLuong INT,
    PRIMARY KEY (MaSanPham, MaSize),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaSize) REFERENCES Size(MaSize)
);

/* ===============================
   TAI KHOAN
   =============================== */
CREATE TABLE TaiKhoan (
    Email VARCHAR(100) PRIMARY KEY,
    MatKhau VARCHAR(255),
    LoaiTaiKhoan INT
);

/* ===============================
   KHACH HANG
   =============================== */
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


/* ===============================
   NHAN VIEN
   =============================== */
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

/* ===============================
   PHUONG THUC THANH TOAN
   =============================== */
CREATE TABLE PhuongThucThanhToan (
    MaPTTT INT IDENTITY PRIMARY KEY,
    TenPTTT NVARCHAR(100) NOT NULL     
);

/* ===============================
   GIO HANG
   =============================== */
CREATE TABLE PhieuMua (
    MaPhieuMua INT IDENTITY PRIMARY KEY,
    NgayDat DATE,
    MaKhachHang INT,
    MaNhanVien INT,   
	MaPTTT INT,
    MaVoucher NVARCHAR(20),
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

/* ===============================
   CHI TIET GIO HANG
   =============================== */
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


/* ===============================
   VOUCHER_KHACHHANG 
   =============================== */
   CREATE TABLE ViVoucher (
    MaLuuVoucher INT IDENTITY PRIMARY KEY, 
    MaKhachHang INT,
    MaVoucher NVARCHAR(20),
    TrangThaiSuDung BIT DEFAULT 0, 
    NgayNhan DATETIME DEFAULT GETDATE(),
    NgaySuDung DATETIME NULL, 
    MaPhieuMua INT NULL, 
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaVoucher) REFERENCES Voucher(MaVoucher),
    FOREIGN KEY (MaPhieuMua) REFERENCES PhieuMua(MaPhieuMua) 
);


/* ===============================
   CHÈN DỮ LIỆU TÀI KHOẢN
   =============================== */
INSERT INTO TaiKhoan (Email, MatKhau, LoaiTaiKhoan) VALUES
-- 20 khách hàng đầu tiên (LoaiTaiKhoan = 0)
('vuvannam@gmail.com', '123456', 0),
('nguyenthihanh@gmail.com', '123456', 0),
('buivanthang@gmail.com', '123456', 0),
('phamthingoc@gmail.com', '123456', 0),
('dovancuong@gmail.com', '123456', 0),
('luuthihoa@gmail.com', '123456', 0),
('nguyenvanson@gmail.com', '123456', 0),
('trinhthimai@gmail.com', '123456', 0),
('hoangvanphu@gmail.com', '123456', 0),
('phanthituyet@gmail.com', '123456', 0),
('ngovanminh@gmail.com', '123456', 0),
('lethihuong@gmail.com', '123456', 0),
('dangvantoan@gmail.com', '123456', 0),
('nguyenthitrang@gmail.com', '123456', 0),
('phamvantai@gmail.com', '123456', 0),
('dothiloan@gmail.com', '123456', 0),
('vovanduc@gmail.com', '123456', 0),
('nguyenthivan@gmail.com', '123456', 0),
('tranvanhai@gmail.com', '123456', 0),
('lethinhung@gmail.com', '123456', 0);
GO

INSERT INTO TaiKhoan (Email, MatKhau, LoaiTaiKhoan) VALUES
-- 10 nhân viên (LoaiTaiKhoan = 1)
('nguyenvanhung@gmail.com', 'nvh123', 1),
('tranthimai@gmail.com', 'ttm123', 1),
('levancuong@gmail.com', 'lvc123', 1),
('phamthilan@gmail.com', 'ptl123', 1),
('hoangvantuan@gmail.com', 'hvt123', 1),
('dothihuong@gmail.com', 'dth123', 1),
('vuvanminh@gmail.com', 'vvm123', 1),
('nguyenthingoc@gmail.com', 'ntn123', 1),
('buivanphuc@gmail.com', 'bvp123', 1),
('lethithuy@gmail.com', 'ltt123', 1);
GO

INSERT INTO TaiKhoan (Email, MatKhau, LoaiTaiKhoan) VALUES
('admin@gmail.com', '123', 2);
GO

INSERT INTO TaiKhoan (Email, MatKhau, LoaiTaiKhoan) VALUES
-- 20 khách hàng tiếp theo (LoaiTaiKhoan = 0)
('nguyenvanbinh@gmail.com', '123456', 0),
('tranthihuyen@gmail.com', '123456', 0),
('phamvandung@gmail.com', '123456', 0),
('lethithu@gmail.com', '123456', 0),
('hoangvankhanh@gmail.com', '123456', 0),
('nguyenthioanh@gmail.com', '123456', 0),
('dovanhung@gmail.com', '123456', 0),
('vuthithao@gmail.com', '123456', 0),
('nguyenvankhoa@gmail.com', '123456', 0),
('phanthinga@gmail.com', '123456', 0),
('levanphuong@gmail.com', '123456', 0),
('tranthily@gmail.com', '123456', 0),
('nguyenvanhieu@gmail.com', '123456', 0),
('buithiphuong@gmail.com', '123456', 0),
('dangvanlam@gmail.com', '123456', 0),
('nguyenthimaianh@gmail.com', '123456', 0),
('phamvanquang@gmail.com', '123456', 0),
('hoangthivan@gmail.com', '123456', 0),
('nguyenvanphat@gmail.com', '123456', 0),
('lethidiep@gmail.com', '123456', 0);
GO

/* ===============================
   CHÈN DỮ LIỆU KHÁCH HÀNG
   =============================== */
INSERT INTO KhachHang (TenKhachHang, Email, SoDienThoai, DiaChi, GioiTinh, NgaySinh, TongChi)
VALUES
(N'Vũ Văn Nam', 'vuvannam@gmail.com', '0972222222', N'Hà Nội', N'Nam', '1994-04-14', 4500000),
(N'Nguyễn Thị Hạnh', 'nguyenthihanh@gmail.com', '0973333333', N'TP.HCM', N'Nữ', '1998-10-10', 7200000),
(N'Bùi Văn Thắng', 'buivanthang@gmail.com', '0974444444', N'Hải Phòng', N'Nam', '1993-03-03', 9000000),
(N'Phạm Thị Ngọc', 'phamthingoc@gmail.com', '0975555555', N'Nam Định', N'Nữ', '1999-12-12', 3800000),
(N'Đỗ Văn Cường', 'dovancuong@gmail.com', '0976666666', N'Hà Nam', N'Nam', '1995-06-06', 6100000),
(N'Lưu Thị Hòa', 'luuthihoa@gmail.com', '0977777777', N'Thanh Hóa', N'Nữ', '1997-07-07', 5300000),
(N'Nguyễn Văn Sơn', 'nguyenvanson@gmail.com', '0978888888', N'Bình Thuận', N'Nam', '1992-08-08', 8200000),
(N'Trịnh Thị Mai', 'trinhthimai@gmail.com', '0979999999', N'Nghệ An', N'Nữ', '1996-09-09', 4700000),
(N'Hoàng Văn Phú', 'hoangvanphu@gmail.com', '0981111111', N'Hà Tĩnh', N'Nam', '1991-01-01', 9500000),
(N'Phan Thị Tuyết', 'phanthituyet@gmail.com', '0982222222', N'Lâm Đồng', N'Nữ', '1998-02-02', 5600000),
(N'Ngô Văn Minh', 'ngovanminh@gmail.com', '0983333333', N'TP.HCM', N'Nam', '1995-03-03', 7400000),
(N'Lê Thị Hương', 'lethihuong@gmail.com', '0984444444', N'Hà Nội', N'Nữ', '1997-04-04', 6200000),
(N'Đặng Văn Toàn', 'dangvantoan@gmail.com', '0985555555', N'Bắc Ninh', N'Nam', '1994-05-05', 8300000),
(N'Nguyễn Thị Trang', 'nguyenthitrang@gmail.com', '0986666666', N'Vĩnh Phúc', N'Nữ', '1999-06-06', 3900000),
(N'Phạm Văn Tài', 'phamvantai@gmail.com', '0987777777', N'Đồng Nai', N'Nam', '1993-07-07', 8800000),
(N'Đỗ Thị Loan', 'dothiloan@gmail.com', '0988888888', N'Hưng Yên', N'Nữ', '1996-08-08', 5200000),
(N'Võ Văn Đức', 'vovanduc@gmail.com', '0989999999', N'Quảng Ngãi', N'Nam', '1992-09-09', 9100000),
(N'Nguyễn Thị Vân', 'nguyenthivan@gmail.com', '0991111111', N'Hà Nam', N'Nữ', '1998-11-11', 4800000),
(N'Trần Văn Hải', 'tranvanhai@gmail.com', '0992222222', N'Bình Định', N'Nam', '1995-12-12', 6700000),
(N'Lê Thị Nhung', 'lethinhung@gmail.com', '0993333333', N'Thái Bình', N'Nữ', '1997-01-15', 5900000);
GO

INSERT INTO KhachHang (TenKhachHang, Email, SoDienThoai, DiaChi, GioiTinh, NgaySinh, TongChi)
VALUES
(N'Nguyễn Văn Bình', 'nguyenvanbinh@gmail.com', '0994444444', N'Hà Nội', N'Nam', '1993-02-10', 7200000),
(N'Trần Thị Huyền', 'tranthihuyen@gmail.com', '0995555555', N'TP.HCM', N'Nữ', '1998-03-18', 5100000),
(N'Phạm Văn Dũng', 'phamvandung@gmail.com', '0996666666', N'Bắc Giang', N'Nam', '1991-06-06', 8600000),
(N'Lê Thị Thu', 'lethithu@gmail.com', '0997777777', N'Hà Nam', N'Nữ', '1999-09-09', 3900000),
(N'Hoàng Văn Khánh', 'hoangvankhanh@gmail.com', '0998888888', N'Nam Định', N'Nam', '1994-11-11', 6400000),
(N'Nguyễn Thị Oanh', 'nguyenthioanh@gmail.com', '0999999999', N'Nghệ An', N'Nữ', '1997-01-20', 5800000),
(N'Đỗ Văn Hùng', 'dovanhung@gmail.com', '0901111111', N'Quảng Bình', N'Nam', '1992-04-04', 9100000),
(N'Vũ Thị Thảo', 'vuthithao@gmail.com', '0902222222', N'Thanh Hóa', N'Nữ', '1996-05-05', 4700000),
(N'Nguyễn Văn Khoa', 'nguyenvankhoa@gmail.com', '0903333333', N'Bình Định', N'Nam', '1995-07-07', 6900000),
(N'Phan Thị Nga', 'phanthinga@gmail.com', '0904444444', N'Hải Dương', N'Nữ', '1998-08-08', 5200000),
(N'Lê Văn Phương', 'levanphuong@gmail.com', '0905555555', N'Hà Nội', N'Nam', '1990-10-10', 9800000),
(N'Trần Thị Lý', 'tranthily@gmail.com', '0906666666', N'Quảng Nam', N'Nữ', '1997-12-12', 4600000),
(N'Nguyễn Văn Hiếu', 'nguyenvanhieu@gmail.com', '0907777777', N'TP.HCM', N'Nam', '1994-03-03', 7300000),
(N'Bùi Thị Phương', 'buithiphuong@gmail.com', '0908888888', N'Phú Thọ', N'Nữ', '1999-06-15', 4100000),
(N'Đặng Văn Lâm', 'dangvanlam@gmail.com', '0909999999', N'Bắc Ninh', N'Nam', '1993-09-09', 8500000),
(N'Nguyễn Thị Mai Anh', 'nguyenthimaianh@gmail.com', '0911111111', N'Hà Nội', N'Nữ', '1998-02-22', 5600000),
(N'Phạm Văn Quang', 'phamvanquang@gmail.com', '0912222222', N'Hải Phòng', N'Nam', '1992-05-25', 9000000),
(N'Hoàng Thị Vân', 'hoangthivan@gmail.com', '0913333333', N'Thái Nguyên', N'Nữ', '1996-07-30', 4900000),
(N'Nguyễn Văn Phát', 'nguyenvanphat@gmail.com', '0914444444', N'Long An', N'Nam', '1995-08-08', 7700000),
(N'Lê Thị Diệp', 'lethidiep@gmail.com', '0915555555', N'Quảng Trị', N'Nữ', '1999-11-11', 4300000);
GO

/* ===============================
   CHÈN DỮ LIỆU NHÂN VIÊN
   =============================== */
INSERT INTO NhanVien (TenNhanVien, Email, DiaChi, SoDienThoai, GioiTinh, NgaySinh)
VALUES
(N'Nguyễn Văn Hùng', 'nguyenvanhung@gmail.com', N'Hà Nội', '0901112233', N'Nam', '1990-01-15'),
(N'Trần Thị Mai', 'tranthimai@gmail.com', N'TP.HCM', '0902223344', N'Nữ', '1992-03-22'),
(N'Lê Văn Cường', 'levancuong@gmail.com', N'Đà Nẵng', '0903334455', N'Nam', '1988-07-10'),
(N'Phạm Thị Lan', 'phamthilan@gmail.com', N'Hải Phòng', '0904445566', N'Nữ', '1995-11-05'),
(N'Hoàng Văn Tuấn', 'hoangvantuan@gmail.com', N'Cần Thơ', '0905556677', N'Nam', '1991-04-18'),
(N'Đỗ Thị Hương', 'dothihuong@gmail.com', N'Bắc Ninh', '0906667788', N'Nữ', '1993-09-30'),
(N'Vũ Văn Minh', 'vuvanminh@gmail.com', N'Nghệ An', '0907778899', N'Nam', '1989-06-12'),
(N'Nguyễn Thị Ngọc', 'nguyenthingoc@gmail.com', N'Thái Bình', '0908889900', N'Nữ', '1994-02-25'),
(N'Bùi Văn Phúc', 'buivanphuc@gmail.com', N'Hà Nam', '0910001122', N'Nam', '1992-08-08'),
(N'Lê Thị Thúy', 'lethithuy@gmail.com', N'Quảng Ninh', '0911112233', N'Nữ', '1996-12-20');
GO


INSERT INTO PhuongThucThanhToan (TenPTTT) VALUES
(N'COD'),
(N'VNPAY'),
(N'MoMo'),
(N'Chuyển khoản');




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
    
    -- Không cho phép thêm trùng lặp
    CONSTRAINT UQ_YeuThich UNIQUE (Email, MaSanPham)
);