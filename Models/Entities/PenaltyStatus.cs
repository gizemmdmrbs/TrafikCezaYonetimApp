using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimApp.Models.Entities;

public enum PenaltyStatus
{
    [Display(Name = "Yeni")]
    Yeni = 1,

    [Display(Name = "Yönetici Onayı Bekliyor")]
    YoneticiOnayi = 2,

    [Display(Name = "Finans Onayı Bekliyor")]
    FinansOnayi = 3,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 4,

    [Display(Name = "Reddedildi")]
    Reddedildi = 5
}