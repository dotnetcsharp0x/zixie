namespace zixie.Models
{
    public class Crypto
    {
        public int Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Blockchain { get; set; }
        public float? Price { get; set; }
        public float? PriceYesterday { get; set; }
        public float? VolumeYesterdayUSD { get; set; }
        public string? Time { get; set; }
        public string? Source { get; set; }
    }
}
