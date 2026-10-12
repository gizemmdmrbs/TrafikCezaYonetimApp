using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Data;
using TrafikCezaYonetimApp.Models.Entities;
using TrafikCezaYonetimApp.Models.ViewModels;

namespace TrafikCezaYonetimApp.Controllers;

[Authorize]
public class PenaltyController : Controller
{
    private readonly AppDbContext _db;

    public PenaltyController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var penalties = await _db.Penalties
            .Include(p => p.Vehicle)
            .Include(p => p.CreatedByUser)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(penalties);
    }

    [Authorize(Roles = "Personel")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new PenaltyCreateViewModel { PenaltyDate = DateTime.Today };
        await FillVehiclesAsync(vm);
        return View(vm);
    }

    [Authorize(Roles = "Personel")]
    [HttpPost]
    public async Task<IActionResult> Create(PenaltyCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await FillVehiclesAsync(vm);
            return View(vm);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var penalty = new Penalty
        {
            VehicleId = vm.VehicleId!.Value,
            Description = vm.Description.Trim(),
            Amount = vm.Amount!.Value,
            PenaltyDate = vm.PenaltyDate!.Value,
            Status = PenaltyStatus.YoneticiOnayi,
            CreatedByUserId = userId
        };

        penalty.History.Add(new PenaltyHistory
        {
            UserId = userId,
            ActionType = PenaltyActionType.Olusturuldu,
            FromStatus = PenaltyStatus.Yeni,
            ToStatus = PenaltyStatus.YoneticiOnayi,
            Note = "Ceza girildi."
        });

        _db.Penalties.Add(penalty);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Ceza kaydedildi ve yönetici onayına gönderildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillVehiclesAsync(PenaltyCreateViewModel vm)
    {
        vm.Vehicles = await _db.Vehicles
            .OrderBy(v => v.Plate)
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = v.Plate + " - " + v.Brand + " " + v.Model
            })
            .ToListAsync();
    }
}