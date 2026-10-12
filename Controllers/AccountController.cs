using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Data;
using TrafikCezaYonetimApp.Models.Entities;
using TrafikCezaYonetimApp.Models.ViewModels;

namespace TrafikCezaYonetimApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Vehicle");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userName = model.UserName.Trim();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            var hasher = new PasswordHasher<User>();
            var valid = user != null &&
                hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password)
                    != PasswordVerificationResult.Failed;

            if (!valid)
            {
                _logger.LogWarning("Başarısız giriş denemesi. Kullanıcı adı: {UserName}", userName);
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Vehicle");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}