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
    private readonly ILogger<PenaltyController> _logger;

    public PenaltyController(AppDbContext db, ILogger<PenaltyController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        IQueryable<Penalty> query = _db.Penalties
            .Include(p => p.Vehicle)
            .Include(p => p.CreatedByUser);

        if (User.IsInRole("Finans"))
        {
            query = query.Where(p =>
                p.History.Any(h => h.ToStatus == PenaltyStatus.FinansOnayi));
        }

        var penalties = await query
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

        if (User.IsInRole("Finans") &&
            !penalty.History.Any(h => h.ToStatus == PenaltyStatus.FinansOnayi))
        {
            return NotFound();
        }

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

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ceza kaydedilemedi.");
            ModelState.AddModelError("", "Ceza kaydedilirken bir hata oluştu.");
            await FillVehiclesAsync(vm);
            return View(vm);
        }

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

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ceza güncellenemedi. Id: {Id}", vm.Id);
            ModelState.AddModelError("", "Ceza güncellenirken bir hata oluştu.");
            await FillVehiclesAsync(vm);
            return View(vm);
        }

        TempData["Success"] = "Ceza güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Personel")]