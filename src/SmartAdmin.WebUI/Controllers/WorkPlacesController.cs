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
	public class WorkPlacesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public WorkPlacesController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TWorkplaces.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			WorkPlaces workPlaces = await _context.TWorkplaces.SingleOrDefaultAsync((WorkPlaces m) => (int?)m.IdWorkPlace == id);
			if (workPlaces == null)
			{
				return NotFound();
			}
			return View(workPlaces);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdWorkPlace,workPlace,notes"
		})] WorkPlaces workPlaces)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(workPlaces);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(workPlaces);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			WorkPlaces workPlaces = await _context.TWorkplaces.SingleOrDefaultAsync((WorkPlaces m) => (int?)m.IdWorkPlace == id);
			if (workPlaces == null)
			{
				return NotFound();
			}
			return View(workPlaces);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdWorkPlace,workPlace,notes"
		})] WorkPlaces workPlaces)
		{
			if (id != workPlaces.IdWorkPlace)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(workPlaces);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!WorkPlacesExists(workPlaces.IdWorkPlace))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(workPlaces);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			WorkPlaces workPlaces = await _context.TWorkplaces.SingleOrDefaultAsync((WorkPlaces m) => (int?)m.IdWorkPlace == id);
			if (workPlaces == null)
			{
				return NotFound();
			}
			return View(workPlaces);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			WorkPlaces workPlaces = await _context.TWorkplaces.SingleOrDefaultAsync((WorkPlaces m) => m.IdWorkPlace == id);
			_context.TWorkplaces.Remove(workPlaces);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool WorkPlacesExists(int id)
		{
			return _context.TWorkplaces.Any((WorkPlaces e) => e.IdWorkPlace == id);
		}
	}
}
