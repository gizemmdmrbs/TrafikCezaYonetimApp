using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimApp.Models.ViewModels;

public class PenaltyCreateViewModel
{
    [Required(ErrorMessage = "Araç seçilmelidir.")]
    [Display(Name = "Araç")]
    public int? VehicleId { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(0.01, 1000000, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
    [Display(Name = "Tutar (TL)")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "Ceza tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ceza Tarihi")]
    public DateTime? PenaltyDate { get; set; }

    [ValidateNever]
    public List<SelectListItem> Vehicles { get; set; } = new();
}