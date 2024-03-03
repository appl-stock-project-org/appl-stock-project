using NUnit.Framework;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System;

namespace AppleStockAPI.Tests
{
    [TestFixture]
    public class OfferControllerTests
    {
        private OfferController offerController;

        [SetUp]
        public void SetUp()
        {
            offerController = new OfferController();
        }

        [TearDown]
        public void TearDown()
        {
            offerController.ClearOffers();
        }

        [Test]
        public void HandleOffer_ValidOffer_SuccessResponse()
        {
            Offer offer = new Offer { Quantity = 10, Price = 95.0 }; 
            Response response = offerController.HandleOffer(offer);

            Assert.IsTrue(response.Success);
            Assert.AreEqual($"Offer succesful with the price of {offer.Price} and quantity of {offer.Quantity}", response.SuccessMessage);
            CollectionAssert.Contains(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidPrice_ErrorMessage()
        {
            Offer offer = new Offer { Quantity = 10, Price = 80.0 };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Offer rejected with the value of {offer.Price}, offer needs to be in the price range of 10% of the market price", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidQuantity_ErrorMessage()
        {
            Offer offer = new Offer { Quantity = 0, Price = 95.0 };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Offer quantity invalid, offer should contain a quantity of larger than 0", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidPriceAndQuantity_ErrorMessage()
        {
            Offer offer = new Offer { Quantity = 0, Price = 80.0 };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Something went terribly wrong, offer quantity AND offer price were invalid", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }
    }
}

