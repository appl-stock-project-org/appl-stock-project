using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System;

namespace AppleStockAPI.Unit.Tests
{
    public class BidControllerTests
    {
        private static BidController controller;

        [OneTimeSetUp]
        public void Setup()
        {
            controller = new BidController();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            controller.ClearBids();
        }


        [TestCase(100, 100.0, 100.0)] // Test with exact matching price and valid quantity
        [TestCase(123, 90.0, 100.0)] // Test with other bid values and that SuccessMessage is dynamic
        [TestCase(1, 110.0, 100.0)] // Test upper bound
        [TestCase(6, 654.0, 654.0)] // Test other stockprice than 100
        [TestCase(6, 588.6, 654.0)] // Test decimal bid price
        [TestCase(6, 719.4, 654.0)] // Upper bound with decimal
        [TestCase(11, 111.11, 111.11)] // With decimal stockprice
        [TestCase(11, 100.0, 111.11)] // Lower bound with decimals
        [TestCase(11, 122.2211111111, 111.11)] // Upper bound with decimal stockprice. Also test the rounding of bid price
        public void ValidateBid_ValidBid_Passes(int quantity, double price, double mockPrice)
        {
            Bid bid = new Bid { Quantity = quantity, Price = price };

            controller.ValidateBid(bid, mockPrice);
            Assert.Pass();
        }

        [Test]
        public void ValidateBid_InvalidBid_ThrowsError_BidPriceTooHigh()
        {
            const int quantity = 1;
            const double price = 110.01;
            const double mockPrice = 100.0;
            Bid bid = new Bid { Quantity = quantity, Price = price };

            Exception ex = Assert.Throws<Exception>(() => controller.ValidateBid(bid, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo("Bid price is too high. Highest accepted price at the moment is 110."));
        }

        [Test]
        public void ValidateBid_InvalidBid_ThrowsError_BidPriceTooLow()
        {
            const int quantity = 1;
            const double price = 89.9999999;
            const double mockPrice = 100.0;
            Bid bid = new Bid { Quantity = quantity, Price = price };

            Exception ex = Assert.Throws<Exception>(() => controller.ValidateBid(bid, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo("Bid price is too low. Lowest accepted price at the moment is 90."));
        }

        [TestCase(0, 100, 100.0)]
        [TestCase(-45, 100, 1000)]
        public void ValidateBid_InvalidBid_ThrowsError_BidQuantityZero(int quantity, double price, double mockPrice)
        {
            Bid bid = new Bid { Quantity = quantity, Price = price };

            Exception ex = Assert.Throws<Exception>(() => controller.ValidateBid(bid, mockPrice));

            Assert.IsNotNull(ex);
            Assert.That(ex.Message, Is.EqualTo("Bid quantity needs to be above 0."));
        }


        [Test]
        public void RemovingBidFromListTest()
        {
            var testGuid = Guid.NewGuid();
            Bid bid1 = new Bid { Id = testGuid, Quantity = 1, Price = 100.0 };
            Bid bid2 = new Bid { Quantity = 2, Price = 200.0 };
            Bid bid3 = new Bid { Quantity = 2, Price = 200.0 };
            controller.AddBid(bid1);
            controller.AddBid(bid2);
            controller.AddBid(bid3);

            // Remove bid from list
            controller.RemoveBidWithId(testGuid);
            // Check that count decreased by one ...
            Assert.That(controller.GetBids(), Has.Count.EqualTo(2));
            // ... and that the bid is not to be found
            Bid? notToBeFoundBid = controller.GetBidWithId(testGuid);
            Assert.That(notToBeFoundBid, Is.EqualTo(null));
        }

        [Test]
        public void RemovingNonExistentBidTest()
        {
            // Removing with random id gives error message
            Guid newId = new ();
            StringAssert.IsMatch($"Bid with id {newId} was not found.", controller.RemoveBidWithId(newId));
        }
    }
}
