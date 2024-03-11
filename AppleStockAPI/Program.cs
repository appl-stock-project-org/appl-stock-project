using AppleStockAPI.Controllers;
using System.Text.Json;
public class Program
{
    public readonly SystemController systemController;

    public Program(SystemController? sc)
    {
        // If running normally initiate a new SystemController
        if (sc == null) systemController = new SystemController();
        // If a test, parameter is a systemController
        else systemController = sc;

    }

    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        return builder;
    }

    public static void Configure(WebApplication app, SystemController systemController)
    {
        // Configure the application using the provided systemController
        app.MapGet("/trades", () => systemController.ListTrades());
        app.MapPost("/offer", (JsonElement payload) => systemController.HandleOffer(payload));
        app.MapPost("/bid", (JsonElement payload) => systemController.HandleBid(payload));
        app.Run();
    }

    public static void Main(string[] args)
    {
        var builder = CreateBuilder(args);
        var app = builder.Build();

        
        // Try to find this file. Only to be found on test run / test folder
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Properties/runtime-testconfig.json", optional: true, reloadOnChange: true);

        var configuration = configBuilder.Build();
        // Get the variable from the config
        bool isTest = configuration.GetValue<bool>("isTest");

        SystemController? systemController = null;
        if (isTest) {
            // Use the SystemController instance from the Dependency Injection container
            systemController = app.Services.GetRequiredService<SystemController>();
        }

        // Create an instance of Program with the SystemController instance, if it exists
        Program program = new Program(systemController);

        // Configure the routes and run
        Configure(app, program.systemController);
    }
}