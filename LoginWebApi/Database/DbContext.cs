using LoginWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginWebApi.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = Guid.NewGuid(), Username = "Oleksandr", Password = "Password123" },
                new User { Id = Guid.NewGuid(), Username = "Ivan", Password = "Password456" },
                new User { Id = Guid.NewGuid(), Username = "Maria", Password = "Password789" }
            );
        }
    }
}
