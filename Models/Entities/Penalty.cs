namespace TrafikCezaYonetimApp.Models.Entities;

public class Penalty
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime PenaltyDate { get; set; }
    public PenaltyStatus Status { get; set; } = PenaltyStatus.Yeni;
    public string? RejectReason { get; set; }
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<PenaltyHistory> History { get; set; } = new();
}