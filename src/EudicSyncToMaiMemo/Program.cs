using EudicSyncToMaiMemo.Extensions.ServiceExtensions;
using EudicSyncToMaiMemo.Services.BackgroundServices;
using EudicSyncToMaiMemo.Services.Interfaces;

var syncOnce = args.Contains("--sync-once", StringComparer.OrdinalIgnoreCase)
    || string.Equals(Environment.GetEnvironmentVariable("SYNC_ONCE"), "true", StringComparison.OrdinalIgnoreCase);

var builder = Host.CreateApplicationBuilder(args);

if (!syncOnce)
{
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "Eudic Sync To MaiMemo Service";
    });
    builder.Services.AddHostedService<SyncBackgroundService>();
}

builder.Services.AddSerilogSetup(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

using IHost host = builder.Build();

if (syncOnce)
{
    using var cancellation = new CancellationTokenSource();
    Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;
        cancellation.Cancel();
    };

    try
    {
        using IServiceScope scope = host.Services.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<IDictionarySyncService>();
        await syncService.SyncDictionaries(cancellation.Token);
        return 0;
    }
    catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
    {
        ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("SyncOnce");
        logger.LogWarning("单次同步已取消");
        return 130;
    }
    catch (Exception ex)
    {
        ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("SyncOnce");
        logger.LogError(ex, "单次同步失败");
        return 1;
    }
}

await host.RunAsync();
return 0;
