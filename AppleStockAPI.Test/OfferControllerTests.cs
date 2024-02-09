using NUnit.Framework;
using System.Collections.Generic;
using AppleStockAPI.Models;
using AppleStockAPI.Controllers;

namespace AppleStockAPI.Tests
{
    [TestFixture]
    public class OfferControllerTests
    {
        [Test]
        public void HandleOffer_ValidOffer_ReturnsSuccessResponse()
        {
            var offer = new Offer { Price = 95, Quantity = 10 };
            var offers = new List<Offer>();

            var response = OfferController.handleOffer(offer, offers);

            Assert.IsTrue(response.Success);
            Assert.AreEqual($"Offer succesful with the price of {offer.Price} and quantity of {offer.Quantity}", response.SuccessMessage);
        }

        [Test]
        public void HandleOffer_InvalidPrice_ReturnsErrorResponse()
        {
            var offer = new Offer { Price = 80, Quantity = 10 };
            var offers = new List<Offer>();

            var response = OfferController.handleOffer(offer, offers);

            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price", response.ErrorMessage);
        }

        [Test]
        public void CheckOfferQuantity_ValidQuantity_ReturnsTrue()
        {
            int quantity = 5;

            var result = OfferController.checkOfferQuantity(quantity);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckOfferQuantity_InvalidQuantity_ReturnsFalse()
        {
            int quantity = -1;

            var result = OfferController.checkOfferQuantity(quantity);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckOfferPrice_ValidPrice_ReturnsTrue()
        {
            double price = 95;

            var result = OfferController.checkOfferPrice(price);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckOfferPrice_InvalidPrice_ReturnsFalse()
        {
            double price = 80;

            var result = OfferController.checkOfferPrice(price);

            Assert.IsFalse(result);
        }
    }
}
