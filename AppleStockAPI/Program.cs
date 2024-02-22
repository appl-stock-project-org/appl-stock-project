using Microsoft.EntityFrameworkCore;
using AppleStockAPI.Models;
using AppleStockAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Enable sqlite with connection string and builder
var connectionString = builder.Configuration.GetConnectionString("Stock") ?? "Data Source=Stock.db";
builder.Services.AddSqlite<StockDb>(connectionString);

var app = builder.Build();

// Let's leave the above db scaffolding as an example for now, in case we want to use it afterall.
// It does look quite simple, I just did the List implementation for a start
List<Bid> bids = new List<Bid>();

app.MapGet("/", () => "Hello World!");

var tradeController = new TradeController();
app.MapGet("/trades", () => tradeController.ListTrades());


/*
    Example POST request body/payload to endpoint "/bid"
    {
        "price": 80,
        "quantity": 30
    }
*/
app.MapPost("/bid", (Bid payload) => BidController.handleBid(payload, bids));
app.Run();

