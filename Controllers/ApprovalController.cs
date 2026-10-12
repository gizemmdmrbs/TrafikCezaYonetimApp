using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Data;
using TrafikCezaYonetimApp.Models.Entities;
using TrafikCezaYonetimApp.Models.ViewModels;

namespace TrafikCezaYonetimApp.Controllers;

[Authorize(Roles = "Yonetici,Finans")]
public class ApprovalController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<ApprovalController> _logger;

    public ApprovalController(AppDbContext db, ILogger<ApprovalController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var stage = GetMyStage();

        var penalties = await _db.Penalties
            .Include(p => p.Vehicle)
            .Include(p => p.CreatedByUser)
            .Where(p => p.Status == stage)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        return View(penalties);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var penalty = await _db.Penalties.FindAsync(id);
        if (penalty == null) return NotFound();

        if (penalty.Status != GetMyStage())
        {
            TempData["Error"] = "Bu ceza sizin onay aşamanızda değil.";
            return RedirectToAction(nameof(Index));
        }

        var from = penalty.Status;
        var to = from == PenaltyStatus.YoneticiOnayi
            ? PenaltyStatus.FinansOnayi
            : PenaltyStatus.Tamamlandi;

        penalty.Status = to;

        _db.PenaltyHistories.Add(new PenaltyHistory
        {
            PenaltyId = penalty.Id,
            UserId = GetUserId(),
            ActionType = PenaltyActionType.Onaylandi,
            FromStatus = from,
            ToStatus = to,
            Note = "Onaylandı."
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ceza onaylanamadı. Id: {Id}", id);
            TempData["Error"] = "Ceza onaylanırken bir hata oluştu.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Ceza onaylandı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Reject(int id)
    {
        var penalty = await _db.Penalties.Include(p => p.Vehicle)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (penalty == null) return NotFound();

        if (penalty.Status != GetMyStage())
        {
            TempData["Error"] = "Bu ceza sizin onay aşamanızda değil.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Info = $"{penalty.Vehicle.Plate} - {penalty.Description} ({penalty.Amount:N2} TL)";
        return View(new PenaltyRejectViewModel { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(PenaltyRejectViewModel vm)
    {
        var penalty = await _db.Penalties.Include(p => p.Vehicle)
            .FirstOrDefaultAsync(p => p.Id == vm.Id);
        if (penalty == null) return NotFound();

        if (penalty.Status != GetMyStage())
        {
            TempData["Error"] = "Bu ceza sizin onay aşamanızda değil.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Info = $"{penalty.Vehicle.Plate} - {penalty.Description} ({penalty.Amount:N2} TL)";
            return View(vm);
        }

        var from = penalty.Status;
        var reason = vm.Reason.Trim();

        penalty.Status = PenaltyStatus.Reddedildi;
        penalty.RejectReason = reason;

        _db.PenaltyHistories.Add(new PenaltyHistory
        {
            PenaltyId = penalty.Id,
            UserId = GetUserId(),
            ActionType = PenaltyActionType.Reddedildi,
            FromStatus = from,
            ToStatus = PenaltyStatus.Reddedildi,
            Note = reason
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ceza reddedilemedi. Id: {Id}", vm.Id);
            TempData["Error"] = "Ceza reddedilirken bir hata oluştu.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Ceza reddedildi.";
        return RedirectToAction(nameof(Index));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private PenaltyStatus GetMyStage() =>
        User.IsInRole("Yonetici")
            ? PenaltyStatus.YoneticiOnayi
            : PenaltyStatus.FinansOnayi;
}