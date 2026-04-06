using Microsoft.EntityFrameworkCore;
using WarehouseInventory_Claude.Data;
using WarehouseInventory_Claude.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Sqlite 3 Implementation", "WarehouseData.db3");
builder.Services.AddDbContext<InventoryContext>(options => options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<InventoryContext>().Database.EnsureCreated();
}

app.UseRouting();
app.MapControllers();

app.Run();
