using EudicSyncToMaiMemo.Infrastructure.Helpers;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    public class JsonHelperTests
    {
        [Fact]
        public void ObjToJson_WithValidObject_ReturnsJsonString()
        {
            // Arrange
            var obj = new { Name = "John", Age = 30 };

            // Act
            var jsonString = JsonHelper.ObjToJson(obj);

            // Assert
            Assert.NotNull(jsonString);
            Assert.NotEmpty(jsonString);
        }

        [Fact]
        public void JsonToObj_WithValidJsonString_ReturnsObject()
        {
            // Arrange
            var jsonString = "{\"Name\":\"John\",\"Age\":30}";

            // Act
            var obj = JsonHelper.JsonToObj<object>(jsonString);

            // Assert
            Assert.NotNull(obj);
        }

        [Fact]
        public void JsonToObj_WithInvalidJsonString_ReturnsDefault()
        {
            // Arrange
            var invalidJsonString = "invalid json string";

            // Act
            var obj = JsonHelper.JsonToObj<object>(invalidJsonString);

            // Assert
            Assert.Null(obj);
        }

        [Fact]
        public void JsonToObj_WithEmptyJsonString_ReturnsDefault()
        {
            // Arrange
            var emptyJsonString = "";

            // Act
            var obj = JsonHelper.JsonToObj<object>(emptyJsonString);

            // Assert
            Assert.Null(obj);
        }
    }
}
