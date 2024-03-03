using NUnit.Framework;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System.Collections.Generic;

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
            const int expectedQuantity = 10;
            const double expectedPrice = 95.6;
            Offer offer = new Offer { Quantity = expectedQuantity, Price = expectedPrice };
            Response response = offerController.HandleOffer(offer);

            Assert.IsTrue(response.Success);
            Assert.AreEqual($"Offer successful with the price of {expectedPrice} and quantity of {expectedQuantity}", response.SuccessMessage);
            CollectionAssert.Contains(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidPrice_ErrorMessage()
        {
            const int quantity = 10;
            const double invalidPrice = 80.0;
            Offer offer = new Offer { Quantity = quantity, Price = invalidPrice };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Offer rejected with the value of {invalidPrice}, offer needs to be in the price range of 10% of the market price", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidQuantity_ErrorMessage()
        {
            const int invalidQuantity = 0;
            const double price = 95.0;
            Offer offer = new Offer { Quantity = invalidQuantity, Price = price };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Offer quantity invalid, offer should contain a quantity of larger than 0", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }

        [Test]
        public void HandleOffer_InvalidPriceAndQuantity_ErrorMessage()
        {
            const int invalidQuantity = 0;
            const double invalidPrice = 80.0;
            Offer offer = new Offer { Quantity = invalidQuantity, Price = invalidPrice };
            Response response = offerController.HandleOffer(offer);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Something went terribly wrong, offer quantity AND offer price were invalid", response.ErrorMessage);
            CollectionAssert.DoesNotContain(offerController.GetOffers(), offer);
        }
    }
}


