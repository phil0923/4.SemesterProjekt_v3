using _4.SemesterProjekt_v3.AuthorService.Data;
using Microsoft.EntityFrameworkCore;

namespace _4.SemesterProjekt_v3.AuthorService.Extensions
{
    public static class AuthorDbContextExtensions
    {
        public static void AddAuthorDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthorDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("AuthorDb"));
            });
        }

        public static void EnsureAuthorDbIsCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<AuthorDbContext>();
            context.Database.EnsureCreated();
            context.Database.CloseConnection();
        }
    }
}
