using Microsoft.EntityFrameworkCore;
using AppleStockAPI.Models;
var builder = WebApplication.CreateBuilder(args);

// Enable sqlite with connection string and builder
var connectionString = builder.Configuration.GetConnectionString("Stock") ?? "Data Source=Stock.db";
builder.Services.AddSqlite<StockDb>(connectionString);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/stocks", async (StockDb db) => await db.Stocks.ToListAsync());
app.Run();
