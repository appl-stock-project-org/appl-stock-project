using Microsoft.EntityFrameworkCore;
using AppleStockAPI.Models;
using AppleStockAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Enable sqlite with connection string and builder
var connectionString = builder.Configuration.GetConnectionString("Stock") ?? "Data Source=Stock.db";
builder.Services.AddSingleton<SystemController>();

var app = builder.Build();

SystemController systemController = new();

/*
    Example POST request body/payload to endpoint "/bid"
    {
        "price": 80,
        "quantity": 30
    }
*/
app.MapGet("/", () => "Hello World!");
app.MapGet("/trades", () => systemController.ListTrades());


app.MapPost("/offer", (Offer payload) => systemController.HandleOffer(payload));
app.MapPost("/bid", (Bid payload) => systemController.HandleBid(payload));
app.Run();

public partial class Program { }