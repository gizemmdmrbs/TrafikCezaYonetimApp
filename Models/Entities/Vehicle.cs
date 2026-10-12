namespace TrafikCezaYonetimApp.Models.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Plate { get; set; } = "";
        public VehicleType VehicleType { get; set; }
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
