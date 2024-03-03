using System.Timers;
using System.Text.Json;
using AppleStockAPI.Models;

namespace AppleStockAPI.Controllers
{
    /// <summary>
    /// Controller for fetching the last Apple stock price from the external API
    /// </summary>
    public class ExternalCallController {

        static readonly string EXTERNAL_API_ENDPOINT = "https://api.marketdata.app/v1/stocks/quotes/AAPL";

        /// <summary>
        /// Used to fire the request hourly
        /// </summary>
		private readonly System.Timers.Timer timer;
        private double lastFetchedPrice;
        /// <summary>
        /// Used to make the HTTP-requests to the external API
        /// </summary>
        private readonly HttpClient httpClient;


		public ExternalCallController() {
            httpClient = new();

            // Set interval to 1 hour
			timer = new System.Timers.Timer(60 * 60 * 1000);
            timer.Elapsed += CallExternalApiForPrice;
            timer.AutoReset = true;
            CallExternalApiForPrice(null, null);
            timer.Enabled = true;

		}


        private async void CallExternalApiForPrice(object? source, ElapsedEventArgs? e) {
            await using Stream stream = await httpClient.GetStreamAsync(EXTERNAL_API_ENDPOINT);
            StockPrice? stockPrice =
                await JsonSerializer.DeserializeAsync<StockPrice>(stream);

            if (stockPrice != null) {
                double? fetchedPrice =  stockPrice?.LastTradedPrice.First();
                lastFetchedPrice = fetchedPrice ?? lastFetchedPrice;
                Console.WriteLine($"Fetched price {fetchedPrice} at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}");
            }
        }
        
        public double GetLastFetchedPrice() {
            return lastFetchedPrice;
        }
	}
}