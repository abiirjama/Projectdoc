using Microsoft.EntityFrameworkCore;
using StudentDocs.Models;

namespace StudentDocs.Data
{
    // Database context for the application
    public class AppDbContext : DbContext
    {
        // Configure the context using dependency injection
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Represents the Documents table in the database
        public DbSet<Document> Documents { get; set; }
    }
}
