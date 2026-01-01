using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class DrawingDbContext: DbContext
    {
        public DrawingDbContext(DbContextOptions<DrawingDbContext> options)
           : base(options)
        {
        }

        public DbSet<Drawing> Drawings => Set<Drawing>();
    }
}
