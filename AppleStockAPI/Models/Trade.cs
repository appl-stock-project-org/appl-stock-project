namespace AppleStockAPI.Models
{
    /// <summary>
    /// Class for Trade objects
    /// </summary>
    public class Trade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
        public double Price { get; set; }

        public DateTime TradeTime { get; set; } = DateTime.Now;
    }
}
