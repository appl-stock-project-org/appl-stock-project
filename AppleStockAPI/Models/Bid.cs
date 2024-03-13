using System;
using System.Text.Json.Serialization;

namespace AppleStockAPI.Models {
    /// <summary>
    /// Class for a bid
    /// </summary>
	public record class Bid {

        [property: JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [property: JsonPropertyName("price")]
        public double Price { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        override public string ToString() {
            var message = $"Id: {this.Id} | CreatedAt: {this.CreatedAt} | Price: {this.Price} | Quantity: {this.Quantity}";
            return message;
        }        
    }
}