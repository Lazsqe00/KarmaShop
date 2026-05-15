using KarmaShop.Models;

namespace KarmaShop.Repository.PhieuThu
{
    public interface VoucherInterface
    {
        List<Voucher> GetActiveVouchers();
        Voucher GetVoucherByCode(string maVoucher);
        void GiamSoLuongVoucher(string maVoucher);
    }
}
