using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
	[Authorize]
	public class DistrictsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DistrictsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TDistricts.Include((Districts d) => d.mCity).ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Districts districts = await _context.TDistricts.Include((Districts d) => d.mCity).SingleOrDefaultAsync((Districts m) => (int?)m.IdDistrict == id);
			if (districts == null)
			{
				return NotFound();
			}
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
			return View(districts);
		}

		public IActionResult Create()
		{
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdDistrict,DistrictName,IdCity"
		})] Districts districts)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(districts);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
			return View(districts);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Districts districts = await _context.TDistricts.SingleOrDefaultAsync((Districts m) => (int?)m.IdDistrict == id);
			if (districts == null)
			{
				return NotFound();
			}
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
			return View(districts);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdDistrict,DistrictName,IdCity"
		})] Districts districts)
		{
			if (id != districts.IdDistrict)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(districts);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DistrictsExists(districts.IdDistrict))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
			return View(districts);
		}

		public async Task<IActionResult> Delete(int? id)
		{

			if (!id.HasValue)
			{
				return NotFound();
			}
			Districts districts = await _context.TDistricts.Include((Districts d) => d.mCity).SingleOrDefaultAsync((Districts m) => (int?)m.IdDistrict == id);
			base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
			if (districts == null)
			{
				return NotFound();
			}
			return View(districts);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Districts districts = await _context.TDistricts.SingleOrDefaultAsync((Districts m) => m.IdDistrict == id);
			try
			{
				_context.TDistricts.Remove(districts);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["IdCity"] = new SelectList(_context.TCities, "IdCity", "CityName", districts.IdCity);
				base.ViewData["AlertSaveErr"] = "Districts is added to Other Table You Have to Remove them From These tables first";
				return View();
			}
			return RedirectToAction("Index");
		}

		private bool DistrictsExists(int id)
		{
			return _context.TDistricts.Any((Districts e) => e.IdDistrict == id);
		}
	}
}
