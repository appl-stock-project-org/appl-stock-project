using AppleStockAPI.Controllers;
using AppleStockAPI.Models;
using System.Diagnostics;
using System;
using System.Globalization;

namespace AppleStockAPI.Unit.Tests
{
    public class TradeControllerTests
    {
        private TradeController controller;

        [SetUp]
        public void Setup()
        {
            controller = new TradeController();
        }

        [TearDown]
        public void TearDown()
        {
            controller.ClearTrades();
        }

        [TestCase(150.0, 50)]
        [TestCase(1, 1)]
        public void RecordTrade_AddsTrade(double price, int quantity) 
        {
            controller.RecordTrade(price, quantity);

            Assert.That(controller.GetTrades().Count(), Is.EqualTo(1));
            
            var trade = controller.GetTrades().First();
            var time = DateTime.Now;
            Assert.That(trade.Price, Is.EqualTo(price));
            Assert.That(trade.Quantity, Is.EqualTo(quantity));
            Assert.That(trade.Id, Is.Not.Empty);
            Assert.That(trade.TradeTime.Year, Is.EqualTo(time.Year));
            Assert.That(trade.TradeTime.Month, Is.EqualTo(time.Month));
            Assert.That(trade.TradeTime.Day, Is.EqualTo(time.Day));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(-1, 1)]
        public void RecordTrade_NoAddedTrade(double price, int quantity)
        {
            controller.RecordTrade(price, quantity);

            Assert.That(controller.GetTrades().Count(), Is.EqualTo(0));
        }

        [Test]
        public void ClearTrades_EmptiesTrades()
        {
            controller.RecordTrade(150.0, 50);
            Assert.That(controller.GetTrades().Count(), Is.EqualTo(1));

            controller.ClearTrades();
            Assert.That(controller.GetTrades().Count(), Is.EqualTo(0));
        }

        [Test]
        public void ListTrades_TradesList()
        {
            controller.RecordTrade(2.0, 1);
            controller.RecordTrade(1.0, 2);
            controller.RecordTrade(4.0, 5);

            var list = controller.ListTrades();

            Assert.Greater(list[1].TradeTime, list[0].TradeTime);
        }

        [Test]
        public void ListTrades_EmptyTradesList()
        {
            var list = controller.ListTrades();

            Assert.That(list.Count, Is.EqualTo(0));
        }
    }
}
