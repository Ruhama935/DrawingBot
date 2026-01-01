using Google.GenAI;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Repositories;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace server.Services
{
    public class DrawingService : IDrawingService
    {
        private readonly IAiService _aiService;
        private readonly IDrawingRepository _drawingRepository;

        public DrawingService(IDrawingRepository drawingRepository, IAiService aiService)
        {            
            _aiService = aiService;
            _drawingRepository = drawingRepository;
        }

        public async Task<GenerateDrawingResponse> GenerateDrawingAsync(GenerateDrawingRequest request)
        {            
            var aiResponse = await _aiService.AskAsync(request.Prompt);

            if (string.IsNullOrWhiteSpace(aiResponse))
                throw new Exception("AI returned empty response");

            JsonElement commands;
            try
            {
                using var doc = JsonDocument.Parse(aiResponse);
                commands = doc.RootElement.Clone();

                if (commands.ValueKind != JsonValueKind.Array)
                {
                    throw new Exception("AI response is not an array");
                }
            }
            catch
            {
                throw new Exception("AI returned invalid JSON format");
            }

            return new GenerateDrawingResponse
            {
                Commands = commands,
            };
        }
        public async Task<SaveDrawingResponse> SaveDrawingAsync(SaveDrawingRequest request)
        {
            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            var commandsJson = JsonSerializer.Serialize(request.Commands);

            if (commandsJson.Length > 50000) 
                throw new ArgumentException("Commands data is too large");            

            var drawing = new Drawing
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PromptText = request.Prompt,
                CommandsJson = JsonSerializer.Serialize(request.Commands),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _drawingRepository.SaveDrawingAsync(drawing);

            return new SaveDrawingResponse
            {
                DrawingId = drawing.Id
            };
        }

        public async Task<GenerateDrawingResponse?> GetDrawingByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid drawing ID");

            var drawing = await _drawingRepository.GetByIdAsync(id);
            if (drawing == null) return null;

            try
            {
                return new GenerateDrawingResponse
                {
                    Commands = JsonSerializer.Deserialize<object>(drawing.CommandsJson)!
                };
            }
            catch (JsonException)
            {
                throw new Exception("Drawing data is corrupted");
            }
        }

        public async Task<IEnumerable<GenerateDrawingResponse>> GetDrawingsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            var drawings = await _drawingRepository.GetByUserIdAsync(userId);

            var results = new List<GenerateDrawingResponse>();

            foreach (var d in drawings)
            {
                try
                {
                    results.Add(new GenerateDrawingResponse
                    {
                        Commands = JsonSerializer.Deserialize<object>(d.CommandsJson)!,
                        Prompt = d.PromptText
                    });
                }
                catch (JsonException)
                {
                    continue;
                }
            }

            return results;
        }
    }
}
