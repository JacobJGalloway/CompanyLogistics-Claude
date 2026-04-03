using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "Implementation Example - Sqlite 3", "WarehouseData.db3");
builder.Services.AddDbContext<InventoryContext>(options => options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IClothingRepository, ClothingRepository>();
builder.Services.AddScoped<IPPERepository, PPERepository>();
builder.Services.AddScoped<IToolRepository, ToolRepository>();
builder.Services.AddControllers();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<InventoryContext>().Database.EnsureCreated();
}

app.UseRouting();
app.MapControllers();

app.Run();
