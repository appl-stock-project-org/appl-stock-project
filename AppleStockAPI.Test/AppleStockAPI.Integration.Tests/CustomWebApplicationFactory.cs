using Microsoft.AspNetCore.Mvc.Testing;

namespace AppleStockAPI.Test
{

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Customize your web host configuration here (if needed)
            // For example, configure services or set environment variables.
            // You can also use this method to seed test data into your database.


            // Call the base configuration
            base.ConfigureWebHost(builder);
        }

    }

}