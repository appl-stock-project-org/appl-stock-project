using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using AppleStockAPI.Models;

/**
 * 
 * Handles and validates place offers and its responses.
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

        public List<Offer> GetOffers()
        {
            return offers;
        }

        public void ClearOffers()
        {
           offers.Clear();
        }

        // Offer response handling, creates the response, formats the offer price accordingly and calls to check the offer.
        public static Response HandleOffer(Offer offer, List<Offer> Offers)
        {

            Response response = new Response();
            offer.Price = Math.Truncate(offer.Price * 100) / 100;
            Console.WriteLine(offer.Price);

            CheckOffer(offer, response);

            return response;

        }


        // Checks the offer price and quantity validity, and sets the correct response message.
        public static bool CheckOffer(Offer offer, Response response)
        {
            if (CheckOfferQuantity(offer.Quantity) && CheckOfferPrice(offer.Price))
            {
                response.Success = true;
                response.SuccessMessage = $"Offer succesful with the price of {offer.Price} and quantity of {offer.Quantity}";
            }
            else if (!CheckOfferPrice(offer.Price) && !CheckOfferQuantity(offer.Quantity))
            {
                response.Success = false;
                response.ErrorMessage = "Something went terribly wrong, offer quantity AND offer price were invalid";
            }
            else if (!CheckOfferQuantity(offer.Quantity))
            {
                response.Success = false;
                response.ErrorMessage = $"Offer quantity invalid, offer should contain a quantity of larger than 0";
            }
            else if (!CheckOfferPrice(offer.Price))
            {
                response.Success = false;
                response.ErrorMessage = $"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price";
            }

            return response.Success;
        }


        // Checks that offer has a quantity larger than 0
        public static bool CheckOfferQuantity(int offerQuantity)
        {
            return offerQuantity > 0;
        }

        // Checks that the offer has a valid price
        public static bool CheckOfferPrice(double offerPrice)
        {
            const double MOCKPRICE = 100;
            double highestValid = Math.Round(MOCKPRICE * 1.1, 2);
            double lowestValid = Math.Round(MOCKPRICE * 0.9, 2);

            return (offerPrice <= highestValid && offerPrice >= lowestValid);
        }

    }
}

