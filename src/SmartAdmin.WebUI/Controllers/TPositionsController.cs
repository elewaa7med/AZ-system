using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AZBMS.Controllers
{
	[Authorize]
	public class TPositionsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TPositionsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TPosition.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			TPositions tPositions = await _context.TPosition.SingleOrDefaultAsync((TPositions m) => (int?)m.IdPosition == id);
			if (tPositions == null)
			{
				return NotFound();
			}
			return View(tPositions);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdPosition,positionName_a,positionName_E"
		})] TPositions tPositions)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(tPositions);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(tPositions);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			TPositions tPositions = await _context.TPosition.SingleOrDefaultAsync((TPositions m) => (int?)m.IdPosition == id);
			if (tPositions == null)
			{
				return NotFound();
			}
			return View(tPositions);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdPosition,positionName_a,positionName_E"
		})] TPositions tPositions)
		{
			if (id != tPositions.IdPosition)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(tPositions);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TPositionsExists(tPositions.IdPosition))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(tPositions);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			TPositions tPositions = await _context.TPosition.SingleOrDefaultAsync((TPositions m) => (int?)m.IdPosition == id);
			if (tPositions == null)
			{
				return NotFound();
			}
			return View(tPositions);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			TPositions tPositions = await _context.TPosition.SingleOrDefaultAsync((TPositions m) => m.IdPosition == id);
			try
			{
				_context.TPosition.Remove(tPositions);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
			}
			return RedirectToAction("Index");
		}

		private bool TPositionsExists(int id)
		{
			return _context.TPosition.Any((TPositions e) => e.IdPosition == id);
		}
	}
}
