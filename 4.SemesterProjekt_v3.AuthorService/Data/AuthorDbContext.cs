using _4.SemesterProjekt_v3.AuthorService.Entities;
using Microsoft.EntityFrameworkCore;

namespace _4.SemesterProjekt_v3.AuthorService.Data
{
    public class AuthorDbContext : DbContext
    {
        public AuthorDbContext(DbContextOptions<AuthorDbContext> options) : base(options) { }

        public DbSet<Author> Author { get; set; }
        public DbSet<IntegrationEvent> IntegrationEvent { get; set; }
    }
}
