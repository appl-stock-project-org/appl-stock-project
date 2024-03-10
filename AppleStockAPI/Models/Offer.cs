/*
* Offer class including its quantity, price, id and time of the offer creation .
*/
using System.Text.Json.Serialization;


namespace AppleStockAPI.Models
{
    public class Offer
    {
         [property: JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [property: JsonPropertyName("price")]
        public double Price { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
