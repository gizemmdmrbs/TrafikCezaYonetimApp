using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Models.Entities;

namespace TrafikCezaYonetimApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Penalty> Penalties => Set<Penalty>();
        public DbSet<PenaltyHistory> PenaltyHistories => Set<PenaltyHistory>();

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

            modelBuilder.Entity<Penalty>(e =>
            {
                e.Property(p => p.Description).HasMaxLength(500).IsRequired();
                e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                e.Property(p => p.RejectReason).HasMaxLength(300);

                e.HasOne(p => p.Vehicle).WithMany()
                    .HasForeignKey(p => p.VehicleId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.CreatedByUser).WithMany()
                    .HasForeignKey(p => p.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PenaltyHistory>(e =>
            {
                e.Property(h => h.Note).HasMaxLength(300);

                e.HasOne(h => h.Penalty).WithMany(p => p.History)
                    .HasForeignKey(h => h.PenaltyId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(h => h.User).WithMany()
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }


    }
}
