using KarmaShop.Models;
using KarmaShop.Repository.PhieuThu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KarmaShop.Repositories
{
    public class OrderRepository : OrderInterface
    {
        private readonly QuanLyBanGiayContext _db;
        private IDbContextTransaction _transaction;

        public OrderRepository(QuanLyBanGiayContext db) => _db = db;

        public KhachHang GetKhachHangByEmail(string email) => _db.KhachHangs.FirstOrDefault(x => x.Email == email);
        public List<PhuongThucThanhToan> GetPhuongThucThanhToans() => _db.PhuongThucThanhToans.ToList();

        public PhieuMua? GetOrder(int id)
        {
            return _db.PhieuMuas
                .Include(p => p.ChiTietPhieuMuas)
                .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefault(p => p.MaPhieuMua == id);
        }

    
        public void BeginTransaction() => _transaction = _db.Database.BeginTransaction();
        public void CommitTransaction() => _transaction.Commit();
        public void RollbackTransaction() => _transaction.Rollback();

        public void SaveOrder(PhieuMua order, List<ChiTietPhieuMua> cartItems)
        {
            _db.PhieuMuas.Add(order);
            _db.SaveChanges();

            foreach (var item in cartItems)
            {
                var chiTiet = new ChiTietPhieuMua
                {
                    MaPhieuMua = order.MaPhieuMua,
                    MaSanPham = item.MaSanPham,
                    MaSize = item.MaSize,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };
                _db.ChiTietPhieuMuas.Add(chiTiet);

                var stock = _db.SanPhamSizes.FirstOrDefault(s => s.MaSanPham == item.MaSanPham && s.MaSize == item.MaSize);
                if (stock == null || stock.SoLuong < item.SoLuong)
                {
                    throw new Exception($"Sản phẩm mã {item.MaSanPham} không đủ hàng.");
                }
                stock.SoLuong -= item.SoLuong;
            }

            _db.SaveChanges();
        }
        public Sodiachi? GetDefaultAddress(int maKhachHang)
        {
            return _db.Sodiachis
                .Include(s => s.MaPhuongNavigation)
                .Include(s => s.MaQuanNavigation)
                .Include(s => s.MaTinhNavigation)
                .FirstOrDefault(s => s.MaKhachHang == maKhachHang && s.IsDefault == true);
        }
        public PhuongThucThanhToan? GetPhuongThucThanhToanById(int maPTTT)
        {
            return _db.PhuongThucThanhToans
                           .FirstOrDefault(p => p.MaPttt == maPTTT);
        }
    }
}