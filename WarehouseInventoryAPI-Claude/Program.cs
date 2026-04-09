using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WarehouseInventory_Claude.Data;
using WarehouseInventory_Claude.Data.Interfaces;
using WarehouseInventory_Claude.Data.Sync;
using WarehouseInventory_Claude.Models;
using WarehouseInventory_Claude.Services;
using WarehouseInventory_Claude.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var httpsPort = builder.Configuration.GetValue<int>("Ports:Https", 7000);
builder.WebHost.UseUrls($"https://localhost:{httpsPort}");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadInventory",
        policy => policy.RequireClaim("permissions", "read:inventory"));
});

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Sqlite 3 Implementation", "WarehouseData.db3");
var readDbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Sqlite 3 Implementation", "WarehouseRead.db3");

var syncChannel = Channel.CreateUnbounded<SyncJob>();
builder.Services.AddSingleton(syncChannel.Writer);
builder.Services.AddSingleton(syncChannel.Reader);

builder.Services.AddSingleton<InventorySyncInterceptor>();
builder.Services.AddDbContext<InventoryContext>((sp, options) =>
{
    options.UseSqlite($"Data Source={dbPath}");
    options.AddInterceptors(sp.GetRequiredService<InventorySyncInterceptor>());
});
builder.Services.AddDbContext<InventoryReadContext>(options => options.UseSqlite($"Data Source={readDbPath}"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClothingService, ClothingService>();
builder.Services.AddScoped<IPPEService, PPEService>();
builder.Services.AddScoped<IToolService, ToolService>();
builder.Services.AddHostedService<InventorySyncWorker>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var writeCtx = scope.ServiceProvider.GetRequiredService<InventoryContext>();
    var readCtx = scope.ServiceProvider.GetRequiredService<InventoryReadContext>();
    writeCtx.Database.EnsureCreated();
    readCtx.Database.EnsureCreated();
}

// Enqueue an initial full sync so the read DB is populated on startup
syncChannel.Writer.TryWrite(new SyncJob(new HashSet<Type> { typeof(Clothing), typeof(PPE), typeof(Tool) }));

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
