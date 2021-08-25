using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TCities.Include((Cities d) => d.mBuildingCountry).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Cities cities = await _context.TCities.Include((Cities d) => d.mBuildingCountry).SingleOrDefaultAsync((Cities m) => (int?)m.IdCity == id);
            if (cities == null)
            {
                return NotFound();
            }
            return View(cities);
        }

        public IActionResult Create()
        {
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(new string[]
        {
            "IdCity,CityName,IdCountry"
        })] Cities cities)
        {
            if (base.ModelState.IsValid)
            {
                _context.Add(cities);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName");
            return View(cities);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Cities cities = await _context.TCities.SingleOrDefaultAsync((Cities m) => (int?)m.IdCity == id);
            if (cities == null)
            {
                return NotFound();
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName");
            return View(cities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(new string[]
        {
            "IdCity,CityName,IdCountry"
        })] Cities cities)
        {
            if (id != cities.IdCity)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                try
                {
                    _context.Update(cities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitiesExists(cities.IdCity))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName");
            return View(cities);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Cities cities = await _context.TCities.Include((Cities d) => d.mBuildingCountry).SingleOrDefaultAsync((Cities m) => (int?)m.IdCity == id);
            if (cities == null)
            {
                return NotFound();
            }
            return View(cities);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Cities cities = await _context.TCities.SingleOrDefaultAsync((Cities m) => m.IdCity == id);
            try
            {
                _context.TCities.Remove(cities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                base.ViewData["AlertSaveErr"] = "District Added to This City So You Have To Delete District First.";
                return View();
            }
            return RedirectToAction("Index");
        }

        private bool CitiesExists(int id)
        {
            return _context.TCities.Any((Cities e) => e.IdCity == id);
        }
    }
}
