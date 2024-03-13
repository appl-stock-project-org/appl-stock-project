using System.Text.Json.Serialization;
namespace AppleStockAPI.Models {
    public record class StockPrice(
        // Last traded price is returned as a list of doubles with one value.
        [property: JsonPropertyName("last")] List<double> LastTradedPrice);
}