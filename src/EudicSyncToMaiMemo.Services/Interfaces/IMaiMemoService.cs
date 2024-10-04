namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 墨墨背单词服务接口
    /// </summary>
    public interface IMaiMemoService
    {
        Task SyncToMaimemoNotepad(string notepadId, IEnumerable<string> eudicWords);
    }
}
