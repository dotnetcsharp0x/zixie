namespace zixie.Models
{
    public class CryptosTable
    {
        public int Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public float Price { get; set; }
        public float? VolumeYesterdayUSD { get; set; }
    }
}
