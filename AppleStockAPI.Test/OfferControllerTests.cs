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
        public void HandleOffer_Multiple_POST_Requests()
        {
            int quantity = 5;
            double price = 95.0;

            Offer validOffer1 = new Offer { Quantity = quantity, Price = price };
            Offer validOffer2 = new Offer { Quantity = quantity, Price = price };
            quantity = 1;
            price = 75.0;
            Offer invalidOffer1 = new Offer { Quantity = quantity, Price = price };
            Offer invalidOffer2 = new Offer { Quantity = quantity, Price = price };

            Response response1 = offerController.HandleOffer(validOffer1);
            Response response2 = offerController.HandleOffer(validOffer2);
            Response response3 = offerController.HandleOffer(invalidOffer1);
            Response response4 = offerController.HandleOffer(invalidOffer2);

            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
            Assert.IsFalse(response3.Success);
            Assert.IsFalse(response4.Success);

            List<Offer> offers = offerController.GetOffers();
            CollectionAssert.Contains(offers, validOffer1);
            CollectionAssert.Contains(offers, validOffer2);
            CollectionAssert.DoesNotContain(offers, invalidOffer1);
            CollectionAssert.DoesNotContain(offers, invalidOffer2);
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

        [Test]
        public void GetOffer_ExistingOffer_ReturnsOffer()
        {
            Guid id = Guid.NewGuid();
            const int quantity = 10;
            const double price = 95.0;

            Offer offerToAdd = new Offer { Id = id, Quantity = quantity, Price = price };
            offerController.GetOffers().Add(offerToAdd);

            Offer retrievedOffer = offerController.GetOffer(id);

            Assert.AreEqual(offerToAdd, retrievedOffer);
        }

        [Test]
        public void GetOffer_NonExistentOffer_ReturnsNull()
        {
            Guid id = Guid.NewGuid();
            Offer retrievedOffer = offerController.GetOffer(id);

            Assert.IsNull(retrievedOffer);
        }

        [Test]
        public void RemoveOffer_ExistingOffer_RemovesOffer()
        {
            Guid id = Guid.NewGuid();
            const int quantity = 50;
            const double price = 95.0;
            Offer offerToRemove = new Offer { Id = id, Quantity = quantity, Price = price };
            offerController.GetOffers().Add(offerToRemove);

            offerController.RemoveOffer(id);
            Offer retrievedOffer = offerController.GetOffer(id);

            Assert.IsNull(retrievedOffer);
        }

        [Test]
        public void RemoveOffer_NonExistentOffer_DoesNothing()
        {
            Guid id = Guid.NewGuid();
            offerController.RemoveOffer(id);

            // No assertion is needed, the method should not throw an exception or modify the list
        }

    }
}


