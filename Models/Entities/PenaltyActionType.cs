using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimApp.Models.Entities;

public enum PenaltyActionType
{
    [Display(Name = "Oluşturuldu")]
    Olusturuldu = 1,

    [Display(Name = "Onaylandı")]
    Onaylandi = 2,

    [Display(Name = "Reddedildi")]
    Reddedildi =3,
    [Display(Name = "Güncellendi")]
    Guncellendi = 4
}