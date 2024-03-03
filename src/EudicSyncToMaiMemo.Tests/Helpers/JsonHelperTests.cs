using EudicSyncToMaiMemo.Infrastructure.Helpers;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    public class JsonHelperTests
    {
        [Fact]
        public void Serialize_WithValidObject_ReturnsJsonString()
        {
            // Arrange
            var obj = new { Name = "John", Age = 30 };

            // Act
            var jsonString = JsonHelper.Serialize(obj);

            // Assert
            Assert.NotNull(jsonString);
            Assert.NotEmpty(jsonString);
        }

        [Fact]
        public void Deserialize_WithValidJsonString_ReturnsObject()
        {
            // Arrange
            var jsonString = "{\"Name\":\"John\",\"Age\":30}";

            // Act
            var obj = JsonHelper.Deserialize<object>(jsonString);

            // Assert
            Assert.NotNull(obj);
        }

        [Fact]
        public void Deserialize_WithInvalidJsonString_ReturnsDefault()
        {
            // Arrange
            var invalidJsonString = "invalid json string";

            // Act
            var obj = JsonHelper.Deserialize<object>(invalidJsonString);

            // Assert
            Assert.Null(obj);
        }

        [Fact]
        public void Deserialize_WithEmptyJsonString_ReturnsDefault()
        {
            // Arrange
            var emptyJsonString = "";

            // Act
            var obj = JsonHelper.Deserialize<object>(emptyJsonString);

            // Assert
            Assert.Null(obj);
        }
    }
}
