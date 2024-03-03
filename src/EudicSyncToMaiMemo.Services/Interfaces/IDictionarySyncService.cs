namespace EudicSyncToMaiMemo.Services.Interfaces
{
    public interface IDictionarySyncService
    {
        Task SyncDictionariesAsync(CancellationToken stoppingToken);
    }
}
