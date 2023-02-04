namespace zixie.Models
{
    public class PortfolioItemsmodel
    {
        public int Id { get; set; }
        public int? Id_Portfolio { get; set; }
        public int? Instrument_Type { get; set; }
        public int? Id_Instrument { get; set; }
        public string? NameInstrument { get; set; }
        public string? Ticker { get; set; }
        public string? Currency { get; set; }
        public float? Count { get; set; }
        public float? Price { get; set; }
        public float? CuurentPrice { get; set; }
        public string? Figi { get; set; }
        public string? Date { get; set; }

    }
}
