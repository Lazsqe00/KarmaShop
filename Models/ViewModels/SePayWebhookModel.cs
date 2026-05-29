namespace KarmaShop.Models.ViewModels
{
    public class SePayWebhookModel
    {
        public long id { get; set; }
        public string gateway { get; set; } = string.Empty;
        public string transactionDate { get; set; } = string.Empty;
        public string accountName { get; set; } = string.Empty;
        public string accountNumber { get; set; } = string.Empty;
        public decimal transferAmount { get; set; }
        public string content { get; set; } = string.Empty;
        public string referenceCode { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string TransferContent => content;
    }
}
