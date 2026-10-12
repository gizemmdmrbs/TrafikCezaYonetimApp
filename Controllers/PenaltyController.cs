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
    public async Task<IActionResult> Details(int id)
    {
        var penalty = await _db.Penalties
            .Include(p => p.Vehicle)
            .Include(p => p.CreatedByUser)
            .Include(p => p.History)
                .ThenInclude(h => h.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (penalty == null) return NotFound();

        return View(penalty);
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PenaltyCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await FillVehiclesAsync(vm);
            return View(vm);
        }

        var userId = GetUserId();

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

    [Authorize(Roles = "Personel")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var penalty = await _db.Penalties.FindAsync(id);
        if (penalty == null) return NotFound();

        if (!CanModify(penalty))
        {
            TempData["Error"] = "Bu ceza artık düzenlenemez.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new PenaltyEditViewModel
        {
            Id = penalty.Id,
            VehicleId = penalty.VehicleId,
            Description = penalty.Description,
            Amount = penalty.Amount,
            PenaltyDate = penalty.PenaltyDate
        };

        await FillVehiclesAsync(vm);
        return View(vm);
    }

    [Authorize(Roles = "Personel")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PenaltyEditViewModel vm)
    {
        var penalty = await _db.Penalties.FindAsync(vm.Id);
        if (penalty == null) return NotFound();

        if (!CanModify(penalty))
        {
            TempData["Error"] = "Bu ceza artık düzenlenemez.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            await FillVehiclesAsync(vm);
            return View(vm);
        }

        penalty.VehicleId = vm.VehicleId!.Value;
        penalty.Description = vm.Description.Trim();
        penalty.Amount = vm.Amount!.Value;
        penalty.PenaltyDate = vm.PenaltyDate!.Value;

        _db.PenaltyHistories.Add(new PenaltyHistory
        {
            PenaltyId = penalty.Id,
            UserId = GetUserId(),
            ActionType = PenaltyActionType.Guncellendi,
            FromStatus = penalty.Status,
            ToStatus = penalty.Status,
            Note = "Ceza bilgileri güncellendi."
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Ceza güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Personel")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var penalty = await _db.Penalties.FindAsync(id);
        if (penalty == null) return NotFound();

        if (!CanModify(penalty))
        {
            TempData["Error"] = "Bu ceza artık silinemez.";
            return RedirectToAction(nameof(Index));
        }

        _db.Penalties.Remove(penalty);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Ceza silindi.";
        return RedirectToAction(nameof(Index));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool CanModify(Penalty penalty) =>
        penalty.CreatedByUserId == GetUserId() &&
        penalty.Status == PenaltyStatus.YoneticiOnayi;

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