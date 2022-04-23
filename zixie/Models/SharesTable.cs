namespace zixie.Models
{
    public class SharesTable
    {
        //Ticker,share.Currency,share.DivYieldFlag,share.Exchange,share.Figi,share.IpoDate,share.Isin,share.BuyAvailableFlag,share.Name,share.Nominal,share.Sector
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ticker { get; set; }
        public string? Currency { get; set; }
        public float Price { get; set; }
        public string Figi { get; set; }
        public Watchlist? Watchlist { get; set; }
    }
}
