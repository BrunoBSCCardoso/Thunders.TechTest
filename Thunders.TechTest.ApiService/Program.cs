using Microsoft.OpenApi.Models;
using Thunders.TechTest.ApiService;
using Thunders.TechTest.ApiService.DataBase.Context;
using Thunders.TechTest.ApiService.Interfaces;
using Thunders.TechTest.ApiService.Producers;
using Thunders.TechTest.ApiService.Services;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Thunder.TechTest",
        Version = "v1",
        Description = "An API for generating detailed billing reports for toll units in Brazil"
    });
});

var redisConnectionString = builder.Configuration.GetConnectionString("cache");

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    throw new InvalidOperationException("Redis connection string 'cache' not found. Ensure Redis is configured via Aspire AppHost.");
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("cache");
});

var features = Features.BindFromConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddProblemDetails();

if (features.UseMessageBroker)
{
    builder.Services.AddBus(builder.Configuration, new SubscriptionBuilder());
}

if (features.UseEntityFramework)
{
    builder.Services.AddSqlServerDbContext<ThunderDbContext>(builder.Configuration);
}

//DI
builder.Services.AddScoped<IMessageSender, RebusMessageSender>();
builder.Services.AddScoped<ITollStationUsageRegisteredProducer, TollStationUsageRegisteredProducer>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRedisService, RedisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Thunder TechTest API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseAutomaticMigrations<ThunderDbContext>();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();
