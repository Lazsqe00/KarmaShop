using KarmaShop.Models;

namespace KarmaShop.Repository.PhieuThu
{
    public class VoucherRepository: VoucherInterface
    {
        private readonly QuanLyBanGiayContext _db;
        public VoucherRepository(QuanLyBanGiayContext db) => _db = db;

        public List<Voucher> GetActiveVouchers()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return _db.Vouchers
                .Where(v => v.NgayTao <= today && v.NgayHetHan >= today && v.SoLuong > 0)
                .ToList();
        }

        public Voucher GetVoucherByCode(string maVoucher)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return _db.Vouchers.FirstOrDefault(v => v.MaVoucher == maVoucher
                && v.NgayTao <= today && v.NgayHetHan >= today && v.SoLuong > 0);
        }

        public void GiamSoLuongVoucher(string maVoucher)
        {
            var v = _db.Vouchers.FirstOrDefault(x => x.MaVoucher == maVoucher);
            if (v != null && v.SoLuong > 0)
            {
                v.SoLuong -= 1;
            }
        }
    }
}

