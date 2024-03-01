using NUnit.Framework;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace AppleStockAPI.Tests
{
    [TestFixture]
    public class OfferControllerTests
    {
        private List<Offer> offers;
        private OfferController offerController;

        [SetUp]
        public void SetUp()
        {
            offers = new List<Offer>();
            offerController = new OfferController();
        }

        [TearDown]
        public void TearDown()
        {
            offers = null;
            offerController = null;
        }

        [Test]
        public void HandleOffer_ValidOffer_ReturnsSuccessResponse()
        {
            Offer offer = new Offer { Price = 95.0, Quantity = 10 };
            Response response = OfferController.HandleOffer(offer, offers);

            Assert.IsTrue(response.Success);
            Assert.AreEqual($"Offer succesful with the price of {offer.Price} and quantity of {offer.Quantity}", response.SuccessMessage);
        }

        [Test]
        public void HandleOffer_InvalidPrice_ReturnsErrorResponse()
        {
            Offer offer = new Offer { Price = 120.0, Quantity = 10 };
            Response response = OfferController.HandleOffer(offer, offers);

            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price", response.ErrorMessage);
        }

        [Test]
        public void HandleOffer_InvalidQuantity_ReturnsErrorResponse()
        {
            Offer offer = new Offer { Price = 95.0, Quantity = -5 };
            Response response = OfferController.HandleOffer(offer, offers);

            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Offer quantity invalid, offer should contain a quantity of larger than 0", response.ErrorMessage);
        }

        [Test]
        public void CheckOfferPrice_ValidPrice_ReturnsTrue()
        {
            double validPrice = 105.0;
            bool result = OfferController.CheckOfferPrice(validPrice);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckOfferPrice_InvalidPrice_ReturnsFalse()
        {
            double invalidPrice = 120.0;
            bool result = OfferController.CheckOfferPrice(invalidPrice);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckOfferQuantity_ValidQuantity_ReturnsTrue()
        {
            int validQuantity = 5;
            bool result = OfferController.CheckOfferQuantity(validQuantity);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckOfferQuantity_InvalidQuantity_ReturnsFalse()
        {
            int invalidQuantity = -5;
            bool result = OfferController.CheckOfferQuantity(invalidQuantity);

            Assert.IsFalse(result);
        }

        [Test]
        public void GetOffers_AfterClearing_ReturnsEmptyList()
        {
            offerController.ClearOffers();
            List<Offer> result = offerController.GetOffers();

            Assert.IsEmpty(result);
        }
    }
}

