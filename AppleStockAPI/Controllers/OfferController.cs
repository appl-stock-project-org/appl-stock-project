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
        // Offer response handling, creates the response, formats the offer price accordingly and calls to check the offer.
        public static Response handleOffer(Offer offer, List<Offer> Offers)
        {

            Response response = new Response();
            offer.Price = Math.Truncate(offer.Price * 100) / 100;
            Console.WriteLine(offer.Price);

            checkOffer(offer, response);

            return response;

        }


        // Checks the offer price and quantity validity, and sets the correct response message.
        public static bool checkOffer(Offer offer, Response response)
        {
            if (checkOfferQuantity(offer.Quantity) && checkOfferPrice(offer.Price))
            {
                response.Success = true;
                response.SuccessMessage = $"Offer succesful with the price of {offer.Price}, and quantity of {offer.Quantity}";
            }
            else if (!checkOfferPrice(offer.Price) && !checkOfferQuantity(offer.Quantity))
            {
                response.Success = false;
                response.ErrorMessage = "Something went terribly wrong, offer quantity AND offer price were invalid";
            }
            else if (!checkOfferQuantity(offer.Quantity))
            {
                response.Success = false;
                response.ErrorMessage = $"Offer quantity invalid, offer should contain a quantity of larger than 0";
            }
            else if (!checkOfferPrice(offer.Price))
            {
                response.Success = false;
                response.ErrorMessage = $"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price";
            }

            return response.Success;
        }


        // Checks that offer has a quantity larger than 0
        public static bool checkOfferQuantity(int offerQuantity)
        {
            if (offerQuantity > 0) return true;
            return false;
        }

        // Checks that the offer has a valid price
        public static bool checkOfferPrice(double offerPrice)
        {
            const double MOCKPRICE = 100;
            double highestValid = Math.Round(MOCKPRICE * 1.1, 2);
            double lowestValid = Math.Round(MOCKPRICE * 0.9, 2);

            if (offerPrice <= highestValid && offerPrice >= lowestValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

