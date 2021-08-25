using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.BldMetersViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
	[Authorize]
	public class BldMeterWaterController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> _user;

		public BldMeterWaterController(ApplicationDbContext context, UserManager<ApplicationUser> user)
		{
			_context = context;
			_user = user;
		}

		public async Task<IActionResult> Index()
		{
			return View(await (from m in _context.TBuildings
							   select new BldWaterMeterVM
							   {
								   IdBuilding = m.IdBuilding,
								   MeterWaterNumber = m.MeterWaterNumber,
								   BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
								   districtName = m.mDistrict.DistrictName
							   } into m
							   where !string.IsNullOrWhiteSpace(m.MeterWaterNumber)
							   orderby m.districtName, m.BuildingInfo
							   select m).ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			MeterWaterInfo meterWaterInfo = await _context.TMeterWaterInfo.Include((MeterWaterInfo m) => m.mBuilding).SingleOrDefaultAsync((MeterWaterInfo m) => (int?)m.IdMeter == id);
			if (meterWaterInfo == null)
			{
				return NotFound();
			}
			return View(meterWaterInfo);
		}

		public IActionResult Create(int? IdBuilding = 0)
		{
			base.ViewData["IdBuilding"] = new SelectList(from m in _context.TBuildings
														 select new
														 {
															 IdBuilding = m.IdBuilding,
															 BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
															 districtName = m.mDistrict.DistrictName
														 } into m
														 orderby m.districtName, m.BuildingInfo
														 select m, "IdBuilding", "BuildingInfo", IdBuilding);
			if (IdBuilding != 0)
			{
				base.ViewData["MeterWaterNumber"] = (from m in _context.TBuildings
													 where (int?)m.IdBuilding == IdBuilding
													 select m.MeterWaterNumber).First().ToString();
			}
			else
			{
				base.ViewData["MeterWaterNumber"] = "";
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(MeterWaterInfo meterWaterInfo)
		{
			if (base.ModelState.IsValid)
			{
				Buildings bld = _context.TBuildings.Where((Buildings m) => m.IdBuilding == meterWaterInfo.IdBuilding).First();
				_context.Update(bld);
				meterWaterInfo.MeterNumber = base.HttpContext.Request.Form["MeterNumber"].ToString();
				bld.MeterWaterNumber = meterWaterInfo.MeterNumber;
				await _context.SaveChangesAsync();
				base.ViewData["AlertSaveOK"] = "The Record has been updated and saved successfully.";
			}
			base.ViewData["IdBuilding"] = new SelectList(from m in _context.TBuildings
														 select new
														 {
															 IdBuilding = m.IdBuilding,
															 BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
															 DistrictName = m.mDistrict.DistrictName
														 } into m
														 orderby m.DistrictName, m.DistrictName
														 select m, "IdBuilding", "BuildingInfo", meterWaterInfo.IdBuilding);
			base.ViewData["MeterWaterNumber"] = meterWaterInfo.MeterNumber;
			return View();
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			MeterWaterInfo meterWaterInfo = await _context.TMeterWaterInfo.SingleOrDefaultAsync((MeterWaterInfo m) => (int?)m.IdMeter == id);
			if (meterWaterInfo == null)
			{
				return NotFound();
			}
			base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingAddress", meterWaterInfo.IdBuilding);
			return View(meterWaterInfo);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, MeterWaterInfo meterWaterInfo)
		{
			if (id != meterWaterInfo.IdMeter)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(meterWaterInfo);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!MeterWaterInfoExists(meterWaterInfo.IdMeter))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingAddress", meterWaterInfo.IdBuilding);
			return View(meterWaterInfo);
		}

		public async Task<IActionResult> Delete(int? IdBuilding)
		{
			if (!IdBuilding.HasValue)
			{
				return NotFound();
			}
			Buildings bld = await _context.TBuildings.SingleOrDefaultAsync((Buildings m) => (int?)m.IdBuilding == IdBuilding);
			if (bld == null)
			{
				return NotFound();
			}
			bld.MeterWaterNumber = "";
			_context.Update(bld);
			await _context.SaveChangesAsync();
			return View("Index", await (from m in _context.TBuildings
										select new BldWaterMeterVM
										{
											IdBuilding = m.IdBuilding,
											MeterWaterNumber = m.MeterWaterNumber,
											BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
											districtName = m.mDistrict.DistrictName
										} into m
										where !string.IsNullOrWhiteSpace(m.MeterWaterNumber)
										orderby m.districtName, m.BuildingInfo
										select m).ToListAsync());
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			MeterWaterInfo meterWaterInfo = await _context.TMeterWaterInfo.SingleOrDefaultAsync((MeterWaterInfo m) => m.IdMeter == id);
			_context.TMeterWaterInfo.Remove(meterWaterInfo);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool MeterWaterInfoExists(int id)
		{
			return _context.TMeterWaterInfo.Any((MeterWaterInfo e) => e.IdMeter == id);
		}

		public JsonResult getWaterMeterInfo(int IdBuilding)
		{
			var buildingWaterMeter = (from m in _context.TBuildings
									  where m.IdBuilding == IdBuilding
									  select new
									  {
										  meterWaterNumber = m.MeterWaterNumber
									  }).FirstOrDefault();
			return Json(buildingWaterMeter);
		}
	}
}
