using System.Text.Json.Serialization;

namespace AppleStockAPI.Models
{
    /// <summary>
    /// Class for Trade objects
    /// </summary>
    public class Trade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [property: JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [property: JsonPropertyName("price")]
        public double Price { get; set; }
        [property: JsonPropertyName("tradeTime")]
        public DateTime TradeTime { get; set; } = DateTime.Now;
    }
}
