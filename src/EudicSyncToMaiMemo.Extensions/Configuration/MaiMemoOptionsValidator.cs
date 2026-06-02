using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Services.Helpers;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Extensions.Configuration
{
    /// <summary>
    /// 按 Provider 校验 MaiMemo 必填字段
    /// </summary>
    public sealed class MaiMemoOptionsValidator : IValidateOptions<MaiMemoOptions>
    {
        /// <summary>
        /// 校验 DefaultNotepadId 及 Provider 对应的认证字段
        /// </summary>
        /// <param name="name">Options 注册名称</param>
        /// <param name="options">待校验配置</param>
        public ValidateOptionsResult Validate(string? name, MaiMemoOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.DefaultNotepadId))
            {
                return ValidateOptionsResult.Fail("没有配置默认的云词库 ID");
            }

            return options.Provider switch
            {
                MaiMemoProvider.OpenApi when string.IsNullOrWhiteSpace(options.Token) =>
                    ValidateOptionsResult.Fail("墨墨开放 API Token 为空"),
                MaiMemoProvider.OpenApi
                    when !MaiMemoNotepadIdHelper.IsOpenApiNotepadId(options.DefaultNotepadId)
                         && string.IsNullOrWhiteSpace(options.DefaultNotepadTitle) =>
                    ValidateOptionsResult.Fail(
                        "Open API 模式需配置 np- 开头的 DefaultNotepadId，或配置 DefaultNotepadTitle 按标题匹配"),
                MaiMemoProvider.Legacy when string.IsNullOrWhiteSpace(options.Username) =>
                    ValidateOptionsResult.Fail("墨墨背单词用户名为空"),
                MaiMemoProvider.Legacy when string.IsNullOrWhiteSpace(options.Password) =>
                    ValidateOptionsResult.Fail("墨墨背单词密码为空"),
                _ => ValidateOptionsResult.Success
            };
        }
    }
}
