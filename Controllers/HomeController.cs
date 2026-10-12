using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafikCezaYonetimApp.Models;

namespace TrafikCezaYonetimApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code)
        {
            ViewBag.Message = code switch
            {
                404 => "Aradığınız sayfa bulunamadı.",
                403 => "Bu sayfaya erişim yetkiniz yok.",
                _ => "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin."
            };

            if (code is null)
            {
                _logger.LogError("Beklenmeyen hata. RequestId: {RequestId}",
                    Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}