using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimApp.Data;
using TrafikCezaYonetimApp.Models.Entities;
using TrafikCezaYonetimApp.Models.ViewModels;

namespace TrafikCezaYonetimApp.Controllers
{
    public class VehicleController : Controller
    {
        private readonly AppDbContext _db;

        public VehicleController(AppDbContext db)
        {
            _db = db;
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

            await _db.SaveChangesAsync();

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
            await _db.SaveChangesAsync();

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

            // Başka bir araçta aynı plaka var mı? (kendisi hariç)
            if (await _db.Vehicles.AnyAsync(v => v.Plate == plate && v.Id != id))
            {
                ModelState.AddModelError(nameof(vmmodel.Plate), "Bu plaka zaten kayıtlı.");
                return View(vmmodel);
            }

            vehicle.Plate = plate;
            vehicle.VehicleType = vmmodel.VehicleType!.Value;
            vehicle.Brand = vmmodel.Brand;
            vehicle.Model = vmmodel.Model;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Araç güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
