/*
* Offer class including its quantity, price, id and time of the offer creation .
*/
using AppleStockAPI.Models;
using System;


namespace AppleStockAPI.Models
{
    public class Offer
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OfferCreationTime { get; set; } = DateTime.Now;
    }

}
