using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;

namespace server.Repositories
{
    public class DrawingRepository : IDrawingRepository
    {
        private readonly DrawingDbContext _context;

        public DrawingRepository(DrawingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveDrawingAsync(Drawing drawing)
        {
            if (drawing == null)
                throw new ArgumentNullException(nameof(drawing));

            try
            {
                await _context.Drawings.AddAsync(drawing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to save drawing to database", ex);
            }
        }

        public async Task<Drawing?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Drawings.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve drawing from database", ex);
            }
        }

        public async Task<List<Drawing>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                return await _context.Drawings
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(100) 
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve drawings from database", ex);
            }
        }
    }
}