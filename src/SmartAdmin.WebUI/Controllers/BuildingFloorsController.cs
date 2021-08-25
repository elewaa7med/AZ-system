using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
	[Authorize]
	public class BuildingFloorsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public BuildingFloorsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TBuildingFloors.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			BuildingFloors buildingFloors = await _context.TBuildingFloors.SingleOrDefaultAsync((BuildingFloors m) => (int?)m.IdBuildingFloor == id);
			if (buildingFloors == null)
			{
				return NotFound();
			}
			return View(buildingFloors);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdBuildingFloor,PropertyFloorName"
		})] BuildingFloors buildingFloors)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(buildingFloors);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(buildingFloors);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			BuildingFloors buildingFloors = await _context.TBuildingFloors.SingleOrDefaultAsync((BuildingFloors m) => (int?)m.IdBuildingFloor == id);
			if (buildingFloors == null)
			{
				return NotFound();
			}
			return View(buildingFloors);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdBuildingFloor,PropertyFloorName"
		})] BuildingFloors buildingFloors)
		{
			if (id != buildingFloors.IdBuildingFloor)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(buildingFloors);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BuildingFloorsExists(buildingFloors.IdBuildingFloor))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(buildingFloors);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			BuildingFloors buildingFloors = await _context.TBuildingFloors.SingleOrDefaultAsync((BuildingFloors m) => (int?)m.IdBuildingFloor == id);
			if (buildingFloors == null)
			{
				return NotFound();
			}
			return View(buildingFloors);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			BuildingFloors buildingFloors = await _context.TBuildingFloors.SingleOrDefaultAsync((BuildingFloors m) => m.IdBuildingFloor == id);
			try
			{
				_context.TBuildingFloors.Remove(buildingFloors);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
			}
			return RedirectToAction("Index");
		}

		private bool BuildingFloorsExists(int id)
		{
			return _context.TBuildingFloors.Any((BuildingFloors e) => e.IdBuildingFloor == id);
		}
	}
}
