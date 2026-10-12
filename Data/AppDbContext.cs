using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Models.Entities;

namespace TrafikCezaYonetimApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.HasIndex(u => u.UserName).IsUnique();
                user.Property(u => u.UserName).HasMaxLength(70).IsRequired();
                user.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<Vehicle>(vehicle =>
            {
                vehicle.HasIndex(v => v.Plate).IsUnique();
                vehicle.Property(v => v.Plate).HasMaxLength(20).IsRequired();
                vehicle.Property(v => v.Brand).HasMaxLength(50).IsRequired();
                vehicle.Property(v => v.Model).HasMaxLength(50).IsRequired();
            });
        }


    }
}
