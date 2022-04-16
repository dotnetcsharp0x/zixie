namespace zixie.Models
{
    public class Shares
    {
        //Ticker,share.Currency,share.DivYieldFlag,share.Exchange,share.Figi,share.IpoDate,share.Isin,share.BuyAvailableFlag,share.Name,share.Nominal,share.Sector
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ticker { get; set; }
        public string? Currency { get; set; }
        public bool? DivYieldFlag { get; set; }
        public string? Exchange { get; set; }
        public string? Figi { get; set; }
        //public string? IpoDate { get; set; }
        public string? Isin { get; set; }
        public bool? BuyAvailableFlag { get; set; }
        public string? Nominal { get; set; }
        public string? Sector { get; set; }
    }
}
