namespace TrafikCezaYonetimApp.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
