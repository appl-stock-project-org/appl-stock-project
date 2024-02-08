using Microsoft.EntityFrameworkCore;
using AppleStockAPI.Models;
var builder = WebApplication.CreateBuilder(args);

// Enable sqlite with connection string and builder
var connectionString = builder.Configuration.GetConnectionString("Stock") ?? "Data Source=Stock.db";
builder.Services.AddSqlite<StockDb>(connectionString);

// !!!THIS IS DUMMY!!!
var dummyProgram = new DummyProgram();
var percentage = 100;
var grade = dummyProgram.GetGradesByPercentage(percentage);
// !!!THIS IS DUMMY!!!

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/stocks", async (StockDb db) => await db.Stocks.ToListAsync());
app.Run();
