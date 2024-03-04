# EudicSyncToMaiMemo

[欧路词典](https://www.eudic.net/v4/en/app/eudic) 生词本同步到 [墨墨背单词](https://www.maimemo.com/) 云词库。

## 介绍

### 主要流程

![flow.excalidraw.png](doc/img/flow.excalidraw.png)

### 配置说明

```json
{
  "Eudic": {
    "Authorization": "NIS XXX",
    "DefaultBookId": default_book_id
  },
  "MaiMemo": {
    "Username": "your_username",
    "Password": "your_password",
    "DefaultNotepadId": default_notepad_id
  }
}
```

| 来源       | 字段名           | 说明                                     | 必填  |
| ---------- | ---------------- | ---------------------------------------- | ----- |
| 欧路词典   | Authorization    | 接口授权，有了这个授权才能请求后续的接口 | True  |
| 欧路词典   | DefaultBookId    | 默认同步的生词本 Id                      | False |
| 墨墨背单词 | Username         | 用于登录的用户名，邮箱或手机号           | True  |
| 墨墨背单词 | Password         | 用于登录的密码                           | True  |
| 墨墨背单词 | DefaultNotepadId | 默认同步的云词库 id                      | False |

## Todo

- [ ] 默认词库自动同步
- [ ] 接入消息通知服务
- [ ] 接入 Telegram Bot，用于手动选择词库同步
