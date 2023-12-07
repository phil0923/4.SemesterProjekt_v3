using AuthenticationWebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWebApi.Extensions
{
    public static class AuthDbContextExtensions
    {
        public static void AddAuthDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("AuthDb"));
            });
        }

        public static void EnsureAuthDbIsCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<AuthDbContext>();
            context.Database.EnsureCreated();
            context.Database.CloseConnection();
        }
    }
}
