using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    /// <summary>
    /// 墨墨 Open API 云词本 DTO 反序列化测试
    /// </summary>
    public class NotepadDtoTests
    {
        private const string SampleGetNotepadJson = """
            {
              "errors": [],
              "data": {
                "notepad": {
                  "id": "np-1rhmOwcfmnFKW1Bt4P965UQPyFpV6JE0jjV_dRoflR59ROkVbXPYOYrZeQctE2hX",
                  "type": "NOTEPAD",
                  "status": "UNPUBLISHED",
                  "content": "apple\nbanana",
                  "title": "常用词汇",
                  "brief": "常用",
                  "tags": [],
                  "list": [
                    { "type": "CHAPTER", "chapter": "单词列表" },
                    { "type": "WORD", "chapter": "单词列表", "word": "apple" }
                  ],
                  "created_time": "2023-03-13T16:00:00.000Z",
                  "updated_time": "2023-03-13T16:00:00.000Z"
                }
              },
              "success": true
            }
            """;

        private const string SampleListNotepadsJson = """
            {
              "errors": [],
              "data": {
                "notepads": [
                  {
                    "id": "np-aaa",
                    "type": "NOTEPAD",
                    "status": "PUBLISHED",
                    "title": "生词",
                    "brief": "简介",
                    "tags": [],
                    "created_time": "2023-03-13T16:00:00.000Z",
                    "updated_time": "2023-03-13T16:00:00.000Z"
                  }
                ]
              },
              "success": true
            }
            """;

        [Fact]
        public void JsonToObj_GetNotepadResponse_ParsesEnvelope()
        {
            var response = JsonHelper.JsonToObj<EnvelopeDto<NotepadPayloadDto>>(SampleGetNotepadJson);

            Assert.NotNull(response);
            Assert.True(response!.Success);
            Assert.NotNull(response.Data?.Notepad);
            Assert.StartsWith("np-", response.Data!.Notepad!.Id);
            Assert.Equal("NOTEPAD", response.Data.Notepad.Type);
            Assert.Equal("UNPUBLISHED", response.Data.Notepad.Status);
            Assert.Contains("apple", response.Data.Notepad.Content);
            Assert.Equal(2, response.Data.Notepad.List!.Count);
            Assert.Equal("apple", response.Data.Notepad.List[1].Word);
        }

        [Fact]
        public void JsonToObj_ListNotepadsResponse_ParsesEnvelope()
        {
            var response = JsonHelper.JsonToObj<EnvelopeDto<NotepadListPayloadDto>>(SampleListNotepadsJson);

            Assert.NotNull(response);
            Assert.Single(response!.Data!.Notepads!);
            Assert.Equal("生词", response.Data.Notepads![0].Title);
            Assert.Equal("PUBLISHED", response.Data.Notepads[0].Status);
        }

        [Fact]
        public void ObjToJson_UpdateRequest_OnlyContainsWritableFields()
        {
            var request = new NotepadUpdateRequestDto
            {
                Notepad = new NotepadUpdateDto
                {
                    Status = "PUBLISHED",
                    Content = "apple",
                    Title = "常用词汇",
                    Brief = "常用",
                    Tags = ["考研"]
                }
            };

            string json = JsonHelper.ObjToJson(request);

            Assert.Contains("\"status\"", json);
            Assert.Contains("\"content\"", json);
            Assert.DoesNotContain("\"id\"", json);
            Assert.DoesNotContain("\"list\"", json);
            Assert.DoesNotContain("\"created_time\"", json);
        }
    }
}
