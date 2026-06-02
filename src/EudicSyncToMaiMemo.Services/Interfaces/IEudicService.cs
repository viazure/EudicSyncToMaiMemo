using EudicSyncToMaiMemo.Models.DTOs.Eudic;

namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 欧路词典 Open API 生词本访问
    /// </summary>
    public interface IEudicService
    {
        /// <summary>
        /// 获取当前账号下的生词本列表
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        Task<List<BookDto>> GetAllBooks(CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页拉取指定生词本中的全部单词拼写
        /// </summary>
        /// <param name="bookId">生词本 ID，对应 API 参数 category_id</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<List<string>> GetWords(string bookId, CancellationToken cancellationToken = default);
    }
}
