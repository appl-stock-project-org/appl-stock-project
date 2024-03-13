using AppleStockAPI.Models;
using System;
using static System.DateTime;

namespace AppleStockAPI.Controllers
{
    /// <summary>
    /// Class to handle trades
    /// </summary>
    public class TradeController
    {
        private List<Trade> trades;

        public TradeController() 
        {
            trades = new List<Trade>();
        }

        /// <summary>
        /// Function to save a record of a trade. Price and quantity must be positive
        /// </summary>
        /// <param name="price">Price of the trade</param>
        /// <param name="quantity">Quatity of traded stock</param>
        public void RecordTrade(double price, int quantity) 
        {
            if (price <= 0 || quantity <= 0) return;
            var trade = new Trade()
            {
                Price = price,
                Quantity = quantity,
            };

            trades.Add(trade);
        }

        /// <summary>
        /// Function to fetch all recorded trades
        /// </summary>
        /// <returns>List of recorded trades</returns>
        public List<Trade> GetTrades() 
        {  
            return trades;
        }

        public List<Trade> ListTrades()
        {
            trades.Sort(delegate(Trade a, Trade b)
            {
                if (a.TradeTime < b.TradeTime) return -1;
                return 1;
            });

            return trades;
        }

        /// <summary>
        /// Function to empty the trades list
        /// </summary>
        public void ClearTrades()
        {
            trades.Clear();
        }
    }
}
