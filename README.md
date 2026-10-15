# Trafik Cezası Yönetim Modülü

## Proje Hakkında
Araç kayıtlarının yönetilmesini, trafik cezalarının oluşturulmasını ve rol bazlı onay süreçlerinin yürütülmesini sağlayan bir ASP.NET Core MVC uygulamasıdır.

## Özellikler
- **Rol Bazlı Yetkilendirme:** Personel, Yönetici ve Finans rolleri.
- **Araç Yönetimi:** Araç ekleme, listeleme, düzenleme ve silme.
- **Ceza Yönetimi:** Ceza oluşturma, listeleme, düzenleme ve silme.
- **Onay Süreci:** Yönetici ve Finans onay mekanizması, ret nedeni zorunluluğu.
- **İşlem Geçmişi:** Kullanıcı, tarih, işlem ve durum değişikliklerinin kaydedilmesi.
- **Güvenlik ve Hata Yönetimi:** Şifrelerin hash yöntemiyle saklanması, try-catch ve loglama.

## Kullanılan Teknolojiler
- C#
- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server

## Kurulum ve Çalıştırma
1. Projeyi GitHub üzerinden klonlayın.
2. `appsettings.json` dosyasındaki veritabanı bağlantısını yapılandırın.
3. Gerekliyse veritabanı migration işlemini gerçekleştirin.
4. Terminalde aşağıdaki komutları çalıştırın:

```bash
dotnet restore
dotnet run
```
5. Terminalde belirtilen yerel adres üzerinden uygulamaya erişin.
6. Varsayılan oalrak üç kullanıcı tanımlanmıştır kullanıcı rolleri,kullanıcı adları ve şifreleri aşağıdaki gibidir:

Kullanıcı Rolü : Personel , Kullanıcı Adı: personel , Şifre: Test123!
Kullanıcı Rolü : Yonetici , Kullanıcı Adı: yonetici , Şifre: Test123!
Kullanıcı Rolü : Finans   , Kullanıcı Adı: finans   , Şifre: Test123!
