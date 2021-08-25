using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.UnitElectricityMeterVM;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
	[Authorize]
	public class ElectricityMeterUnitController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> _user;

		public ElectricityMeterUnitController(ApplicationDbContext context, UserManager<ApplicationUser> user)
		{
			_context = context;
			_user = user;
		}

		public async Task<IActionResult> Index(int? IdBuilding = 0)
		{
			var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
								  select new
								  {
									  IdBuilding = m.IdBuilding,
									  BuildingName = m.BuildingName,
									  BuildingAddress = m.BuildingAddress,
									  DistrictName = m.mDistrict.DistrictName,
									  buildingInfo = string.Concat(m.BuildingName ," - ", m.mDistrict.DistrictName)
									  //buildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName)
								  } into m
								  orderby m.DistrictName, m.BuildingName
								  select m).ToListAsync();
			base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
			return View(await (from m in _context.Units.Include((Units m) => m.mBuilding)
							   select new UnitElectricityMeterVM
							   {
								   IdUnit = m.IdUnit,
								   IdBuilding = m.IdBuilding,
								   ElectricityMeterNumber = m.ElectricityNo,
								   UnitNumber = m.UnitNumber,
								   BuildingInfo = string.Concat(m.mBuilding.BuildingName ," - ", m.mBuilding.mDistrict.DistrictName),
								   //BuildingInfo = string.Concat(m.mBuilding.BuildingName + 45, m.mBuilding.mDistrict.DistrictName),
								   districtName = m.mBuilding.mDistrict.DistrictName
							   } into m
							   where !string.IsNullOrWhiteSpace(m.ElectricityMeterNumber) && ((IdBuilding > (int?)0) ? ((int?)m.IdBuilding == IdBuilding) : true)
							   orderby m.districtName, m.BuildingInfo, m.UnitNumber
							   select m).ToListAsync());
		}

		public IActionResult Create(int? IdUnit = 0)
		{
			int IdBuilding = 0;
			if (IdUnit > 0)
			{
				IdBuilding = int.Parse((from m in _context.Units
										where (int?)m.IdUnit == IdUnit
										select m.IdBuilding).First().ToString());
			}
			base.ViewData["IdBuilding"] = new SelectList(from m in _context.TBuildings
														 select new
														 {
															 IdBuilding = m.IdBuilding,
															 BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
															 districtName = m.mDistrict.DistrictName
														 } into m
														 orderby m.districtName, m.BuildingInfo
														 select m, "IdBuilding", "BuildingInfo", IdBuilding);
			if (IdUnit > 0)
			{
				base.ViewData["IdUnit"] = new SelectList(from m in _context.Units
														 select new
														 {
															 m.IdUnit,
															 m.UnitNumber,
															 m.IdBuilding
														 } into m
														 where m.IdBuilding == IdBuilding
														 orderby m.UnitNumber
														 select m, "IdUnit", "UnitNumber", IdUnit);
			}
			if (IdUnit != 0)
			{
				base.ViewData["MeterNumber"] = (from m in _context.Units
												where (int?)m.IdUnit == IdUnit
												select m.ElectricityNo).First().ToString();
			}
			else
			{
				base.ViewData["MeterNumber"] = "";
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(MetersElectricityInfo meterInfo)
		{
			int IdBuilding = 0;
			int idunit = 0;
			int.TryParse(base.HttpContext.Request.Form["IdUnit"].ToString(), out idunit);
			meterInfo.IdUnit = idunit;
			if (idunit > 0)
			{
				int.TryParse((from m in _context.Units
							  select new
							  {
								  m.IdBuilding,
								  m.IdUnit
							  } into m
							  where m.IdUnit == meterInfo.IdUnit
							  select m).First().IdBuilding.ToString(), out IdBuilding);
			}
			if (base.ModelState.IsValid)
			{
				Units unit = _context.Units.Where((Units m) => m.IdUnit == meterInfo.IdUnit).First();
				_context.Update(unit);
				unit.ElectricityNo = meterInfo.MeterNumber;
				await _context.SaveChangesAsync();
				base.ViewData["AlertSaveOK"] = "The Record has been Updated and Saved Successfully";
			}
			base.ViewData["IdBuilding"] = new SelectList(from m in _context.TBuildings
														 select new
														 {
															 IdBuilding = m.IdBuilding,
															 BuildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName),
															 DistrictName = m.mDistrict.DistrictName
														 } into m
														 orderby m.DistrictName, m.DistrictName
														 select m, "IdBuilding", "BuildingInfo", IdBuilding);
			base.ViewData["IdUnit"] = new SelectList(from m in _context.Units
													 select new
													 {
														 m.IdUnit,
														 m.UnitNumber,
														 m.IdBuilding
													 } into m
													 where m.IdBuilding == IdBuilding
													 orderby m.UnitNumber
													 select m, "IdUnit", "UnitNumber", idunit);
			base.ViewData["MeterNumber"] = meterInfo.MeterNumber;
			return View();
		}

		public async Task<IActionResult> Delete(int? IdUnit)
		{
			if (!IdUnit.HasValue)
			{
				return NotFound();
			}
			Units mUnit = await _context.Units.SingleOrDefaultAsync((Units m) => (int?)m.IdUnit == IdUnit);
			if (mUnit == null)
			{
				return NotFound();
			}
			int IdBuilding = mUnit.IdBuilding;
			mUnit.ElectricityNo = "";
			_context.Update(mUnit);
			await _context.SaveChangesAsync();
			IOrderedQueryable<UnitElectricityMeterVM> applicationDbContext = from m in _context.Units.Include((Units m) => m.mBuilding)
																			 select new UnitElectricityMeterVM
																			 {
																				 IdUnit = m.IdUnit,
																				 IdBuilding = m.IdBuilding,
																				 ElectricityMeterNumber = m.ElectricityNo,
																				 UnitNumber = m.UnitNumber,
																				 BuildingInfo = string.Concat(string.Concat(string.Concat(m.mBuilding.BuildingName + 45, m.mBuilding.BuildingAddress), 45), m.mBuilding.mDistrict.DistrictName),
																				 districtName = m.mBuilding.mDistrict.DistrictName
																			 } into m
																			 where !string.IsNullOrWhiteSpace(m.ElectricityMeterNumber) && ((IdBuilding > 0) ? (m.IdBuilding == IdBuilding) : true)
																			 orderby m.districtName, m.BuildingInfo, m.UnitNumber
																			 select m;
			var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
								  select new
								  {
									  IdBuilding = m.IdBuilding,
									  BuildingName = m.BuildingName,
									  BuildingAddress = m.BuildingAddress,
									  DistrictName = m.mDistrict.DistrictName,
									  buildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName)
								  } into m
								  orderby m.DistrictName, m.BuildingName
								  select m).ToListAsync();
			base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
			var result = await applicationDbContext.ToListAsync();
			return RedirectToAction("Index");
		}

		private bool MeterWaterInfoExists(int id)
		{
			return _context.TMeterWaterInfo.Any((MeterWaterInfo e) => e.IdMeter == id);
		}

		public async void fillBuildingsforDDLFilter(int? idBuilding = 0)
		{
			var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
								  select new
								  {
									  IdBuilding = m.IdBuilding,
									  BuildingName = m.BuildingName,
									  BuildingAddress = m.BuildingAddress,
									  DistrictName = m.mDistrict.DistrictName,
									  buildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName)
								  } into m
								  orderby m.DistrictName, m.BuildingName
								  select m).ToListAsync();
			base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", idBuilding);
		}

		public async Task<JsonResult> getBuildingsforDDLFilter()
		{
			return Json(new SelectList(await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
											  select new
											  {
												  IdBuilding = m.IdBuilding,
												  BuildingName = m.BuildingName,
												  BuildingAddress = m.BuildingAddress,
												  DistrictName = m.mDistrict.DistrictName,
												  buildingInfo = string.Concat(m.BuildingName + 45, m.mDistrict.DistrictName)
											  } into m
											  orderby m.DistrictName, m.BuildingName
											  select m).ToListAsync(), "IdBuilding", "buildingInfo"));
		}

		public JsonResult getElectricityMeterInfo(int IdUnit)
		{
			var unitElectricityMeter = (from m in _context.Units
										select new
										{
											electricityMeter = m.ElectricityNo,
											IdUnit = m.IdUnit
										} into m
										where m.IdUnit == IdUnit
										select m).FirstOrDefault();
			return Json(unitElectricityMeter);
		}

		public async Task<JsonResult> getBuildingUnits(int IdBuilding)
		{
			return Json(new SelectList(await (from m in _context.Units
											  select new
											  {
												  m.IdUnit,
												  m.UnitNumber,
												  m.IdBuilding
											  } into m
											  where m.IdBuilding == IdBuilding
											  orderby m.UnitNumber
											  select m).ToListAsync(), "IdUnit", "UnitNumber"));
		}
	}
}
