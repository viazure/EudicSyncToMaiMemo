# EudicSyncToMaiMemo

[欧路词典](https://www.eudic.net/v4/en/app/eudic) 生词本同步到 [墨墨背单词](https://www.maimemo.com/) 云词库。

## 介绍

由于欧路词典提供了全平台的客户端，并且支持可定制的词库，我在日常生活与工作中都使用欧路词典来收集生词。然后我会定期将新的生词导入到更专注于背单词的软件——墨墨背单词中进行记忆。为了减少重复的工作，我开发了这个项目。

### 流程图

![flow.excalidraw.png](doc/img/flow.excalidraw.png)

## 使用说明

### 获取授权信息

欧路词典需要先获取相应的授权信息。按照以下步骤操作：

1. 访问 [欧路词典开放平台授权页面](https://my.eudic.net/OpenAPI/Authorization)。
2. 完成登录后在页面找到授权信息。

墨墨背单词需要配置账号和密码，在后续同步服务中会自动完成登录授权。

### 获取词库 ID

#### 获取欧路词典生词本 ID

欧路词典默认的生词本 ID 是 `0`。若希望使用特定的生词本，需按照以下步骤操作：

1. 使用 API 测试工具向 [https://api.frdic.com/api/open/v1/studylist/category?language=en](https://api.frdic.com/api/open/v1/studylist/category?language=en) 发起请求。
2. 查看响应数据中的生词本列表，获取所需生词本的 ID。

#### 获取墨墨背单词云词库 ID

通过以下步骤来找到的云词库 ID：

1. 登录 [墨墨背单词网页版](https://www.maimemo.com/)。
2. 导航至「我的编辑」-「云词库」。
3. 新建云词库或选择一个已有的云词库。
4. 在云词库页面的网址中查找词库 ID（例如 `https://www.maimemo.com/notepad/detail/1234567?scene=`, 这里的 `1234567` 就是的云词库 ID）。

### 运行同步程序

同步程序支持控制台和 Windows 服务两种运行模式，用户可按需选择。

#### 控制台程序

1. 访问 [Releases 页面](https://github.com/viazure/EudicSyncToMaiMemo/releases) 并下载名为 `EudicSyncToMaiMemo-版本号-win-x64.zip` 的压缩包。
2. 解压缩下载的文件。
3. [修改配置文件](#修改配置文件)以设置相应的词库信息。
4. 双击 `EudicSyncToMaiMemo.exe` 来运行程序。每次运行都会同步一次。

#### Windows 服务

> [!WARNING]
> 在使用 msi 安装包安装或更新时，程序会自动处理 Windows 服务的安装或更新，但会重置配置文件，需重新配置，建议提前备份。

1. 访问 [Releases 页面](https://github.com/viazure/EudicSyncToMaiMemo/releases) 并下载名为 `EudicSyncToMaiMemo-版本号-win-x64.msi` 的安装文件。
2. 安装下载的 `.msi` 文件。
3. 打开目录 `C:\Program Files\Eudic Sync To MaiMemo Service` ，[修改配置文件](#修改配置文件)以设置相应的词库信息。
4. 打开 Windows 的「服务」管理器。
5. 找到并启动「Eudic Sync To MaiMemo Service」。

### 修改配置文件

> [!TIP]
> 程序支持通过配置文件中的 SyncInterval 参数自定义同步间隔。该参数的值表示同步的频率，单位为天。默认情况下，同步间隔为每 7 天一次。

**配置文件 `appsettings.json`**

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
  },
  "Notification": {
    "Enabled": false,
    "Url": "",
    "RequestBody": "",
    "Headers": ""
  }
}
```

**字段说明：**

| 来源       | 字段名           | 说明                                            | 必填  |
| ---------- | ---------------- | ----------------------------------------------- | ----- |
| 欧路词典   | Authorization    | 接口授权，有了这个授权才能请求后续的接口        | True  |
| 欧路词典   | DefaultBookId    | 默认同步的生词本 Id，默认生词本 id 为 0         | True  |
| 墨墨背单词 | Username         | 用于登录的用户名（邮箱或手机号）                | True  |
| 墨墨背单词 | Password         | 用于登录的密码                                  | True  |
| 墨墨背单词 | DefaultNotepadId | 默认同步的云词库 id                             | True  |
| 通知服务   | Enabled          | 是否启用通知                                    | False |
| 通知服务   | Url              | 通知地址                                        | False |
| 通知服务   | RequestBody      | 通知请求体，需将 JSON 字符串转义                | False |
| 通知服务   | Headers          | 通知请求头，格式："key1=value1;key2=value2;..." | False |

### 配置通知服务

消息通知服务的接入逻辑参考了 [jeessy2/ddns-go](https://github.com/jeessy2/ddns-go) 项目的通知服务实现，特此表示感谢。

#### 配置步骤

1. 打开配置文件 `appsettings.json`，找到 `Notification` 节点。
2. 将 `Enabled` 设置为 `true`，启用消息通知服务。
3. 根据您使用的服务继续设置该节点下的 `Url`、`RequestBody`、`Headers`。
4. RequestBody 为转义后的 JSON 字符串，例如：`{\"message\":\"同步完成：{content} \"}`。如果此内容为空，调用通知服务会使用 GET 请求，否则使用 POST 请求。

**支持的变量：**

| 变量名    | 说明                                                                               |
| --------- | ---------------------------------------------------------------------------------- |
| {content} | 消息通知内容。若同步成功且单词数量大于 0，内容为单词列表，否则为其他服务状态通知。 |

**示例：**

[Server 酱](https://sct.ftqq.com/r/15820)

```url
https://sctapi.ftqq.com/<SENDKEY>.send?title=单词同步&desp={content}
```

更多消息通知配置可以参考：[ddns-go Webhook 配置参考](https://github.com/jeessy2/ddns-go/issues/327)

## 项目依赖

- [.NET 9 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) 17.12 或更高版本

## 问题排查

当程序出现异常时，可以查看日志文件来排查问题。日志文件默认存储在 `%PROGRAMDATA%\EudicSyncToMaiMemo\logs` 目录中，若需要更改日志文件的存放位置，可以修改配置文件中的 Serilog 节点，具体操作是调整 WriteTo 部分中 Args 下的 path 参数。

## Todo

- [x] MVP 版本：默认词库自动同步（控制台程序）
- [x] 可注册为 Windows 服务，并定期执行
- [x] 接入消息通知服务
- [ ] 接入 墨墨开放 API，替换现有的网页解析方案
- [ ] ~~接入 Telegram Bot，用于手动选择词库同步（放弃）~~，计划做一个可操作的界面，练习一下前端开发
- [ ] 调整项目打包方式，支持多平台部署
