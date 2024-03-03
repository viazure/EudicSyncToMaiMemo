using EudicSyncToMaiMemo.Models.DTOs.Eudic;

namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 欧路词典服务接口
    /// </summary>
    public interface IEudicService
    {
        Task<List<BookDto>> GetAllBooks();

        Task<List<string>> GetWords(string? bookId = null);
    }
}
