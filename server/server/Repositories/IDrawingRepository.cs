using server.Models;

namespace server.Repositories
{
    public interface IDrawingRepository
    {
        Task SaveDrawingAsync(Drawing drawing);
        Task<Drawing?> GetByIdAsync(Guid id);
        Task<List<Drawing>> GetByUserIdAsync(Guid userId);
    }
}
