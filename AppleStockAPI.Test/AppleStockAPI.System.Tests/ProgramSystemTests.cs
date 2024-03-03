using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using AppleStockAPI.Models; 
using AppleStockAPI.Controllers;
using System.Text.Json;
using System.Text;

namespace AppleStockAPI.Test
{
    public class ProgramIntegrationTests
    {
        private HttpClient _client;

        private ExternalCallController apiCaller;

        /// <summary>
        ///  Make a POST request to the given endpoint with content of given object
        /// </summary>
        /// <param name="endPoint">An endpoint of the program's own API. Example: "/bid" </param>
        /// <param name="contentObject">Object to send with the POST request.</param>
        /// <returns></returns>
        private async Task<Response?> MakePostRequest(string endPoint, object contentObject) {

            // Convert object to json formatted string
            string jsonToPost = JsonSerializer.Serialize(contentObject);

            // Convert json string to StringContent to pass for the post operation
            StringContent contentToPost = new (jsonToPost, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await _client.PostAsync(endPoint, contentToPost);
            httpResponse.EnsureSuccessStatusCode();
            // Read the response content
            string responseJsonString = await httpResponse.Content.ReadAsStringAsync();
            // Convert from string to Response-object
            Response? finalResponseObject = JsonSerializer.Deserialize<Response>(responseJsonString);
            return finalResponseObject;
        }

        [SetUp]
        public async Task SetupAsync()
        {
            var factory = new CustomWebApplicationFactory<Program>();
            _client = factory.CreateClient();
            apiCaller = new();

            Console.WriteLine($"System Test Setup: To assure the external API request is received, delay for 5s starting at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}");
            await Task.Delay(5000);
            Console.WriteLine($"System Test Setup: Delay ended at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}. Fetched price is {apiCaller.GetLastFetchedPrice()}");

            if (apiCaller.GetLastFetchedPrice() <= 0) {
                throw new Exception("Couldn't fetch stock price from external API!");
            }

        }

        [Test]
        public async Task Should_Return_OK()
        {
            // Arrange (optional)

            // Act
            var response = await _client.GetAsync("/");

            // Assert
            var content = response.EnsureSuccessStatusCode();
            Assert.That(content, Is.EqualTo(response));

        }

        [Test]
        public async Task Should_Return_Not_Found()
        {
            // Arrange (optional)

            // Act
            var response = await _client.GetAsync("/dummy");
            var content = System.Net.HttpStatusCode.NotFound;

            // Assert
            Assert.That(content, Is.EqualTo(response.StatusCode));
        }


        [Test, Order(1)]
        public async Task Get_Trades_Should_Return_Empty()
        {
            // Arrange (optional)

            // Act
            var response = await _client.GetStringAsync("/trades");

            // Assert
            Assert.That(response, Is.EqualTo("[]"));
        }

        [Test, Order(2)]
        public async Task Post_Valid_Bid_Should_Return_Success()
        {
            var bidToPost = new{ price = 179, quantity = 10 };
            Response? response = await MakePostRequest("/bid", bidToPost);

            Console.WriteLine($"Response: {response}");

            // Assert.That(content, Is.EqualTo(response.StatusCode));
        }


        [TearDown]
        public void TearDown()
        {
            // Dispose of any IDisposable resources here
            _client.Dispose();
        }
    }
}