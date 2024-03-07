using NUnit.Framework;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System.Collections.Generic;

namespace AppleStockAPI.Unit.Tests
{
    [TestFixture]
    public class OfferControllerTests
    {
        private OfferController offerController;
        private double mockPrice = 100.0;

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
        public void HandleOffer_ValidOffer_Passes()
        {
            const int expectedQuantity = 10;
            const double expectedPrice = 95.6;
            Offer offer = new Offer { Quantity = expectedQuantity, Price = expectedPrice };

            offerController.ValidateOffer(offer, mockPrice);
            Assert.Pass();
        }

        [Test]
        public void HandleOffer_InvalidPrice_ErrorThrown()
        {
            const int quantity = 10;
            const double invalidPrice = 80.0;
            Offer offer = new Offer { Quantity = quantity, Price = invalidPrice };
            Exception ex = Assert.Throws<Exception>(() => offerController.ValidateOffer(offer, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo($"Offer rejected with the value of {invalidPrice}, offer needs to be in the price range of 10% of the market price"));
        }

        [Test]
        public void HandleOffer_InvalidQuantity_ErrorThrown()
        {
            const int invalidQuantity = 0;
            const double price = 95.0;
            Offer offer = new Offer { Quantity = invalidQuantity, Price = price };
            Exception ex = Assert.Throws<Exception>(() => offerController.ValidateOffer(offer, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo("Offer quantity invalid, offer should contain a quantity of larger than 0"));
        }

        [Test]
        public void HandleOffer_InvalidPriceAndQuantity_ErrorThrown()
        {
            const int invalidQuantity = 0;
            const double invalidPrice = 80.0;
            Offer offer = new Offer { Quantity = invalidQuantity, Price = invalidPrice };
            Exception ex = Assert.Throws<Exception>(() => offerController.ValidateOffer(offer, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo("Something went terribly wrong, offer quantity AND offer price were invalid"));
        }

        [Test]
        public void GetOffer_ExistingOffer_ReturnsOffer()
        {
            Guid id = Guid.NewGuid();
            const int quantity = 10;
            const double price = 95.0;

            Offer offerToAdd = new Offer { Id = id, Quantity = quantity, Price = price };
            offerController.AddOffer(offerToAdd);

            Offer retrievedOffer = offerController.GetOffer(id);

            Assert.That(retrievedOffer, Is.EqualTo(offerToAdd));
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

            Assert.Pass();
        }

    }
}


