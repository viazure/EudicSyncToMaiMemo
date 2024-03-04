# EudicSyncToMaiMemo

[欧路词典](https://www.eudic.net/v4/en/app/eudic) 生词本同步到 [墨墨背单词](https://www.maimemo.com/) 云词库。

## 介绍

由于欧路词典提供了全平台的客户端，并且支持可定制的词库，我在日常生活与工作中都使用欧路词典来收集生词。然后我会定期将新的生词导入到更专注于背单词的软件——墨墨背单词中进行记忆。为了减少重复的工作，我开发了这个项目。

### 流程图

![flow.excalidraw.png](doc/img/flow.excalidraw.png)

## 使用说明

1. 获取欧路词典的开放 API 的授权信息。<https://my.eudic.net/OpenAPI/Authorization)>
2. 登录[墨墨背单词网页版](https://www.maimemo.com/)，进入「我的编辑」-「云词库」。
3. 新建或选择需要同步的云词库，复制网址中的云词库 ID。例如 `https://www.maimemo.com/notepad/detail/1234567?scene=`,这里的 `1234567` 就是当前云词库的 ID。
4. 修改项目配置文件，填入相应内容。

   **项目配置 `appsettings.json`：**

   ```json
   {
     "Eudic": {
       "Authorization": "NIS XXX",
       "DefaultBookId": "0"
     },
     "MaiMemo": {
       "Username": "your_username",
       "Password": "your_password",
       "DefaultNotepadId": "0"
     }
   }
   ```

   **字段说明：**

   | 来源       | 字段名           | 说明                                     | 必填  |
   | ---------- | ---------------- | ---------------------------------------- | ----- |
   | 欧路词典   | Authorization    | 接口授权，有了这个授权才能请求后续的接口 | True  |
   | 欧路词典   | DefaultBookId    | 默认同步的生词本 Id，默认生词本 id 为 0  | False |
   | 墨墨背单词 | Username         | 用于登录的用户名（邮箱或手机号）         | True  |
   | 墨墨背单词 | Password         | 用于登录的密码                           | True  |
   | 墨墨背单词 | DefaultNotepadId | 默认同步的云词库 id                      | False |

5. 运行 EudicSyncToMaiMemo.exe。

## Todo

- [ ] （开发中 🧑‍💻）MVP 版本：默认词库自动同步（控制台程序）
- [ ] 可注册为 Windows 服务，并定期执行
- [ ] 接入消息通知服务
- [ ] 接入 Telegram Bot，用于手动选择词库同步
