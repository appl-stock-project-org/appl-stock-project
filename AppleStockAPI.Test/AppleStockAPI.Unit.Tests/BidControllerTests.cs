using AppleStockAPI.Controllers;
using AppleStockAPI.Models;

namespace AppleStockAPI.Test.AppleStockAPI.Unit.Tests
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

        protected static void TestHandleBid((double CurrentPrice, Bid TestBid, Response? ExpectedRes, int ExpectedCountOfBids) testData)
        {
            var res = controller.HandleBid(testData.TestBid, testData.CurrentPrice);
            Guid id = testData.TestBid.Id;
            Bid? foundBid = controller.GetBidWithId(id);

            // Do we test the response object
            if (testData.ExpectedRes != null)
            {
                bool isSupposedToFail = testData.ExpectedRes.Success == false;
                Assert.Multiple(() =>
                {
                    Assert.That(res, Is.EqualTo(testData.ExpectedRes));
                    Assert.That(foundBid, Is.EqualTo(isSupposedToFail ? null : testData.TestBid));
                });
            }

            // Testing that whether a bid got added to the list
            Assert.That(controller.GetBids(), Has.Count.EqualTo(testData.ExpectedCountOfBids));
        }

        [TestFixture]
        public class ValidBidsTestFixture : BidControllerTests
        {
            [TestCaseSource(nameof(ValidBidTestCases))]
            public void TestValidBid((double CurrentPrice, Bid TestBid, Response? ExpectedRes, int ExpectedCountOfBids) testData)
            {
                TestHandleBid(testData);
            }

            private static IEnumerable<(double, Bid, Response?, int)> ValidBidTestCases()
            {
                // Test with exact matching price and valid quantity
                yield return (
                    100,
                    new Bid { Price = 100, Quantity = 100 },
                    new Response
                    {
                        ErrorMessage = null,
                        SuccessMessage = "Bid placed succesfully with price 100 and quantity 100.",
                        Success = true
                    },
                    1
                    );
                // Test with other bid values and that SuccessMessage is dynamic
                yield return (
                    100,
                    new Bid { Price = 90, Quantity = 123 },
                    new Response
                    {
                        ErrorMessage = null,
                        SuccessMessage = "Bid placed succesfully with price 90 and quantity 123.",
                        Success = true
                    },
                    2
                    );
                // Test upper bound
                yield return (
                    100,
                    new Bid { Price = 110, Quantity = 1 },
                    null,
                    3
                    );
                // Test other stockprice than 100
                yield return (
                    654,
                    new Bid { Price = 654, Quantity = 6 },
                    null,
                    4
                    );
                // Test decimal bid price
                yield return (
                    654,
                    new Bid { Price = 588.6, Quantity = 6 },
                    new Response
                    {
                        ErrorMessage = null,
                        SuccessMessage = $"Bid placed succesfully with price {588.6} and quantity 6.",
                        Success = true
                    },
                    5
                    );
                // Upper bound with decimal
                yield return (
                    654,
                    new Bid { Price = 719.4, Quantity = 6 },
                    null,
                    6
                    );
                // With decimal stockprice
                yield return (
                    111.11,
                    new Bid { Price = 111.11, Quantity = 11 },
                    null,
                    7
                    );
                // Lower bound with decimals
                yield return (
                    111.11,
                    new Bid { Price = 100.0, Quantity = 11 },
                    null,
                    8
                    );
                // Upper bound with decimal stockprice. Also test the rounding of bid price
                yield return (
                    111.11,
                    new Bid { Price = 122.2211111111, Quantity = 11 },
                    new Response
                    {
                        ErrorMessage = null,
                        SuccessMessage = $"Bid placed succesfully with price {122.22} and quantity 11.",
                        Success = true
                    },
                    9
                    );
            }

        }


        [TestFixture]
        public class InvalidBidsTestFixture : BidControllerTests
        {
            [TestCaseSource(nameof(InvalidBidTestCases))]
            public void TestInvalidBid((double CurrentPrice, Bid TestBid, Response? ExpectedRes, int ExpectedCountOfBids) testData)
            {
                TestHandleBid(testData);
            }

            private static IEnumerable<(double, Bid, Response?, int)> InvalidBidTestCases()
            {
                // Test just over upper bound
                yield return (
                    100,
                    new Bid { Price = 110.01, Quantity = 1 },
                    new Response
                    {
                        ErrorMessage = "Bid price is too high. Highest accepted price at the moment is 110.",
                        SuccessMessage = null,
                        Success = false
                    },
                    0
                    );
                // Just under lower bound
                yield return (
                    100,
                    new Bid { Price = 89.9999999, Quantity = 1 },
                    new Response
                    {
                        ErrorMessage = "Bid price is too low. Lowest accepted price at the moment is 90.",
                        SuccessMessage = null,
                        Success = false
                    },
                    0
                    );
                // Quantity zero
                yield return (
                    100,
                    new Bid { Price = 100, Quantity = 0 },
                    new Response
                    {
                        ErrorMessage = "Bid quantity needs to be above 0.",
                        SuccessMessage = null,
                        Success = false
                    },
                    0
                    );
                // Quantity negative
                yield return (
                    1000,
                    new Bid { Price = 100, Quantity = -45 },
                    null,
                    0
                    );
            }
        }

        [TestFixture]
        public class BidControllerUseCasesTests : BidControllerTests
        {


            [Test, Order(1)]
            public void BidsListCountGoesUpTest()
            {
                Assert.That(controller.GetBids(), Has.Count.EqualTo(0));
                Bid bid1 = new() { Price = 100, Quantity = 100 };
                controller.HandleBid(bid1, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(1));
                Bid bid2 = new() { Price = 100, Quantity = 100 };
                controller.HandleBid(bid2, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(2));
            }

            [Test, Order(2)]
            public void CountDoesntGoUpWithInvalidTest()
            {
                Bid invalidBid = new() { Price = 789, Quantity = 100 };
                controller.HandleBid(invalidBid, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(2));
            }

            [Test, Order(3)]
            public void CountDoesntChangeFromAccessingTest()
            {
                Bid bid3 = new() { Price = 100, Quantity = 100 };
                controller.HandleBid(bid3, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(3));
                Bid? foundBid = controller.GetBidWithId(bid3.Id);
                Assert.Multiple(() =>
                {
                    Assert.That(foundBid, Is.EqualTo(bid3));
                    Assert.That(controller.GetBids(), Has.Count.EqualTo(3));
                    Assert.That(controller.GetBidWithId(new Guid()), Is.EqualTo(null));
                });
            }

            [Test, Order(4)]
            public void RemovingBidFromListTest()
            {
                // Get id of a bid in the list
                Guid idFromListBid = controller.GetBids().First().Id;
                // Remove bid from list
                controller.RemoveBidWithId(idFromListBid);
                // Check that count decreased by one ...
                Assert.That(controller.GetBids(), Has.Count.EqualTo(2));
                // ... and that the bid is not to be found
                Bid? notToBeFoundBid = controller.GetBidWithId(idFromListBid);
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
}
