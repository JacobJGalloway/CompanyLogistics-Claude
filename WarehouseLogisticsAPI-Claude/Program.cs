using WarehouseLogistics_Claude.Controllers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var warehouseId = Environment.GetEnvironmentVariable("WAREHOUSE_ID")
    ?? throw new InvalidOperationException("WAREHOUSE_ID environment variable is not set.");

builder.Services.AddSingleton<string>(warehouseId);
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
