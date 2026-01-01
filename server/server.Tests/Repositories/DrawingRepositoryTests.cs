using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.Repositories;
using Xunit;

namespace server.Tests.Repositories
{
    public class DrawingRepositoryTests : IDisposable
    {
        private readonly DrawingDbContext _context;
        private readonly DrawingRepository _repository;

        public DrawingRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DrawingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DrawingDbContext(options);
            _repository = new DrawingRepository(_context);
        }

        // SaveDrawingAsync - HAPPY PATH
        [Fact]
        public async Task SaveDrawingAsync_ValidDrawing_SavesSuccessfully()
        {
            // Arrange
            var drawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PromptText = "draw a circle",
                CommandsJson = "[{\"type\":\"circle\",\"x\":100,\"y\":100,\"radius\":50}]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await _repository.SaveDrawingAsync(drawing);

            // Assert
            var saved = await _context.Drawings.FindAsync(drawing.Id);
            Assert.NotNull(saved);
            Assert.Equal(drawing.Id, saved.Id);
            Assert.Equal(drawing.UserId, saved.UserId);
            Assert.Equal(drawing.PromptText, saved.PromptText);
            Assert.Equal(drawing.CommandsJson, saved.CommandsJson);
        }

        // SaveDrawingAsync - UNHAPPY PATH
        [Fact]
        public async Task SaveDrawingAsync_NullDrawing_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.SaveDrawingAsync(null!));
            Assert.Equal("drawing", exception.ParamName);
        }

        // GetByIdAsync - HAPPY PATH
        [Fact]
        public async Task GetByIdAsync_ExistingDrawing_ReturnsDrawing()
        {
            // Arrange
            var drawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PromptText = "test drawing",
                CommandsJson = "[{\"type\":\"rect\"}]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.Drawings.AddAsync(drawing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(drawing.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(drawing.Id, result.Id);
            Assert.Equal(drawing.PromptText, result.PromptText);
            Assert.Equal(drawing.CommandsJson, result.CommandsJson);
        }

        // GetByIdAsync - UNHAPPY PATH
        [Fact]
        public async Task GetByIdAsync_NonExistentDrawing_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        // GetByUserIdAsync - HAPPY PATH
        [Fact]
        public async Task GetByUserIdAsync_MultipleDrawings_ReturnsOrderedByDateDescending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldestDrawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PromptText = "oldest",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow
            };
            var middleDrawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PromptText = "middle",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow
            };
            var newestDrawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PromptText = "newest",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Drawings.AddRangeAsync(oldestDrawing, middleDrawing, newestDrawing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("newest", result[0].PromptText);
            Assert.Equal("middle", result[1].PromptText);
            Assert.Equal("oldest", result[2].PromptText);
        }

        [Fact]
        public async Task GetByUserIdAsync_FiltersByUserId()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var user1Drawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                PromptText = "user1",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var user2Drawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId2,
                PromptText = "user2",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Drawings.AddRangeAsync(user1Drawing, user2Drawing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId1);

            // Assert
            Assert.Single(result);
            Assert.Equal("user1", result[0].PromptText);
        }

        [Fact]
        public async Task GetByUserIdAsync_LimitsTo100Results()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var drawings = Enumerable.Range(1, 150).Select(i => new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PromptText = $"drawing {i}",
                CommandsJson = "[]",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                UpdatedAt = DateTime.UtcNow
            });

            await _context.Drawings.AddRangeAsync(drawings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(100, result.Count);
        }

        // GetByUserIdAsync - UNHAPPY PATH
        [Fact]
        public async Task GetByUserIdAsync_NoDrawings_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}