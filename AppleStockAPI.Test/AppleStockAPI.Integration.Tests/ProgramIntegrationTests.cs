using System.Text.Json;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;


namespace AppleStockAPI.Integration.Tests
{
    public class ProgramIntegrationTests
    {

        protected JsonElement ConvertObjectToJsonElement(object obj) {
            string jsonString = JsonSerializer.Serialize(obj);
            // Parse the JSON string to get a JsonDocument
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            // Get the root JsonElement
            return jsonDocument.RootElement.Clone();
        }

        SystemController controller { get; set; }

        [SetUp]
        public void Setup()
        {
            controller = new SystemController(100.0); // Mock price
        }

        [TearDown]
        public void TearDown()
        {
            controller.Reset();
        }

        [Test]
        public void HandleOffer_ValidOffer_Passes()
        {
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity = 10, price = 100});
            Response response = controller.HandleOffer(validOffer);
            
            // Check for success response
            Assert.IsTrue(response.Success);
            Assert.That(response.SuccessMessage, Is.EqualTo($"Offer successfully placed with the price of 100 and quantity of 10."));

            // Check that only one offer was added and no bids or trades
            Assert.That(controller.ListOffers().Count, Is.EqualTo(1));
            Assert.That(controller.ListBids().Count, Is.EqualTo(0));
            Assert.That(controller.ListTrades().Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleBid_ValidBid_Passes()
        {
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity = 10, price = 100});
            Response response = controller.HandleBid(validBid);

            // Check for success response
            Assert.IsTrue(response.Success);
            Assert.That(response.SuccessMessage, Is.EqualTo($"Bid successfully placed with the price of 100 and quantity of 10."));

            // Check that only one bid was added and no offers or trades
            Assert.That(controller.ListBids().Count, Is.EqualTo(1));
            Assert.That(controller.ListOffers().Count, Is.EqualTo(0));
            Assert.That(controller.ListTrades().Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleOffer_MultipleAddedOffers()
        {
            // Valid quatity and price for offers
            int quantity = 5;
            double price = 95.0;
            JsonElement validOffer1 = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement validOffer2 = ConvertObjectToJsonElement(new{ quantity, price});

            // Invalid quatity and price for offers
            quantity = 1;
            price = 75.0;
            JsonElement invalidOffer1 = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement invalidOffer2 = ConvertObjectToJsonElement(new{ quantity, price});            

            Response response1 = controller.HandleOffer(validOffer1);
            Response response2 = controller.HandleOffer(validOffer2);
            Response response3 = controller.HandleOffer(invalidOffer1);
            Response response4 = controller.HandleOffer(invalidOffer2);

            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
            Assert.IsFalse(response3.Success);
            Assert.IsFalse(response4.Success);


            List<Offer> offers = controller.ListOffers();
            IEnumerable<Guid>? offerIds = offers.Select(offer => offer.Id);
            CollectionAssert.Contains(offerIds, response1.RecordId);
            CollectionAssert.Contains(offerIds, response2.RecordId);
            CollectionAssert.DoesNotContain(offerIds, response3.RecordId);
            CollectionAssert.DoesNotContain(offerIds, response4.RecordId);
            Assert.That(offers.Count, Is.EqualTo(2));
        }

        [Test]
        public void HandleBid_MultipleAddedBids()
        {
            // Valid quatity and price for bids
            int quantity = 5;
            double price = 95.0;
            JsonElement validBid1 = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement validBid2 = ConvertObjectToJsonElement(new{ quantity, price});
            
            // Invalid quatity and price for bids
            quantity = 1;
            price = 75.0; 
            JsonElement invalidBid1 = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement invalidBid2 = ConvertObjectToJsonElement(new{ quantity, price});

            Response response1 = controller.HandleBid(validBid1);
            Response response2 = controller.HandleBid(validBid2);
            Response response3 = controller.HandleBid(invalidBid1);
            Response response4 = controller.HandleBid(invalidBid2);

            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
            Assert.IsFalse(response3.Success);
            Assert.IsFalse(response4.Success);

            List<Bid> bids = controller.ListBids();
            IEnumerable<Guid>? bidIds = bids.Select(bid => bid.Id);

            CollectionAssert.Contains(bidIds, response1.RecordId);
            CollectionAssert.Contains(bidIds, response2.RecordId);
            CollectionAssert.DoesNotContain(bidIds, response3.RecordId);
            CollectionAssert.DoesNotContain(bidIds, response4.RecordId);
            Assert.That(bids.Count, Is.EqualTo(2));
        }

        [Test]
        public void HandleOffer_MatchesToBid_NoLeftOvers()
        {
            int quantity = 100;
            int price = 100;
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity, price});

            Response response1 = controller.HandleBid(validBid);
            
            Assert.IsTrue(response1.Success);

            Response response2 = controller.HandleOffer(validOffer);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer was not added to offers since there is no excess quantity
            List<Offer> offers = controller.ListOffers();
            CollectionAssert.DoesNotContain(offers, validOffer);
            Assert.That(offers.Count, Is.EqualTo(0));

            // Check that the bid was removed from bids since it was fully consumed
            List<Bid> bids = controller.ListBids();
            CollectionAssert.DoesNotContain(bids, validBid);
            Assert.That(bids.Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleOffer_MatchesToBid_OfferQuantityLeftOver()
        {
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity = 200, price = 100});
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});

            Response response1 = controller.HandleBid(validBid);
            Response response2 = controller.HandleOffer(validOffer);
            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer was added to offers since there is 100 excess quantity
            List<Offer> offers = controller.ListOffers();
            Assert.That(offers.Count, Is.EqualTo(1));
            Assert.That(offers[0].Price, Is.EqualTo(100));
            Assert.That(offers[0].Quantity, Is.EqualTo(100));

            // Check that the bid was removed from bids since it was fully consumed
            List<Bid> bids = controller.ListBids();
            CollectionAssert.DoesNotContain(bids, validBid);
            Assert.That(bids.Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleOffer_MatchesToBid_BidQuantityLeftOver()
        {
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity = 200, price = 100});

            Response response1 = controller.HandleBid(validBid);
            Response response2 = controller.HandleOffer(validOffer);
            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer was not added to offers since there is no excess quantity
            List<Offer> offers = controller.ListOffers();
            CollectionAssert.DoesNotContain(offers, validOffer);
            Assert.That(offers.Count, Is.EqualTo(0));

            // Check that the bid was added to bids since there is 100 excess quantity
            List<Bid> bids = controller.ListBids();
            Assert.That(bids.Count, Is.EqualTo(1));
            Assert.That(bids[0].Price, Is.EqualTo(100));
            Assert.That(bids[0].Quantity, Is.EqualTo(100));
        }

        [Test]
        public void HandleBid_MatchesToOffer_NoLeftOver()
        {
            int quantity = 100;
            int price = 100;
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity, price});

            Response response1 = controller.HandleOffer(validOffer);
            Assert.IsTrue(response1.Success);

            Response response2 = controller.HandleBid(validBid);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer was not added to offers since there is no excess quantity
            List<Offer> offers = controller.ListOffers();
            CollectionAssert.DoesNotContain(offers, validOffer);
            Assert.That(offers.Count, Is.EqualTo(0));

            // Check that the bid was removed from bids since it was fully consumed
            List<Bid> bids = controller.ListBids();
            CollectionAssert.DoesNotContain(bids, validBid);
            Assert.That(bids.Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleBid_MatchesToOffer_BidQuantityLeftOver()
        {
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity = 200, price = 100});
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});

            Response response1 = controller.HandleOffer(validOffer);
            Response response2 = controller.HandleBid(validBid);
            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer was not added to offers since there is no excess quantity
            List<Offer> offers = controller.ListOffers();
            CollectionAssert.DoesNotContain(offers, validOffer);
            Assert.That(offers.Count, Is.EqualTo(0));

            // Check that the bid was added to bids since there is 100 excess quantity
            List<Bid> bids = controller.ListBids();
            Assert.That(bids.Count, Is.EqualTo(1));
            Assert.That(bids[0].Price, Is.EqualTo(100));
            Assert.That(bids[0].Quantity, Is.EqualTo(100));
        }

        [Test]
        public void HandleBid_MatchesToOffer_OfferQuantityLeftOver()
        {
            JsonElement validBid = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});
            JsonElement validOffer = ConvertObjectToJsonElement(new{ quantity = 200, price = 100});

            Response response1 = controller.HandleBid(validBid);
            Response response2 = controller.HandleOffer(validOffer);
            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);

            // Check if the trade was recorded with correct data
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(100));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));

            // Check that the offer is still in offers with 100 quantity left
            List<Offer> offers = controller.ListOffers();
            Assert.That(offers.Count, Is.EqualTo(1));
            Assert.That(offers[0].Price, Is.EqualTo(100));
            Assert.That(offers[0].Quantity, Is.EqualTo(100));

            // Check that the bid is not in bids since it was fully consumed
            List<Bid> bids = controller.ListBids();
            CollectionAssert.DoesNotContain(bids, validBid);
            Assert.That(bids.Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleOffer_MatchesToHighestBid()
        {

            int quantity = 100;
            int price = 100;
            JsonElement offer = ConvertObjectToJsonElement(new{ quantity, price});
            JsonElement bid1 = ConvertObjectToJsonElement(new{ quantity, price});
            
            price = 110;
            JsonElement bid2 = ConvertObjectToJsonElement(new{ quantity, price});

            price = 90;
            JsonElement bid3 = ConvertObjectToJsonElement(new{ quantity, price});

            controller.HandleBid(bid1);
            controller.HandleBid(bid2);
            controller.HandleBid(bid3);
            Assert.That(controller.ListBids().Count, Is.EqualTo(3));

            Response response = controller.HandleOffer(offer);
            Assert.IsTrue(response.Success);

            // Check if the trade was recorded with correct data, which is 110 price and 100 quantity
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(110));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));
        }

        [Test]
        public void HandleBid_MatchesToCheapestOffer()
        {
            JsonElement bid = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});
            JsonElement offer1 = ConvertObjectToJsonElement(new{ quantity = 100, price = 110});
            JsonElement offer2 = ConvertObjectToJsonElement(new{ quantity = 100, price = 100});
            JsonElement offer3 = ConvertObjectToJsonElement(new{ quantity = 100, price = 90});

            controller.HandleOffer(offer1);
            controller.HandleOffer(offer2);
            controller.HandleOffer(offer3);
            Assert.That(controller.ListOffers().Count, Is.EqualTo(3));

            Response response = controller.HandleBid(bid);
            Assert.IsTrue(response.Success);

            // Check if the trade was recorded with correct data, which is 90 price and 100 quantity
            List<Trade> trades = controller.ListTrades();
            Assert.That(trades.Count, Is.EqualTo(1));
            Assert.That(trades[0].Price, Is.EqualTo(90));
            Assert.That(trades[0].Quantity, Is.EqualTo(100));
        }
    }
}