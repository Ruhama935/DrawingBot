using Moq;
using server.DTOs;
using server.Models;
using server.Repositories;
using server.Services;
using System.Text.Json;
using Xunit;

namespace server.Tests.Services
{
    public class DrawingServiceTests
    {
        private readonly Mock<IAiService> _mockAiService;
        private readonly Mock<IDrawingRepository> _mockRepository;
        private readonly DrawingService _service;

        public DrawingServiceTests()
        {
            _mockAiService = new Mock<IAiService>();
            _mockRepository = new Mock<IDrawingRepository>();
            _service = new DrawingService(_mockRepository.Object, _mockAiService.Object);
        }

        // GenerateDrawingAsync - HAPPY PATH
        [Fact]
        public async Task GenerateDrawingAsync_ValidPrompt_ReturnsCommands()
        {
            // Arrange
            var request = new GenerateDrawingRequest { Prompt = "draw a circle" };
            var aiResponse = "[{\"type\":\"circle\",\"x\":100,\"y\":100,\"radius\":50}]";
            _mockAiService.Setup(x => x.AskAsync(request.Prompt)).ReturnsAsync(aiResponse);

            // Act
            var result = await _service.GenerateDrawingAsync(request);

            // Assert
            Assert.NotNull(result.Commands);
            Assert.IsType<JsonElement>(result.Commands);
            _mockAiService.Verify(x => x.AskAsync(request.Prompt), Times.Once);
        }

        // GenerateDrawingAsync - UNHAPPY PATHS
        [Fact]
        public async Task GenerateDrawingAsync_EmptyAiResponse_ThrowsException()
        {
            // Arrange
            var request = new GenerateDrawingRequest { Prompt = "test" };
            _mockAiService.Setup(x => x.AskAsync(It.IsAny<string>())).ReturnsAsync("");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GenerateDrawingAsync(request));
            Assert.Equal("AI returned empty response", exception.Message);
        }

        [Fact]
        public async Task GenerateDrawingAsync_InvalidJson_ThrowsException()
        {
            // Arrange
            var request = new GenerateDrawingRequest { Prompt = "test" };
            _mockAiService.Setup(x => x.AskAsync(It.IsAny<string>())).ReturnsAsync("not valid json");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GenerateDrawingAsync(request));
            Assert.Equal("AI returned invalid JSON format", exception.Message);
        }

        [Fact]
        public async Task GenerateDrawingAsync_JsonNotArray_ThrowsException()
        {
            // Arrange
            var request = new GenerateDrawingRequest { Prompt = "test" };
            _mockAiService.Setup(x => x.AskAsync(It.IsAny<string>())).ReturnsAsync("{\"type\":\"circle\"}");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GenerateDrawingAsync(request));
            Assert.Equal("AI response is not an array", exception.Message);
        }

        // SaveDrawingAsync - HAPPY PATH
        [Fact]
        public async Task SaveDrawingAsync_ValidRequest_ReturnsSavedDrawingId()
        {
            // Arrange
            var commands = JsonSerializer.SerializeToElement(new[] { new { type = "circle", x = 100, y = 100 } });
            var request = new SaveDrawingRequest
            {
                UserId = Guid.NewGuid(),
                Prompt = "draw a circle",
                Commands = commands
            };

            _mockRepository.Setup(x => x.SaveDrawingAsync(It.IsAny<Drawing>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.SaveDrawingAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, result.DrawingId);
            _mockRepository.Verify(x => x.SaveDrawingAsync(It.Is<Drawing>(d =>
                d.UserId == request.UserId &&
                d.PromptText == request.Prompt
            )), Times.Once);
        }

        // SaveDrawingAsync - UNHAPPY PATHS
        [Fact]
        public async Task SaveDrawingAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var commands = JsonSerializer.SerializeToElement(new[] { new { type = "circle" } });
            var request = new SaveDrawingRequest
            {
                UserId = Guid.Empty,
                Prompt = "test",
                Commands = commands
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveDrawingAsync(request));
            Assert.Equal("UserId is required", exception.Message);
        }

        [Fact]
        public async Task SaveDrawingAsync_CommandsTooLarge_ThrowsArgumentException()
        {
            // Arrange
            var largeArray = Enumerable.Range(0, 10000).Select(i => new { type = "circle", x = i, y = i, radius = 50 }).ToArray();
            var commands = JsonSerializer.SerializeToElement(largeArray);
            var request = new SaveDrawingRequest
            {
                UserId = Guid.NewGuid(),
                Prompt = "test",
                Commands = commands
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveDrawingAsync(request));
            Assert.Equal("Commands data is too large", exception.Message);
        }

        // GetDrawingByIdAsync - HAPPY PATH
        [Fact]
        public async Task GetDrawingByIdAsync_ValidId_ReturnsDrawing()
        {
            // Arrange
            var drawingId = Guid.NewGuid();
            var drawing = new Drawing
            {
                Id = drawingId,
                CommandsJson = "[{\"type\":\"circle\",\"x\":100,\"y\":100}]",
                PromptText = "test prompt"
            };
            _mockRepository.Setup(x => x.GetByIdAsync(drawingId)).ReturnsAsync(drawing);

            // Act
            var result = await _service.GetDrawingByIdAsync(drawingId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Commands);
        }

        // GetDrawingByIdAsync - UNHAPPY PATHS
        [Fact]
        public async Task GetDrawingByIdAsync_EmptyId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDrawingByIdAsync(Guid.Empty));
            Assert.Equal("Invalid drawing ID", exception.Message);
        }

        [Fact]
        public async Task GetDrawingByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange
            var drawingId = Guid.NewGuid();
            _mockRepository.Setup(x => x.GetByIdAsync(drawingId)).ReturnsAsync((Drawing?)null);

            // Act
            var result = await _service.GetDrawingByIdAsync(drawingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDrawingByIdAsync_CorruptedJson_ThrowsException()
        {
            // Arrange
            var drawingId = Guid.NewGuid();
            var drawing = new Drawing
            {
                Id = drawingId,
                CommandsJson = "corrupted json data",
                PromptText = "test"
            };
            _mockRepository.Setup(x => x.GetByIdAsync(drawingId)).ReturnsAsync(drawing);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetDrawingByIdAsync(drawingId));
            Assert.Equal("Drawing data is corrupted", exception.Message);
        }

        // GetDrawingsByUserIdAsync - HAPPY PATH
        [Fact]
        public async Task GetDrawingsByUserIdAsync_ValidUserId_ReturnsDrawings()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var drawings = new List<Drawing>
            {
                new Drawing { CommandsJson = "[{\"type\":\"circle\"}]", PromptText = "circle" },
                new Drawing { CommandsJson = "[{\"type\":\"rect\"}]", PromptText = "rectangle" }
            };
            _mockRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(drawings);

            // Act
            var result = await _service.GetDrawingsByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.NotNull(r.Commands));
        }

        // GetDrawingsByUserIdAsync - UNHAPPY PATHS
        [Fact]
        public async Task GetDrawingsByUserIdAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetDrawingsByUserIdAsync(Guid.Empty));
            Assert.Equal("UserId is required", exception.Message);
        }

        [Fact]
        public async Task GetDrawingsByUserIdAsync_SkipsCorruptedDrawings()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var drawings = new List<Drawing>
            {
                new Drawing { CommandsJson = "[{\"type\":\"circle\"}]", PromptText = "valid" },
                new Drawing { CommandsJson = "corrupted", PromptText = "invalid" },
                new Drawing { CommandsJson = "[{\"type\":\"rect\"}]", PromptText = "valid2" }
            };
            _mockRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(drawings);

            // Act
            var result = await _service.GetDrawingsByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count()); // Should skip the corrupted one
        }
    }
}