using System.ComponentModel.DataAnnotations;
using TrafikCezaYonetimApp.Models.Entities;

namespace TrafikCezaYonetimApp.Models.ViewModels
{
    public class VehicleCreateViewModel
    {
        [Required(ErrorMessage = "Plaka zorunludur.")]
        [StringLength(20, ErrorMessage = "Plaka en fazla 20 karakter olabilir.")]
        [Display(Name = "Plaka")]
        public string Plate { get; set; } = "";

        [Required(ErrorMessage = "Araç tipi seçilmelidir.")]
        [Display(Name = "Araç Tipi")]
        public VehicleType? VehicleType { get; set; }

        [Required(ErrorMessage = "Marka zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = "";

        [Required(ErrorMessage = "Model zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Model")]
        public string Model { get; set; } = "";
    }
}
