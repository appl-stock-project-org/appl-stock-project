using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using AppleStockAPI.Models;
using AppleStockAPI.Utilities;

/**
 * 
 * Handles and validates placed offers, stores them into offers list and handles the correct responses.
 * 
 */


namespace AppleStockAPI.Controllers

{
    public class OfferController
    {

        private List<Offer> offers;

        public OfferController()
        {
            offers = new List<Offer>();
        }

        // Returns all the offers from the offers list
        public List<Offer> GetOffers()
        {
            return offers;
        }

        /// <summary>
        /// Returns all of the offers that have a smaller or the same price as the given price, in ascending order by price and date
        /// </summary>
        /// <param name="price"></param>
        /// <returns>List of orders that match the given price</returns>
        public List<Offer> GetValidOffers(double price)
        {

            List<Offer> validOffers = offers.FindAll(offer => offer.Price <= price);

            return validOffers.OrderBy(offer => offer.Price).ThenBy(offer => offer.CreatedAt).ToList();
        }

        // Clears all offers from the offers list
        public void ClearOffers()
        {
           offers.Clear();
        }

        // Formats the offer price accordingly and calls to check the offer.
        public void ValidateOffer(Offer offer, double currentStockPrice)
        {
            offer.Price = MathUtils.TruncateTo2Decimals(offer.Price);

            if (!CheckOfferPrice(offer.Price, currentStockPrice) && !CheckOfferQuantity(offer.Quantity))
            {
                throw new Exception("Something went terribly wrong, offer quantity AND offer price were invalid");
            }
            else if (!CheckOfferQuantity(offer.Quantity))
            {
                throw new Exception("Offer quantity invalid, offer should contain a quantity of larger than 0");
            }
            else if (!CheckOfferPrice(offer.Price, currentStockPrice))
            {
                throw new Exception($"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price. Current market price is {currentStockPrice}.");
            }

            return;
        }

        // Fetches an offer from the offers list based on its id
        public Offer? GetOffer(Guid id)
        {
            return offers.Find(offer => offer.Id == id);
        }

        /// <summary>
        /// Adds an offer to the offers list
        /// </summary>
        /// <param name="offer">Offer to add to the list</param>
        public void AddOffer(Offer offer)
        {
            offers.Add(offer);
        }

        // Removes an offer from the offers list based on its id
        public void RemoveOffer(Guid id)
        {
            Offer? removableOffer = GetOffer(id);
            if (removableOffer != null)
            {
                offers.Remove(removableOffer);
            }
            return;
        }

        // Checks that offer has a quantity larger than 0
        public bool CheckOfferQuantity(int offerQuantity)
        {
            return offerQuantity > 0;
        }

        // Checks that the offer has a valid price
        public bool CheckOfferPrice(double offerPrice, double stockPrice)
        {
            double highestValid = MathUtils.TruncateTo2Decimals(stockPrice * 1.1);
            double lowestValid = MathUtils.TruncateTo2Decimals(stockPrice * 0.9);

            return offerPrice <= highestValid && offerPrice >= lowestValid;
        }
    }
}

