using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Data;
using RealTimeChat.Services;
using System;


var builder = WebApplication.CreateBuilder(args);

// ---Завантаження конфігурації---
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


//  ---Додаємо сервіси---
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});


//  ---Підключення до БД---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlDatabase"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));


// ---Налаштування SignalR---
builder.Services.AddSignalR().AddAzureSignalR(
    builder.Configuration.GetConnectionString("AzureSignalR"));

// ---Реєстрація ISentimentService - Сервіс для аналізу тональності---
builder.Services.AddSingleton<ISentimentService>(sp =>
{
    var endpoint = builder.Configuration["CognitiveServices:Endpoint"]
        ?? throw new ArgumentNullException("Missing CognitiveServices:Endpoint");

    var key = builder.Configuration["CognitiveServices:Key"]
        ?? throw new ArgumentNullException("Missing CognitiveServices:Key");

    return new SentimentService(endpoint, key);
});

var app = builder.Build();

// ---Автоматичне застосування міграцій---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retryCount = 3;
    var retryDelay = TimeSpan.FromSeconds(5);

    for (int i = 0; i < retryCount; i++)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (SqlException ex) when (i < retryCount - 1)
        {
            Console.WriteLine($"Migration failed (attempt {i + 1}), retrying... Error: {ex.Message}");
            await Task.Delay(retryDelay);
        }
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();