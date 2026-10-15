using Microsoft.AspNetCore.Identity;
using TrafikCezaYonetimApp.Models.Entities;

namespace TrafikCezaYonetimApp.Data
{
    public class DbCreateTestUser
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any()) return;

            var hasher = new PasswordHasher<User>();

            var users = new[]
            {
            new User { UserName = "personel", Role = Role.Personel },
            new User { UserName = "yonetici", Role = Role.Yonetici },
            new User { UserName = "finans",   Role = Role.Finans }
            };

            foreach (var u in users)
                u.PasswordHash = hasher.HashPassword(u, "Test123!");

            db.Users.AddRange(users);
            db.SaveChanges();
        }
    }
}
