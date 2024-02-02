using Microsoft.EntityFrameworkCore;

namespace AppleStockAPI.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string? Price { get; set; }
    }


    class StockDb : DbContext
    {
        public StockDb(DbContextOptions options) : base(options) { }
        public DbSet<Stock> Stocks { get; set; } = null!;
    }
}
