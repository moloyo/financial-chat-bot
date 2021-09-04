using TinyCsvParser.Mapping;

namespace FinancialChatBot
{
    public class CsvStockMapping : CsvMapping<Stock>
    {
        public CsvStockMapping()
                : base()
        {
            MapProperty(0, x => x.Symbol);
            MapProperty(1, x => x.Date);
            MapProperty(2, x => x.Time);
            MapProperty(3, x => x.Open);
            MapProperty(4, x => x.High);
            MapProperty(5, x => x.Low);
            MapProperty(6, x => x.Close);
            MapProperty(7, x => x.Volume);
        }
    }
}
