using System;

namespace AppleStockAPI.Models {
    /// <summary>
    /// Class for a bid
    /// </summary>
	public class Bid {

        public int Quantity { get; set; }
        public double Price { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        override public string ToString() {
            var message = $"Id: {this.Id} | CreatedAt: {this.CreatedAt.ToString()} | Price: {this.Price} | Quantity: {this.Quantity}";
            return message;
        }        
    }
}