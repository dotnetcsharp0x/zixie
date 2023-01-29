namespace zixie.Models
{
    public class Shares
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ticker { get; set; }
        public string? Currency { get; set; }
        public int? DivYieldFlag { get; set; }
        public string? Exchange { get; set; }
        public string? Figi { get; set; }
        public string? Isin { get; set; }
        public int? BuyAvailableFlag { get; set; }
        public string? Nominal { get; set; }
        public string? Sector { get; set; }
    }
}
