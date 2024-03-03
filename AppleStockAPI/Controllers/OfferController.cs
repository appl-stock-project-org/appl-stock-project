﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using AppleStockAPI.Models;

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

        public List<Offer> GetOffers()
        {
            return offers;
        }

        public void ClearOffers()
        {
           offers.Clear();
        }

        // Formats the offer price accordingly and calls to check the offer.
        public Response HandleOffer(Offer offer)
        {
            offer.Price = Math.Truncate(offer.Price * 100) / 100;

            return CheckOffer(offer);
        }


        // Creates a response, checks the offer price and quantity validity, returns the response.
        public Response CheckOffer(Offer offer)
        {
            Response response = new Response();
            if (CheckOfferQuantity(offer.Quantity) && CheckOfferPrice(offer.Price))
            {
                response.Success = true;
                response.SuccessMessage = $"Offer succesful with the price of {offer.Price} and quantity of {offer.Quantity}";
                offers.Add(offer);
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

            return response;
        }


        // Checks that offer has a quantity larger than 0
        public bool CheckOfferQuantity(int offerQuantity)
        {
            return offerQuantity > 0;
        }

        // Checks that the offer has a valid price
        public bool CheckOfferPrice(double offerPrice)
        {
            const double MOCKPRICE = 100;
            double highestValid = Math.Round(MOCKPRICE * 1.1, 2);
            double lowestValid = Math.Round(MOCKPRICE * 0.9, 2);

            return (offerPrice <= highestValid && offerPrice >= lowestValid);
        }

    }
}
