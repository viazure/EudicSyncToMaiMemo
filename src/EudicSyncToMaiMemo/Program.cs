using EudicSyncToMaiMemo.Extensions.ServiceExtensions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Services.BackgroundServices;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application");

    var builder = Host.CreateApplicationBuilder(args);

    // Configures the app to work as a Windows Service.
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "Eudic Sync To MaiMemo Service";
    });

    // Add the hosted service for synchronization.
    builder.Services.AddHostedService<SyncBackgroundService>();

    // Set up Serilog using the provided configuration.
    builder.Services.AddSerilogSetup(builder.Configuration);

    // Add additional services.
    builder.Services.AddAdditionalServices();

    // Add the HttpClient.
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IHttpHelper, HttpHelper>();

    using IHost host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
