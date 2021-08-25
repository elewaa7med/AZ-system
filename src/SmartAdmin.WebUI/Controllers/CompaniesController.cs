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
	public class CompaniesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CompaniesController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TCompanies.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Companies companies = await _context.TCompanies.SingleOrDefaultAsync((Companies m) => (int?)m.IdCompany == id);
			if (companies == null)
			{
				return NotFound();
			}
			return View(companies);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdCompany,companyName,companyAddress,compayPhone"
		})] Companies companies)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(companies);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(companies);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Companies companies = await _context.TCompanies.SingleOrDefaultAsync((Companies m) => (int?)m.IdCompany == id);
			if (companies == null)
			{
				return NotFound();
			}
			return View(companies);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdCompany,companyName,companyAddress,compayPhone"
		})] Companies companies)
		{
			if (id != companies.IdCompany)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(companies);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CompaniesExists(companies.IdCompany))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(companies);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Companies companies = await _context.TCompanies.SingleOrDefaultAsync((Companies m) => (int?)m.IdCompany == id);
			if (companies == null)
			{
				return NotFound();
			}
			return View(companies);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Companies companies = await _context.TCompanies.SingleOrDefaultAsync((Companies m) => m.IdCompany == id);
			try
			{
				_context.TCompanies.Remove(companies);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
			}
			return RedirectToAction("Index");
		}

		private bool CompaniesExists(int id)
		{
			return _context.TCompanies.Any((Companies e) => e.IdCompany == id);
		}
	}
}
