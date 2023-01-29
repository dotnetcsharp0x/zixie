namespace zixie.Models
{
    public class SharesTable
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ticker { get; set; }
        public string? Currency { get; set; }
        public float Price { get; set; }
        public string Figi { get; set; }
        public Watchlist? Watchlist { get; set; }
    }
}
