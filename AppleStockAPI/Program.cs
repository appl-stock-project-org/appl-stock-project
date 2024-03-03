using Microsoft.EntityFrameworkCore;
using AppleStockAPI.Models;
using AppleStockAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Enable sqlite with connection string and builder
var connectionString = builder.Configuration.GetConnectionString("Stock") ?? "Data Source=Stock.db";
builder.Services.AddSqlite<StockDb>(connectionString);

var app = builder.Build();

// Let's leave the above db scaffolding as an example for now, in case we want to use it afterall.

app.MapGet("/", () => "Hello World!");

var tradeController = new TradeController();
app.MapGet("/trades", () => tradeController.ListTrades());

// This starts the hourly API requests and saves the price to itself. Give this to the "system controller" also
ExternalCallController apiCaller = new();

BidController bidController = new();
/*
    Example POST request body/payload to endpoint "/bid"
    {
        "price": 80,
        "quantity": 30
    }
*/
const double MOCK_PRICE = 100;
app.MapPost("/bid", (Bid payload) => bidController.HandleBid(payload, MOCK_PRICE));
app.Run();

public partial class Program { }