using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WarehouseLogistics_Claude.Data;
using WarehouseLogistics_Claude.Data.Interfaces;
using WarehouseLogistics_Claude.Data.Sync;
using WarehouseLogistics_Claude.Models;
using WarehouseLogistics_Claude.Services;
using WarehouseLogistics_Claude.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var httpsPort = builder.Configuration.GetValue<int>("Ports:Https", 7001);
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
    options.AddPolicy("ReadBOL",
        policy => policy.RequireClaim("permissions", "read:bol"));
    options.AddPolicy("CreateBOL",
        policy => policy.RequireClaim("permissions", "create:bol"));
    options.AddPolicy("ModifyBOL",
        policy => policy.RequireClaim("permissions", "modify:bol"));
    options.AddPolicy("ManageUsers",
        policy => policy.RequireClaim("permissions", "manage:users"));
});

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Sqlite 3 Implementation", "WarehouseData.db3");
var readDbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "Sqlite 3 Implementation", "WarehouseRead.db3");

var syncChannel = Channel.CreateUnbounded<SyncJob>();
builder.Services.AddSingleton(syncChannel.Writer);
builder.Services.AddSingleton(syncChannel.Reader);

builder.Services.AddSingleton<LogisticsSyncInterceptor>();
builder.Services.AddDbContext<LogisticsContext>((sp, options) =>
{
    options.UseSqlite($"Data Source={dbPath}");
    options.AddInterceptors(sp.GetRequiredService<LogisticsSyncInterceptor>());
});
builder.Services.AddDbContext<LogisticsReadContext>(options => options.UseSqlite($"Data Source={readDbPath}"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBillOfLadingService, BillOfLadingService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddHostedService<LogisticsSyncWorker>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var writeCtx = scope.ServiceProvider.GetRequiredService<LogisticsContext>();
    var readCtx = scope.ServiceProvider.GetRequiredService<LogisticsReadContext>();
    writeCtx.Database.EnsureCreated();
    readCtx.Database.EnsureCreated();
}

// Enqueue an initial full sync so the read DB is populated on startup
syncChannel.Writer.TryWrite(new SyncJob(new HashSet<Type> { typeof(BillOfLading), typeof(LineEntry), typeof(Warehouse), typeof(Store) }));

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
