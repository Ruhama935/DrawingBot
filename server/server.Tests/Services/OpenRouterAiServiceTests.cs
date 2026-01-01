using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace server.Tests.Services
{
    public class OpenRouterAiServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public OpenRouterAiServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["OpenRouter:ApiKey"]).Returns("test-api-key");

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        }

        // AskAsync - HAPPY PATH
        [Fact]
        public async Task AskAsync_ValidPrompt_ReturnsJsonContent()
        {
            // Arrange
            var expectedResponse = new
            {
                choices = new[]
                {
                    new
                    {
                        message = new
                        {
                            content = "[{\"type\":\"circle\",\"x\":100,\"y\":100,\"radius\":50}]"
                        }
                    }
                }
            };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act
            var result = await service.AskAsync("draw a circle");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("circle", result);
            Assert.Contains("type", result);
        }

        // AskAsync - UNHAPPY PATHS
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AskAsync_EmptyOrNullPrompt_ThrowsArgumentException(string? prompt)
        {
            // Arrange
            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.AskAsync(prompt));
            Assert.Equal("Prompt cannot be empty", exception.Message);
        }

        [Fact]
        public async Task AskAsync_ApiReturnsError_ThrowsException()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AskAsync("test"));
            Assert.Contains("AI service error", exception.Message);
            Assert.Contains("500", exception.Message);
        }

        [Fact]
        public async Task AskAsync_RequestTimeout_ThrowsException()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("Request timed out"));

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AskAsync("test"));
            Assert.Equal("AI service request timed out", exception.Message);
        }

        [Fact]
        public async Task AskAsync_NetworkError_ThrowsException()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AskAsync("test"));
            Assert.Equal("Failed to connect to AI service", exception.Message);
        }

        [Fact]
        public async Task AskAsync_InvalidResponseStructure_ThrowsException()
        {
            // Arrange
            var invalidResponse = new { error = "invalid structure" };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(invalidResponse))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AskAsync("test"));
            Assert.Equal("AI service returned invalid response structure", exception.Message);
        }

        [Fact]
        public async Task AskAsync_EmptyContent_ThrowsException()
        {
            // Arrange
            var emptyContentResponse = new
            {
                choices = new[]
                {
                    new { message = new { content = "" } }
                }
            };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(emptyContentResponse))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var service = new OpenRouterAiService(_httpClient, _mockConfiguration.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AskAsync("test"));
            Assert.Equal("AI service returned empty content", exception.Message);
        }
    }
}