using AppleStockAPI.Controllers;
using AppleStockAPI.Models;

namespace AppleStockAPI.Test
{
    public class BidControllerTests {
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

        protected static void TestHandleBid((double CurrentPrice, Bid TestBid, Response ExpectedRes, int ExpectedCountOfBids) testData)
        {
            var res = controller.HandleBid(testData.TestBid, testData.CurrentPrice);
            Guid id = testData.TestBid.Id;
            Bid? foundBid = controller.GetBidWithId(id);

            bool isSupposedToFail = testData.ExpectedRes.Success == false;
            Assert.Multiple(() => 
            {
                Assert.That(res, Is.EqualTo(testData.ExpectedRes));
                Assert.That(foundBid, Is.EqualTo(isSupposedToFail ? null : testData.TestBid));
                Assert.That(controller.GetBids(), Has.Count.EqualTo(testData.ExpectedCountOfBids));
            });
        }

        [TestFixture]
        public class AddingOnlyValidBidsTestFixture : BidControllerTests
        {
            [TestCaseSource(nameof(ValidBidTestCases))]
            public void TestValidBid((double CurrentPrice, Bid TestBid, Response ExpectedRes, int ExpectedCountOfBids) testData)
            {
                TestHandleBid(testData);
            }

            private static IEnumerable<(double, Bid, Response, int)> ValidBidTestCases()
            {
                yield return (
                    100,
                    new Bid { Price = 100, Quantity = 100 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 100 and quantity 100.", 
                        Success = true
                        },
                    1
                    );
                yield return (
                    100,
                    new Bid { Price = 90, Quantity = 123 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 90 and quantity 123.", 
                        Success = true
                        },
                    2
                    );
                yield return (
                    100,
                    new Bid { Price = 110, Quantity = 1 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 110 and quantity 1.", 
                        Success = true
                        },
                    3
                    );
                yield return (
                    654,
                    new Bid { Price = 654, Quantity = 6 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 654 and quantity 6.", 
                        Success = true
                        },
                    4
                    );
                yield return (
                    654,
                    new Bid { Price = 588.6, Quantity = 6 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 588,6 and quantity 6.", 
                        Success = true
                        },
                    5
                    );
                yield return (
                    654,
                    new Bid { Price = 719.4, Quantity = 6 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 719,4 and quantity 6.", 
                        Success = true
                        },
                    6
                    );
                yield return (
                    111.11,
                    new Bid { Price = 111.11, Quantity = 11 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 111,11 and quantity 11.", 
                        Success = true
                        },
                    7
                    );
                yield return (
                    111.11,
                    new Bid { Price = 100.00, Quantity = 11 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 100 and quantity 11.", 
                        Success = true
                        },
                    8
                    );
                yield return (
                    111.11,
                    new Bid { Price = 122.22, Quantity = 11 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 122,22 and quantity 11.", 
                        Success = true
                        },
                    9
                    );
                yield return (
                    111.11,
                    new Bid { Price = 122.2211111111, Quantity = 11 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 122,22 and quantity 11.", 
                        Success = true
                        },
                    10
                    );
            }

        }


        [TestFixture]
        public class AddingValidAndInvalidBidsTestFixture : BidControllerTests
        {
            [TestCaseSource(nameof(ValidAndInvalidBidTestCases))]
            public void TestValidOrInvalidBid((double CurrentPrice, Bid TestBid, Response ExpectedRes, int ExpectedCountOfBids) testData)
            {
                TestHandleBid(testData);
            }

            private static IEnumerable<(double, Bid, Response, int)> ValidAndInvalidBidTestCases()
            {
                yield return (
                    100,
                    new Bid { Price = 100, Quantity = 100 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 100 and quantity 100.", 
                        Success = true
                        },
                    1
                    );
                yield return (
                    100,
                    new Bid { Price = 90, Quantity = 123 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 90 and quantity 123.", 
                        Success = true
                        },
                    2
                    );
                yield return (
                    100,
                    new Bid { Price = 111, Quantity = 1 }, 
                    new Response { 
                        ErrorMessage = "Bid price is too high. Highest accepted price at the moment is 110.", 
                        SuccessMessage = null, 
                        Success = false
                        },
                    2
                    );
                yield return (
                    100,
                    new Bid { Price = 89.99, Quantity = 1 }, 
                    new Response { 
                        ErrorMessage = "Bid price is too low. Lowest accepted price at the moment is 90.", 
                        SuccessMessage = null, 
                        Success = false
                        },
                    2
                    );
                yield return (
                    100,
                    new Bid { Price = 100, Quantity = 0 }, 
                    new Response { 
                        ErrorMessage = "Bid quantity needs to be above 0.", 
                        SuccessMessage = null, 
                        Success = false
                        },
                    2
                    );
                yield return (
                    1000,
                    new Bid { Price = 100, Quantity = -45 }, 
                    new Response { 
                        ErrorMessage = "Bid quantity needs to be above 0.", 
                        SuccessMessage = null, 
                        Success = false
                        },
                    2
                    );
                yield return (
                    200,
                    new Bid { Price = 220, Quantity = 2 }, 
                    new Response { 
                        ErrorMessage = null, 
                        SuccessMessage = "Bid placed succesfully with price 220 and quantity 2.", 
                        Success = true
                        },
                    3
                    );

            }
        }

        [TestFixture]
        public class OtherThanHandleBidMethodsTests : BidControllerTests {

            [TestCase]
            public void BidsListCountGoesUpTest() {
                Assert.That(controller.GetBids(), Has.Count.EqualTo(0));
                Bid bid1 = new (){Price = 100, Quantity = 100};
                controller.HandleBid(bid1, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(1));
                Bid bid2 = new (){Price = 100, Quantity = 100};
                controller.HandleBid(bid2, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(2));
            }

            [TestCase]
            public void CountDoesntChangeFromAccessingTest() {
                Bid bid3 = new (){Price = 100, Quantity = 100};
                controller.HandleBid(bid3, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(3));
                Guid id = bid3.Id;
                Bid? foundBidWithGuid = controller.GetBidWithId(id);
                Bid? foundBidWithStringId = controller.GetBidWithId(id.ToString());
                Assert.Multiple(() => {
                    Assert.That(foundBidWithGuid, Is.EqualTo(foundBidWithStringId));
                    Assert.That(controller.GetBids(), Has.Count.EqualTo(3));
                    Assert.That(controller.GetBidWithId("FakeId"), Is.EqualTo(null));
                });
            }

            [TestCase]
            public void RemovingBidsTest() {
                StringAssert.IsMatch("Bid with id FakeId was not found.", controller.RemoveBidWithId("FakeId"));
                Bid bid4 = new (){Price = 100, Quantity = 100};
                controller.HandleBid(bid4, 100);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(4));
                string id = bid4.Id.ToString();
                controller.RemoveBidWithId(id);
                Assert.That(controller.GetBids(), Has.Count.EqualTo(3));
                Bid? notToBeFoundBid = controller.GetBidWithId(id.ToString());
                Assert.That(notToBeFoundBid, Is.EqualTo(null));
            }
        }
    }
}
