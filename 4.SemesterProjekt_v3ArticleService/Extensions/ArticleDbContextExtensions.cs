using _4.SemesterProjekt_v3ArticleService.Data;
using Microsoft.EntityFrameworkCore;

namespace _4.SemesterProjekt_v3ArticleService.Extensions
{
    public static class ArticleDbContextExtensions
    {
        public static void AddArticleDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ArticleDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("ArticleDb"));
            });
        }

        public static void EnsureArticleDbIsCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ArticleDbContext>();
            context.Database.EnsureCreated();
            context.Database.CloseConnection();
        }
    }
}
