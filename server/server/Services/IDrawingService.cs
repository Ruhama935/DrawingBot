using server.DTOs;

namespace server.Services
{
    public interface IDrawingService
    {
        Task<GenerateDrawingResponse> GenerateDrawingAsync(GenerateDrawingRequest request);
        Task<SaveDrawingResponse> SaveDrawingAsync(SaveDrawingRequest request);
        Task<GenerateDrawingResponse?> GetDrawingByIdAsync(Guid id);
        Task<IEnumerable<GenerateDrawingResponse>> GetDrawingsByUserIdAsync(Guid userId);
    }
}
