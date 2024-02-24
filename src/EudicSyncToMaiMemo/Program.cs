using EudicSyncToMaiMemo.Infrastructure.ServiceExtensions;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting application");

    IHostBuilder builder = Host.CreateDefaultBuilder(args);

    // �йܷ���������ע��ķ���ӿ�
    builder.AddServices();

    // Serilog ��־
    builder.AddSerilogSetup();

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
