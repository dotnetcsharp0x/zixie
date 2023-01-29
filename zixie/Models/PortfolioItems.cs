namespace zixie.Models
{
    public class PortfolioItems
    {
        public int Id { get; set; }
        public int? Id_Portfolio { get; set; }
        public int? Instrument_Type { get; set; }
        public int? Id_Instrument { get; set; }
        public float? Count { get; set; }
        public float? Price { get; set; }
        public string? Date { get; set; }

    }
}
