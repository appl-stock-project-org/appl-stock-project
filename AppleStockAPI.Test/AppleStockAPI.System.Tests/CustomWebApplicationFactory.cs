using Microsoft.AspNetCore.Mvc.Testing;
using AppleStockAPI.Controllers;

namespace AppleStockAPI.Test
{

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        public SystemController? systemController;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Customize your web host configuration here (if needed)
            // For example, configure services or set environment variables.
            // You can also use this method to seed test data into your database.

            builder.ConfigureServices(services =>
                {
                    // Register the SystemController as a singleton
                    services.AddSingleton<SystemController>();
                });

            base.ConfigureWebHost(builder);
        }
    }
}