namespace TrafikCezaYonetimApp.Models.Entities;

public class PenaltyHistory
{
    public int Id { get; set; }

    public int PenaltyId { get; set; }
    public Penalty Penalty { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public PenaltyActionType ActionType { get; set; }
    public PenaltyStatus FromStatus { get; set; }
    public PenaltyStatus ToStatus { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}