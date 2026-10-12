using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Data;
using TrafikCezaYonetimApp.Models.Entities;
using TrafikCezaYonetimApp.Models.ViewModels;

namespace TrafikCezaYonetimApp.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(AppDbContext db, ILogger<VehicleController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _db.Vehicles
                .OrderBy(v => v.Plate)
                .ToListAsync();

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult Create() => View(new VehicleCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleCreateViewModel vmmodel)
        {
            if (!ModelState.IsValid)
                return View(vmmodel);

            var plate = vmmodel.Plate.Trim().ToUpperInvariant();

            if (await _db.Vehicles.AnyAsync(v => v.Plate == plate))
            {
                ModelState.AddModelError(nameof(vmmodel.Plate), "Bu plaka zaten kayıtlı.");
                return View(vmmodel);
            }

            _db.Vehicles.Add(new Vehicle
            {
                Plate = plate,
                VehicleType = vmmodel.VehicleType!.Value,
                Brand = vmmodel.Brand.Trim(),
                Model = vmmodel.Model.Trim()
            });

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Araç kaydedilemedi. Plaka: {Plate}", plate);
                ModelState.AddModelError("", "Araç kaydedilirken bir hata oluştu.");
                return View(vmmodel);
            }

            TempData["Success"] = "Araç eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            _db.Vehicles.Remove(vehicle);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Araç silinemedi. Id: {Id}", id);
                TempData["Error"] = "Araç silinemedi. Bu araca ait cezalar olabilir.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Araç silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            var model = new VehicleCreateViewModel
            {
                Plate = vehicle.Plate,
                VehicleType = vehicle.VehicleType,
                Brand = vehicle.Brand,
                Model = vehicle.Model
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleCreateViewModel vmmodel)
        {
            if (!ModelState.IsValid) return View(vmmodel);

            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            var plate = vmmodel.Plate.Trim().ToUpperInvariant();

            if (await _db.Vehicles.AnyAsync(v => v.Plate == plate && v.Id != id))
            {
                ModelState.AddModelError(nameof(vmmodel.Plate), "Bu plaka zaten kayıtlı.");
                return View(vmmodel);
            }

            vehicle.Plate = plate;
            vehicle.VehicleType = vmmodel.VehicleType!.Value;
            vehicle.Brand = vmmodel.Brand.Trim();
            vehicle.Model = vmmodel.Model.Trim();

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Araç güncellenemedi. Id: {Id}", id);
                ModelState.AddModelError("", "Araç güncellenirken bir hata oluştu.");
                return View(vmmodel);
            }

            TempData["Success"] = "Araç güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}