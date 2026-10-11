using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Models.Entities;

namespace TrafikCezaYonetimApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.UserName).IsUnique();
                e.Property(u => u.UserName).HasMaxLength(70).IsRequired();
                e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            });
        }
    }
}
