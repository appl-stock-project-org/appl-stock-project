using AppleStockAPI.Controllers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleStockAPI.Unit.Tests
{
    public class ExternalCallControllerTests
    {
        private static ExternalCallController controller;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            controller = new ExternalCallController();
            await Task.Delay(5000);
        }

        [Test]
        public async Task Controllers_Price_Matches_Fetched_Price()
        {
            HttpClient httpClient = new();
            await using Stream stream = await httpClient.GetStreamAsync("https://api.marketdata.app/v1/stocks/quotes/AAPL");
            
            FetchedObject? fetchedObject =
                    await JsonSerializer.DeserializeAsync<FetchedObject>(stream);

            double? expectedPrice = null;
            if (fetchedObject != null && fetchedObject.Last != null) {
                expectedPrice =  fetchedObject?.Last[0];
            }

            double result = controller.GetLastFetchedPrice();

            Assert.That(result, Is.EqualTo(expectedPrice));
        }

        public class FetchedObject
        {
            [property: JsonPropertyName("last")]
            public List<double>? Last { get; set; }
        }
    }   
}
