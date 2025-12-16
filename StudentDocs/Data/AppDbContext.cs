using Microsoft.EntityFrameworkCore;
using StudentDocs.Models;

namespace StudentDocs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
    }
}
