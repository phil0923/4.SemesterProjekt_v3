using JwtAuthenticationManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWebApi.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<UserAccount> Users { get; set; }
    }
}
