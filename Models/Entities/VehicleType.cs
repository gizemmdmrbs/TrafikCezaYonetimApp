using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimApp.Models.Entities
{
    public enum VehicleType
    {
        [Display(Name = "Binek")]
        Binek = 1,

    [Display(Name = "Çekici")]
        Cekici = 2,

    [Display(Name = "Dorse")]
        Dorse = 3,

    [Display(Name = "Kiralık Araç")]
        KiralikArac = 4
    }
}
