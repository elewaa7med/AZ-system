using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
	[Authorize]
	public class OwnersController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> _user;

		public OwnersController(ApplicationDbContext context, UserManager<ApplicationUser> user)
		{
			_context = context;
			_user = user;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TOwners.Include((Owners o) => o.mUserCreated).Include((Owners o) => o.mUserModified).ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Owners owners = await _context.TOwners.Include((Owners o) => o.mUserCreated).Include((Owners o) => o.mUserModified).SingleOrDefaultAsync((Owners m) => (int?)m.IdOwner == id);
			if (owners == null)
			{
				return NotFound();
			}
			return View(owners);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdOwner,OwnerName,OwnerMobile,OwnerPhone,Owneremail,OwnerAddress,CompanyName,ContactName,ContactPhone,OwnerNnotes,dtCreated,dtModified,IdCreatedBy,IdModifiedBy"
		})] Owners owners)
		{
			if (base.ModelState.IsValid)
			{
				owners.dtCreated = DateTime.Now;
				if (_user.GetUserName(base.HttpContext.User) != null)
				{
					owners.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
				}
				_context.Add(owners);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(owners);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Owners owners = await _context.TOwners.SingleOrDefaultAsync((Owners m) => (int?)m.IdOwner == id);
			if (owners == null)
			{
				return NotFound();
			}
			return View(owners);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdOwner,OwnerName,OwnerMobile,OwnerPhone,Owneremail,OwnerAddress,CompanyName,ContactName,ContactPhone,OwnerNnotes,dtCreated,dtModified,IdCreatedBy,IdModifiedBy"
		})] Owners owners)
		{
			if (id != owners.IdOwner)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					owners.dtModified = DateTime.Now;
					if (_user.GetUserName(base.HttpContext.User) != null)
					{
						owners.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
					}
					_context.Update(owners);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!OwnersExists(owners.IdOwner))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(owners);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			Owners owners = await _context.TOwners.Include((Owners o) => o.mUserCreated).Include((Owners o) => o.mUserModified).SingleOrDefaultAsync((Owners m) => (int?)m.IdOwner == id);
			if (owners == null)
			{
				return NotFound();
			}
			return View(owners);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			Owners owners = await _context.TOwners.SingleOrDefaultAsync((Owners m) => m.IdOwner == id);
			try
			{
				_context.TOwners.Remove(owners);
				await _context.SaveChangesAsync();
			}
			catch
			{
				base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
			}
			return RedirectToAction("Index");
		}

		private bool OwnersExists(int id)
		{
			return _context.TOwners.Any((Owners e) => e.IdOwner == id);
		}
	}
}
