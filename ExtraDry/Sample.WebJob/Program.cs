﻿using ExtraDry.Server.DataWarehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.WebJob;

var netCoreEnvironment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
var isDevelopment = string.IsNullOrEmpty(netCoreEnvironment) || string.Compare(netCoreEnvironment, "development", StringComparison.OrdinalIgnoreCase) == 0;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddEnvironmentVariables();
if(isDevelopment) {
    configurationBuilder.AddUserSecrets<Program>();
}
var configuration = configurationBuilder.Build();

var builder = new HostBuilder();
builder.ConfigureAppConfiguration(config => {
    config.SetBasePath(Environment.CurrentDirectory);
    config.AddEnvironmentVariables();
    if(isDevelopment) {
        config.AddUserSecrets<Program>();
    }
});
builder.ConfigureWebJobs(b => {
    b.AddAzureStorageCoreServices();
    b.AddAzureStorage();
    b.AddTimers();
});
builder.ConfigureLogging((context, b) => {
    b.AddConsole();
    b.AddApplicationInsightsWebJobs();
    //b.SetMinimumLevel(LogLevel.Debug); // from user secrets
});
builder.ConfigureServices(services => {
    services.AddLogging();
    //services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));
    services.AddSingleton(factory => configuration);
    services.AddScoped<SampleWarehouseModel>();
    services.AddSqlServer<SampleContext>(configuration.GetConnectionString("OltpDatabase"));
    services.AddSqlServer<WarehouseContext>(configuration.GetConnectionString("OlapDatabase"));
    services.AddDataFactory<SampleWarehouseModel, SampleContext, WarehouseContext>(options => {
        options.BatchSize = 10;
        options.AutoMigrations = true;
    });
});
using var host = builder.Build();
await host.RunAsync();

