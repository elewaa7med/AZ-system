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
	public class MandoobsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public MandoobsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TMandoobs.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Mandoobs mandoobs = await _context.TMandoobs.SingleOrDefaultAsync((Mandoobs m) => (int?)m.IdMandoob == id);
			if (mandoobs == null)
			{
				return NotFound();
			}
			return View(mandoobs);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdMandoob,mandoobName,,mandoobMobile,mandoobNotes"
		})] Mandoobs mandoobs)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(mandoobs);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(mandoobs);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Mandoobs mandoobs = await _context.TMandoobs.SingleOrDefaultAsync((Mandoobs m) => (int?)m.IdMandoob == id);
			if (mandoobs == null)
			{
				return NotFound();
			}
			return View(mandoobs);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdMandoob,mandoobName,mandoobMobile,mandoobNotes"
		})] Mandoobs mandoobs)
		{
			if (id != mandoobs.IdMandoob)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(mandoobs);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!MandoobsExists(mandoobs.IdMandoob))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(mandoobs);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Mandoobs mandoobs = await _context.TMandoobs.SingleOrDefaultAsync((Mandoobs m) => (int?)m.IdMandoob == id);
			if (mandoobs == null)
			{
				return NotFound();
			}
			return View(mandoobs);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Mandoobs mandoobs = await _context.TMandoobs.SingleOrDefaultAsync((Mandoobs m) => m.IdMandoob == id);
			try
			{
				_context.TMandoobs.Remove(mandoobs);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
			}
			return RedirectToAction("Index");
		}

		private bool MandoobsExists(int id)
		{
			return _context.TMandoobs.Any((Mandoobs e) => e.IdMandoob == id);
		}
	}
}
