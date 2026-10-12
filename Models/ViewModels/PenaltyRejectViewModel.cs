using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimApp.Models.ViewModels;

public class PenaltyRejectViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ret nedeni zorunludur.")]
    [StringLength(300, ErrorMessage = "Ret nedeni en fazla 300 karakter olabilir.")]
    [Display(Name = "Ret Nedeni")]
    public string Reason { get; set; } = "";
}