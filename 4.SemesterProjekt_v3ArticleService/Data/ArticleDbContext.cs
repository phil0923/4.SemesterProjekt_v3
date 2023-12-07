using _4.SemesterProjekt_v3ArticleService.Entities;
using Microsoft.EntityFrameworkCore;

namespace _4.SemesterProjekt_v3ArticleService.Data
{
    public class ArticleDbContext : DbContext
    {
        public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }

        public DbSet<Article> Article { get; set; }
        public DbSet<Author> Author { get; set; }
    }
}
