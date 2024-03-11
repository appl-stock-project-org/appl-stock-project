using AppleStockAPI.Models; 
using AppleStockAPI.Controllers;
using AppleStockAPI.Utilities; 
using System.Text.Json;
using System.Text;

namespace AppleStockAPI.Test
{
    public class ProgramSystemTests {

        protected static ExternalCallController apiCaller;

        protected static CustomWebApplicationFactory<Program> factory;

        protected static HttpClient client;

        protected static SystemController systemController;

        /// <summary>
        ///  Make a POST request to the given endpoint with content of given object
        /// </summary>
        /// <param name="endPoint">An endpoint of the program's own API. Example: "/bid" </param>
        /// <param name="contentObject">Object to send with the POST request.</param>
        /// <returns></returns>
        protected async Task<Response?> MakePostRequest(string endPoint, object contentObject)
        {

            // Convert object to json formatted string
            string jsonToPost = JsonSerializer.Serialize(contentObject);

            // Convert json string to StringContent to pass for the post operation
            StringContent contentToPost = new(jsonToPost, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await client.PostAsync(endPoint, contentToPost);
            httpResponse.EnsureSuccessStatusCode();
            // Read the response content
            string responseJsonString = await httpResponse.Content.ReadAsStringAsync();
            // Convert from string to Response-object
            Response? finalResponseObject = JsonSerializer.Deserialize<Response>(responseJsonString);
            return finalResponseObject;
        }

        /// <summary>
        /// Make a request to the /trades endpoint and parse the result to a list of trades
        /// </summary>
        /// <returns>list of trades</returns>
        protected async Task<List<Trade>> GetAndParseTrades() {
            await using Stream stream = await client.GetStreamAsync("/trades");
            List<Trade> tradesFromFetch = (await JsonSerializer.DeserializeAsync<List<Trade>>(stream))!;
            return tradesFromFetch;
        }

        [OneTimeSetUp]
        public async Task SetupAsync()
        {

            apiCaller = new();
            factory = new();
            client = factory.CreateClient();

            systemController = factory.Services.GetRequiredService<SystemController>();

            Console.WriteLine($"System Test Setup: To assure the external API request is received, delay for 5s starting at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}");
            await Task.Delay(5000);
            Console.WriteLine($"System Test Setup: Delay ended at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}. Fetched price is {apiCaller.GetLastFetchedPrice()}");

            if (apiCaller.GetLastFetchedPrice() <= 0) {
                throw new Exception("Couldn't fetch stock price from external API!");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Dispose of any IDisposable resources here
            client.Dispose();
        }
    }

    // Assignment document E2E scenario 1
    [TestFixture]
    public class Assignment_E2E_Tests_1 : ProgramSystemTests {

         [OneTimeSetUp]
         public void Setup()
         {
            systemController.Reset();
         }

        // 1. b.
        [Test, Order(1)]
        public async Task Post_Bid_With_Price_108_Percent_Of_Stock_Success()
        {
            double price = apiCaller.GetLastFetchedPrice() * 1.08;
            var bidToPost = new{ price, quantity = 10 };
            Response response = await (MakePostRequest("/bid", bidToPost) as Task<Response>);
            
            Assert.That(response.SuccessMessage, Is.EqualTo($"Bid successfully placed with the price of {MathUtils.TruncateTo2Decimals(price)} and quantity of 10."));
        }

        // 1. c. Mistake in the first E2E scenario? This one doesn't match the described outcome in it
        [Test, Order(2)]
        public async Task Post_Offer_With_Price_90_Percent_Of_Stock_Success()
        {
            double price = apiCaller.GetLastFetchedPrice() * 0.9;
            var offerToPost = new{ price, quantity = 10 };
            Response response = await (MakePostRequest("/offer", offerToPost) as Task<Response>);

            List<Trade> trades = await GetAndParseTrades();
            
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(true));
                Assert.That(response.ErrorMessage, Is.EqualTo(null));
                StringAssert.StartsWith($"Offer successfully placed with the price of {MathUtils.TruncateTo2Decimals(price)} and quantity of 10. Trade made with bid", response.SuccessMessage);
                Assert.That(trades, Has.Count.EqualTo(1));
            });
        }


        // 1. d.
        [Test, Order(3)]
        public async Task Post_Bid_With_Price_111_Percent_Of_Stock_Rejected()
        {
            double price = apiCaller.GetLastFetchedPrice() * 1.11;
            var bidToPost = new{ price, quantity = 10 };
            Response response = await (MakePostRequest("/bid", bidToPost) as Task<Response>);

            Assert.That(response.ErrorMessage, Is.EqualTo($"Bid price is too high. Highest accepted price at the moment is {Math.Round(apiCaller.GetLastFetchedPrice() * 1.1, 2)}."));

        }

        // 1. e.
        [Test, Order(4)]
        public async Task Post_Offer_With_Price_Minus_101_Percent_Of_Stock_Rejected()
        {
            double price = apiCaller.GetLastFetchedPrice() * -1.01;
            var offerToPost = new{ price, quantity = 10 };
            Response response = await (MakePostRequest("/offer", offerToPost) as Task<Response>);

            List<Trade> trades = await GetAndParseTrades();
            
            Assert.Multiple(() =>
            {
                Assert.That(response.ErrorMessage, Is.EqualTo($"Offer rejected with the value of {MathUtils.TruncateTo2Decimals(price)}, offer needs to be in the price range of 10% of the market price. Current market price is {apiCaller.GetLastFetchedPrice()}."));
                Assert.That(trades, Has.Count.EqualTo(1));
            });
        }
    }

    // Assignment document E2E scenario 2
    [TestFixture]
    public class Assignment_E2E_Tests_2 : ProgramSystemTests {

         [OneTimeSetUp]
         public void Setup()
         {
            systemController.Reset();
         }

        // 2. b.
        [Test, Order(1)]
        public async Task Post_Bid_With_Quantity_0_Rejected()
        {
            double price = apiCaller.GetLastFetchedPrice();
            var bidToPost = new{ price, quantity = 0 };
            Response response = await (MakePostRequest("/bid", bidToPost) as Task<Response>);
    
            Assert.That(response.ErrorMessage, Is.EqualTo("Bid quantity needs to be above 0."));
        }

        // 2. c. 
        [Test, Order(2)]
        public async Task Post_Bid_With_Decimal_Quantity_Rejected()
        {
            double price = apiCaller.GetLastFetchedPrice();
            var bidToPost = new{ price, quantity = 10.1 };
            Response response = await (MakePostRequest("/bid", bidToPost) as Task<Response>);
            
            Assert.That(response.ErrorMessage, Is.EqualTo("Request body didn't adhere to the structure of a valid bid."));
        }


        // 2. d. and e.
        [Test, Order(3)]
        public async Task Post_Offer_With_Negative_Quantity_Rejected()
        {
            double price = apiCaller.GetLastFetchedPrice();
            var offerToPost = new{ price, quantity = -100 };
            Response response = await (MakePostRequest("/offer", offerToPost) as Task<Response>);
            
            List<Trade> trades = await GetAndParseTrades();

            Assert.Multiple(() =>
            {
                Assert.That(response.ErrorMessage, Is.EqualTo($"Offer quantity invalid, offer should contain a quantity of larger than 0"));
                Assert.That(trades, Has.Count.EqualTo(0));
            });
        }
    }

    // Assignment document E2E scenario 3
    [TestFixture]
    public class Assignment_E2E_Tests_3 : ProgramSystemTests {

         [OneTimeSetUp]
         public void Setup()
         {
            systemController.Reset();
         }

        // Whole scenario 3
        [Test]
        public async Task Post_Offers_And_Bids_Expect_2_Trades_From_Same_Offer()
        {
            double price = apiCaller.GetLastFetchedPrice();
            // 3. b.
            var bid1 = new{ price, quantity = 100 };
            await (MakePostRequest("/bid", bid1) as Task<Response>);

            // 3. c.
            var offer1 = new{ price = price * 0.8, quantity = 200 };
            await (MakePostRequest("/offer", offer1) as Task<Response>);

            // 3. d.
            var bid2 = new{ price = price * 1.01, quantity = 200 };
            await (MakePostRequest("/bid", bid2) as Task<Response>);

            // 3. e.
            var bid3 = new{ price = price * 0.95, quantity = 50 };
            await (MakePostRequest("/bid", bid3) as Task<Response>);

            // 3. f.
            var bid4 = new{ price, quantity = 30 };
            await (MakePostRequest("/bid", bid4) as Task<Response>);

            // 3. g.
            var offer2 = new{ price, quantity = 250 };
            await (MakePostRequest("/offer", offer2) as Task<Response>);
            
            // 3. h.
            List<Trade> tradesFromFetch = await GetAndParseTrades();

            Assert.Multiple(() =>
            {
                Assert.That(tradesFromFetch, Has.Count.EqualTo(2));
                Assert.That(tradesFromFetch[0].Price, Is.EqualTo(MathUtils.TruncateTo2Decimals(price * 1.01)));
                Assert.That(tradesFromFetch[0].Quantity, Is.EqualTo(200));
                Assert.That(tradesFromFetch[1].Price, Is.EqualTo(price));
                Assert.That(tradesFromFetch[1].Quantity, Is.EqualTo(50));
            });
        }
    }
}