using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace AppleStockAPI.Test
{
    public class ProgramIntegrationTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {

            var factory = new CustomWebApplicationFactory<Program>();
            _client = factory.CreateClient();

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

        [TearDown]
        public void TearDown()
        {
            // Dispose of any IDisposable resources here
            _client.Dispose();
        }
    }
}