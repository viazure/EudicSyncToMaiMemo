namespace EudicSyncToMaiMemo.Services.Interfaces
{
    public interface IDictionarySyncService
    {
        Task SyncDictionaries(CancellationToken stoppingToken);
    }
}
