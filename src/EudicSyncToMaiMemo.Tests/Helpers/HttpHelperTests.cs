﻿using EudicSyncToMaiMemo.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    public class HttpHelperTests
    {
        private readonly IHttpHelper _httpHelper;

        public HttpHelperTests()
        {
            // 创建 IHttpHelper 的 Mock 实例
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            // 设置模拟的行为：在调用 CreateClient 方法时返回一个虚拟的 HttpClient 实例
            mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                                 .Returns(new HttpClient());

            var mockLogger = new Mock<ILogger<HttpHelper>>();

            // 创建 HttpHelper 实例时将模拟的依赖项传递给构造函数
            _httpHelper = new HttpHelper(mockHttpClientFactory.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetAsync_WithValidUri_ReturnsResponse()
        {
            // Act
            var response = await _httpHelper.GetAsync("https://example.com");

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task PostAsync_WithFailedUri_ReturnsResponse()
        {
            // Arrange
            var requestJson = "{\"key\": \"value\"}";

            // Act
            var response = await _httpHelper.PostAsync("https://postman-echo.com/not-found", requestJson);

            // Assert
            Assert.Equal(string.Empty, response);
        }

        [Fact]
        public async Task PostPlainTextAsync_WithFailedUri_ReturnsResponse()
        {
            // Arrange
            var text = "example text";

            // Act
            var (response, cookie) = await _httpHelper.PostPlainTextAsync("https://postman-echo.com/not-found", text);

            // Assert
            Assert.Equal(string.Empty, response);
            Assert.Equal([], cookie);
        }

        [Fact]
        public async Task PostAsync_WithValidUriAndRequestJson_ReturnsResponse()
        {
            // Arrange
            var requestJson = "{\"key\": \"value\"}";

            // Act
            var response = await _httpHelper.PostAsync("https://postman-echo.com/post", requestJson);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task PostPlainTextAsync_WithValidUriAndText_ReturnsResponse()
        {
            // Arrange
            var text = "example text";

            // Act
            var (response, cookie) = await _httpHelper.PostPlainTextAsync("https://postman-echo.com/post", text);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.NotNull(cookie);
            Assert.NotEmpty(cookie);
        }

        [Fact]
        public async Task GetAsync_WithInvalidUri_ReturnsEmptyString()
        {
            // Act
            var response = await _httpHelper.GetAsync("invalid uri");

            // Assert
            Assert.Equal(string.Empty, response);
        }

        [Fact]
        public async Task PostAsync_WithInvalidUriAndRequestJson_ReturnsEmptyString()
        {
            // Arrange
            var requestJson = "invalid request json";

            // Act
            var response = await _httpHelper.PostAsync("invalid uri", requestJson);

            // Assert
            Assert.Equal(string.Empty, response);
        }

        [Fact]
        public async Task PostPlainTextAsync_WithInvalidUriAndText_ReturnsEmptyString()
        {
            // Arrange
            var text = "invalid text";

            // Act
            var (response, cookie) = await _httpHelper.PostPlainTextAsync("invalid uri", text);

            // Assert
            Assert.Equal(string.Empty, response);
            Assert.Equal([], cookie);
        }
    }
}
