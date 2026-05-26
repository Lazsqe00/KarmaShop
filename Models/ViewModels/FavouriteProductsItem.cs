namespace KarmaShop.Models.ViewModels
{
    public class FavouriteProductsItem
    {
        public int Id { get; set; }                   
        public int Madongsp { get; set; }
        public string Tensp { get; set; } = "";
        public string Hinhanh { get; set; } = "";
        public decimal Gia { get; set; }
        public decimal? Phantramgiam { get; set; }
    }
}