using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace AppleStockAPI.Integration.Tests
{
    public class ProgramIntegrationTests
    {
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
            Offer validOffer = new Offer { Quantity = 10, Price = 100 };
            Response response = controller.HandleOffer(validOffer);
            
            // Check for success response
            Assert.IsTrue(response.Success);
            Assert.That(response.SuccessMessage, Is.EqualTo($"Offer successfully placed with the price of {validOffer.Price} and quantity of {validOffer.Quantity}."));

            // Check that only one offer was added and no bids or trades
            Assert.That(controller.ListOffers().Count, Is.EqualTo(1));
            Assert.That(controller.ListBids().Count, Is.EqualTo(0));
            Assert.That(controller.ListTrades().Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleBid_ValidBid_Passes()
        {
            Bid validBid = new Bid { Quantity = 10, Price = 100 };
            Response response = controller.HandleBid(validBid);

            // Check for success response
            Assert.IsTrue(response.Success);
            Assert.That(response.SuccessMessage, Is.EqualTo($"Bid successfully placed with the price of {validBid.Price} and quantity of {validBid.Quantity}."));

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
            Offer validOffer1 = new Offer { Quantity = quantity, Price = price };
            Offer validOffer2 = new Offer { Quantity = quantity, Price = price };

            // Invalid quatity and price for offers
            quantity = 1;
            price = 75.0;
            Offer invalidOffer1 = new Offer { Quantity = quantity, Price = price };
            Offer invalidOffer2 = new Offer { Quantity = quantity, Price = price };

            Response response1 = controller.HandleOffer(validOffer1);
            Response response2 = controller.HandleOffer(validOffer2);
            Response response3 = controller.HandleOffer(invalidOffer1);
            Response response4 = controller.HandleOffer(invalidOffer2);

            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
            Assert.IsFalse(response3.Success);
            Assert.IsFalse(response4.Success);

            List<Offer> offers = controller.ListOffers();
            CollectionAssert.Contains(offers, validOffer1);
            CollectionAssert.Contains(offers, validOffer2);
            CollectionAssert.DoesNotContain(offers, invalidOffer1);
            CollectionAssert.DoesNotContain(offers, invalidOffer2);
            Assert.That(offers.Count, Is.EqualTo(2));
        }

        [Test]
        public void HandleBid_MultipleAddedBids()
        {
            // Valid quatity and price for bids
            int quantity = 5;
            double price = 95.0;
            Bid validBid1 = new Bid { Quantity = quantity, Price = price };
            Bid validBid2 = new Bid { Quantity = quantity, Price = price };
            
            // Invalid quatity and price for bids
            quantity = 1;
            price = 75.0;
            Bid invalidBid1 = new Bid { Quantity = quantity, Price = price };
            Bid invalidBid2 = new Bid { Quantity = quantity, Price = price };

            Response response1 = controller.HandleBid(validBid1);
            Response response2 = controller.HandleBid(validBid2);
            Response response3 = controller.HandleBid(invalidBid1);
            Response response4 = controller.HandleBid(invalidBid2);

            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
            Assert.IsFalse(response3.Success);
            Assert.IsFalse(response4.Success);

            List<Bid> bids = controller.ListBids();
            CollectionAssert.Contains(bids, validBid1);
            CollectionAssert.Contains(bids, validBid2);
            CollectionAssert.DoesNotContain(bids, invalidBid1);
            CollectionAssert.DoesNotContain(bids, invalidBid2);
            Assert.That(bids.Count, Is.EqualTo(2));
        }

        [Test]
        public void HandleOffer_MatchesToBid_NoLeftOvers()
        {
            Offer validOffer = new Offer { Quantity = 100, Price = 100 };
            Bid validBid = new Bid { Quantity = 100, Price = 100 };

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
            Offer validOffer = new Offer { Quantity = 200, Price = 100 };
            Bid validBid = new Bid { Quantity = 100, Price = 100 };

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
            Offer validOffer = new Offer { Quantity = 100, Price = 100 };
            Bid validBid = new Bid { Quantity = 200, Price = 100 };

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
            Bid validBid = new Bid { Quantity = 100, Price = 100 };
            Offer validOffer = new Offer { Quantity = 100, Price = 100 };

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
            Bid validBid = new Bid { Quantity = 200, Price = 100 };
            Offer validOffer = new Offer { Quantity = 100, Price = 100 };

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
            Bid validBid = new Bid { Quantity = 100, Price = 100 };
            Offer validOffer = new Offer { Quantity = 200, Price = 100 };

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
            Offer offer = new Offer { Quantity = 100, Price = 100 };
            Bid bid1 = new Bid { Quantity = 100, Price = 100 };
            Bid bid2 = new Bid { Quantity = 100, Price = 110 };
            Bid bid3 = new Bid { Quantity = 100, Price = 90 };

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
            Bid bid = new Bid { Quantity = 100, Price = 100 };
            Offer offer1 = new Offer { Quantity = 100, Price = 110 };
            Offer offer2 = new Offer { Quantity = 100, Price = 100 };
            Offer offer3 = new Offer { Quantity = 100, Price = 90 };

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