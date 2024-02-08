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

    // !!!THIS IS DUMMY!!!
    public class DummyProgram
    {

        /// <summary>
        /// Receives percent as an integer. Returns the course grade as a string.
        /// krumburum
        /// V07022024
        /// <param name="percentage">the percentage to be entered</param>
        /// <returns>
        /// Returns a grade (as string) based on percentage
        /// </returns>
        /// </summary>
        public string GetGradesByPercentage(int percentage)
        {

            if (percentage >= 90 && percentage <= 100)
                return "5";
            else if (percentage >= 80 && percentage < 90)
                return "4";
            else if (percentage >= 70 && percentage < 80)
                return "3";
            else if (percentage >= 60 && percentage < 70)
                return "2";
            else if (percentage >= 50 && percentage < 60)
                return "1";
            else if (percentage >= 0 && percentage < 50)
                return "Failed";
            else
                return "Invalid";

        }

    }
    // !!!THIS IS DUMMY!!!
}
