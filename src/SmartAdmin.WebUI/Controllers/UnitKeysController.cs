using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace AZBMS.Controllers
{
	[Authorize]
	public class UnitKeysController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> _user;

		public UnitKeysController(ApplicationDbContext context, UserManager<ApplicationUser> user)
		{
			_context = context;
			_user = user;
		}

		public async Task<IActionResult> Index(int id)
		{
			ViewBag.id = id;
			return View(await _context.Units.Where(x=>x.idMasterBuilding ==id && x.UnitRentContract.IdCompound == null).Include(x=>x.UnitRentContract).Include((Units m) => m.mBuilding).Include((Units u) => u.mMandoob).GroupJoin(_context.TUnitKeys, (Units e) => e.IdUnitKey, (UnitKeys d) => d.IdUnitKeys, (Units unt, IEnumerable<UnitKeys> untkeys) => new
			{
				unt,
				untkeys
			}).SelectMany(z => z.untkeys.DefaultIfEmpty(), (p, b) => p.unt)
				.ToListAsync());
		}

		public async Task<IActionResult> Details(int pageid,int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			UnitKeys unitKeys = await _context.TUnitKeys.Include((UnitKeys u) => u.mMandoob).Include((UnitKeys u) => u.mUnit).ThenInclude((Units u) => u.mBuilding)
				.ThenInclude((Buildings u) => u.mDistrict)
				.ThenInclude((Districts u) => u.mCity)
				.ThenInclude((Cities u) => u.mBuildingCountry)
				.SingleOrDefaultAsync((UnitKeys m) => (int?)m.IdUnitKeys == id);
			if (unitKeys == null)
			{
				return NotFound();
			}
			ViewBag.id = pageid;
			return View(unitKeys);
		}

		public async Task<IActionResult> Create(int pageid)
		{
			Countries counter = _context.TCountries.FirstOrDefault();
			Cities city = _context.TCities.Where(x => x.IdCountry == counter.IdCountry).FirstOrDefault();
			Districts districts = _context.TDistricts.Where(x => x.IdCity == city.IdCity).FirstOrDefault();
			Buildings building = _context.TBuildings.Where(x => x.IdDistrict == districts.IdDistrict).FirstOrDefault();
			Units unit = _context.Units.Where(x => x.IdBuilding == building.IdBuilding).FirstOrDefault();
			base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", counter);
			base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities p) => p.IdCountry == counter.IdCountry), "IdCity", "CityName", city);
			base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts p) => p.IdCity == city.IdCity), "IdDistrict", "DistrictName", districts);
			base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where((Buildings p) => p.IdDistrict == districts.IdDistrict), "IdBuilding", "BuildingName", building);
			base.ViewData["IdUnit"] = new SelectList(_context.Units.Where(x=>x.IdBuilding == building.IdBuilding), "IdUnit", "UnitNumber", unit);
			base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs, "IdMandoob", "mandoobName");
			ViewBag.id = pageid;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[]
		{
			"IdUnitKeys,IdUnit,IdMandoob,dtTaken,dtBack,isTheKeyAvailable"
		})] UnitKeys unitKeys)
		{
			Units thisUnit = await _context.Units.Include((Units t) => t.mBuilding).ThenInclude((Buildings t) => t.mDistrict).ThenInclude((Districts t) => t.mCity)
				.ThenInclude((Cities t) => t.mBuildingCountry)
				.SingleOrDefaultAsync((Units t) => t.IdUnit == unitKeys.IdUnit);
			_context.Update(thisUnit);
			string frmisTheKeyAvailable = base.HttpContext.Request.Form["isTheKeyAvailable"].ToString();
			string frmdtTaken = base.HttpContext.Request.Form["dtTaken"].ToString();
			string frmdtBack = base.HttpContext.Request.Form["dtBack"].ToString();
			UnitKeys thisUnitKeys = unitKeys;
			unitKeys.isTheKeyAvailable = bool.Parse(frmisTheKeyAvailable);
			try
			{
				unitKeys.dtTaken = DateTime.Parse(frmdtTaken);
				string frmIdMandoob = base.HttpContext.Request.Form["IdMandoob"].ToString();
				unitKeys.IdMandoob = int.Parse(frmIdMandoob);
				unitKeys.dtBack = DateTime.Parse(frmdtBack);
			}
			catch
			{
			}
			if (!unitKeys.isTheKeyAvailable)
			{
				thisUnitKeys = _context.TUnitKeys.Where((UnitKeys t) => (int?)t.IdUnitKeys == thisUnit.IdUnitKey).FirstOrDefault();
				thisUnitKeys.dtBack = DateTime.Parse(frmdtBack);
			}
			int idbuilding = thisUnit.IdBuilding;
			int iddistrict = thisUnit.mBuilding.IdDistrict;
			Districts dst = thisUnit.mBuilding.mDistrict;
			int idcity = dst.IdCity;
			Cities cty = dst.mCity;
			int idcountry = cty.IdCountry;
			if (base.ModelState.IsValid)
			{
				if (unitKeys.isTheKeyAvailable)
				{
					if (_context.TUnitKeys.Where((UnitKeys dttk) => (DateTime)dttk.dtTaken <= (DateTime)unitKeys.dtTaken && (DateTime)dttk.dtBack > (DateTime)unitKeys.dtTaken && dttk.IdUnit == unitKeys.IdUnit).FirstOrDefault() != null)
					{
						base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", idcountry);
						base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities p) => p.IdCountry == idcountry), "IdCity", "CityName", idcity);
						base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts p) => p.IdCity == idcity), "IdDistrict", "DistrictName", iddistrict);
						base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where((Buildings p) => p.IdDistrict == iddistrict), "IdBuilding", "BuildingName", idbuilding);
						base.ViewData["IdUnit"] = new SelectList(_context.Units, "IdUnit", "UnitNumber", thisUnitKeys.IdUnit);
						base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs, "IdMandoob", "mandoobName", thisUnitKeys.IdMandoob);
						return View(thisUnitKeys);
					}
				}
				else if (_context.TUnitKeys.Where((UnitKeys dtbk) => (DateTime)dtbk.dtTaken <= (DateTime)unitKeys.dtBack && (DateTime)(dtbk.dtBack ?? dtbk.dtTaken) > (DateTime)unitKeys.dtBack && dtbk.IdUnit == unitKeys.IdUnit).FirstOrDefault() != null || unitKeys.dtBack < unitKeys.dtTaken)
				{
					base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", idcountry);
					base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities p) => p.IdCountry == idcountry), "IdCity", "CityName", idcity);
					base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts p) => p.IdCity == idcity), "IdDistrict", "DistrictName", iddistrict);
					base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where((Buildings p) => p.IdDistrict == iddistrict), "IdBuilding", "BuildingName", idbuilding);
					base.ViewData["IdUnit"] = new SelectList(_context.Units, "IdUnit", "UnitNumber", thisUnitKeys.IdUnit);
					base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs.Where((Mandoobs u) => u.IdMandoob == thisUnitKeys.IdMandoob), "IdMandoob", "mandoobName", thisUnitKeys.IdMandoob);
					return View(thisUnitKeys);
				}
				if (unitKeys.isTheKeyAvailable)
				{
					unitKeys.isTheKeyAvailable = !unitKeys.isTheKeyAvailable;
				}
				else
				{
					thisUnitKeys.isTheKeyAvailable = !thisUnitKeys.isTheKeyAvailable;
					unitKeys.isTheKeyAvailable = !unitKeys.isTheKeyAvailable;
				}
				thisUnit.isTheKeyAvailable = unitKeys.isTheKeyAvailable;
				thisUnit.IdMandoob = unitKeys.IdMandoob;
				thisUnit.dtBack = unitKeys.dtBack;
				thisUnit.dtTaken = unitKeys.dtTaken;
				if (!thisUnitKeys.isTheKeyAvailable)
				{
					_context.Add(thisUnitKeys);
					int lastid = 0;
					using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
					{
						command.CommandText = "select ISNULL(IDENT_CURRENT('tunitkeys' ),0)";
						_context.Database.OpenConnection();
						using (DbDataReader result = command.ExecuteReader())
						{
							if (result.HasRows)
							{
								result.Read();
								lastid = int.Parse(result.GetValue(0).ToString());
							}
						}
					}
					thisUnit.IdUnitKey = lastid + 1;
				}
				else
				{
					_context.Update(thisUnitKeys);
				}
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", idcountry);
			base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities p) => p.IdCountry == idcountry), "IdCity", "CityName", idcity);
			base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts p) => p.IdCity == idcity), "IdDistrict", "DistrictName", iddistrict);
			base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where((Buildings p) => p.IdDistrict == iddistrict), "IdBuilding", "BuildingName", idbuilding);
			if (unitKeys.IdUnitKeys != thisUnitKeys.IdUnitKeys)
			{
				base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs.Where((Mandoobs u) => u.IdMandoob == thisUnitKeys.IdMandoob), "IdMandoob", "mandoobName", thisUnitKeys.IdMandoob);
			}
			else
			{
				base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs, "IdMandoob", "mandoobName", thisUnitKeys.IdMandoob);
			}
			base.ViewData["IdUnit"] = new SelectList(_context.Units, "IdUnit", "UnitNumber", thisUnitKeys.IdUnit);
			return View(thisUnitKeys);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			UnitKeys unitKeys = await _context.TUnitKeys.Include((UnitKeys m) => m.mUnit).ThenInclude((Units m) => m.mBuilding).ThenInclude((Buildings m) => m.mDistrict)
				.ThenInclude((Districts m) => m.mCity)
				.ThenInclude((Cities m) => m.mBuildingCountry)
				.SingleOrDefaultAsync((UnitKeys m) => (int?)m.IdUnitKeys == id);
			if (unitKeys == null)
			{
				return NotFound();
			}
			base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", unitKeys.mUnit.mBuilding.mDistrict.mCity.IdCountry);
			base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities p) => p.IdCountry == unitKeys.mUnit.mBuilding.mDistrict.mCity.IdCountry), "IdCity", "CityName", unitKeys.mUnit.mBuilding.mDistrict.IdCity);
			base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts p) => p.IdCity == unitKeys.mUnit.mBuilding.mDistrict.IdCity), "IdDistrict", "DistrictName", unitKeys.mUnit.mBuilding.IdDistrict);
			base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where((Buildings p) => p.IdDistrict == unitKeys.mUnit.mBuilding.IdDistrict), "IdBuilding", "BuildingName", unitKeys.mUnit.IdBuilding);
			base.ViewData["IdUnit"] = new SelectList(_context.Units, "IdUnit", "UnitNumber", unitKeys.IdUnit);
			base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs, "IdMandoob", "mandoobName", unitKeys.IdMandoob);
			ViewBag.id = unitKeys.mUnit.mBuilding.idMasterBuilding;
			return View(unitKeys);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[]
		{
			"IdUnitKeys,IdUnit,IdMandoob,dtTaken,dtBack,isTheKeyAvailable"
		})] UnitKeys unitKeys,int pageid)
		{
			if (id != unitKeys.IdUnitKeys)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(unitKeys);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!UnitKeysExists(unitKeys.IdUnitKeys))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index",new { id = pageid });
			}
			base.ViewData["IdMandoob"] = new SelectList(_context.TMandoobs, "IdMandoob", "mandoobName", unitKeys.IdMandoob);
			base.ViewData["IdUnit"] = new SelectList(_context.Units, "IdUnit", "UnitNumber", unitKeys.IdUnit);
			return View(unitKeys);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			UnitKeys unitKeys = await _context.TUnitKeys.Include((UnitKeys u) => u.mMandoob).Include((UnitKeys u) => u.mUnit).ThenInclude((Units u) => u.mBuilding)
				.ThenInclude((Buildings u) => u.mDistrict)
				.ThenInclude((Districts u) => u.mCity)
				.ThenInclude((Cities u) => u.mBuildingCountry)
				.SingleOrDefaultAsync((UnitKeys m) => (int?)m.IdUnitKeys == id);
			if (unitKeys == null)
			{
				return NotFound();
			}
			ViewBag.id = unitKeys.mUnit.mBuilding.idMasterBuilding;
			return View(unitKeys);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int pageid, int id)
		{
			UnitKeys unitKeys = await _context.TUnitKeys.SingleOrDefaultAsync((UnitKeys m) => m.IdUnitKeys == id);
			Units unit = await _context.Units.SingleOrDefaultAsync((Units t) => t.IdUnitKey == (int?)id);
			if (unit != null)
			{
				unit.IdUnitKey = null;
				unit.dtTaken = null;
				unit.dtBack = null;
				unit.isTheKeyAvailable = true;
				unit.IdMandoob = null;
			}
			_context.TUnitKeys.Remove(unitKeys);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index",new { id = pageid });
		}

		private bool UnitKeysExists(int id)
		{
			return _context.TUnitKeys.Any((UnitKeys e) => e.IdUnitKeys == id);
		}

		public JsonResult getCountries()
		{
			List<Countries> countries2 = new List<Countries>();
			countries2 = _context.TCountries.ToList();
			base.ViewData["IdCountry"] = new SelectList(countries2, "IdCountry", "CountryName");
			return Json(new SelectList(countries2, "IdCountry", "CountryName"));
		}

		public JsonResult getCitiesByCountries(int IdCountry)
		{
			List<Cities> Cities2 = new List<Cities>();
			Cities2 = _context.TCities.Where((Cities u) => u.IdCountry == IdCountry).ToList();
			base.ViewData["IdCity"] = new SelectList(Cities2, "IdCity", "CityName");
			return Json(new SelectList(Cities2, "IdCity", "CityName"));
		}

		public JsonResult getDistrictsByCities(int IdCity)
		{
			List<Districts> Districts2 = new List<Districts>();
			Districts2 = _context.TDistricts.Where((Districts u) => u.IdCity == IdCity).ToList();
			base.ViewData["IdDistrict"] = new SelectList(Districts2, "IdDistrict", "DistrictName");
			return Json(new SelectList(Districts2, "IdDistrict", "DistrictName"));
		}

		public JsonResult getBuildingsByDistricts(int IdDistrict)
		{
			List<Buildings> buildings2 = new List<Buildings>();
			buildings2 = _context.TBuildings.Where((Buildings u) => u.IdDistrict == IdDistrict).ToList();
			base.ViewData["IdBuilding"] = new SelectList(buildings2, "IdBuilding", "BuildingName");
			Json(new SelectList(buildings2, "IdBuilding", "BuildingName"));
			return Json(new SelectList(buildings2, "IdBuilding", "BuildingName"));
		}

		public JsonResult getUnitsByBuildings(int IdBuilding)
		{
			List<Units> units2 = new List<Units>();
			units2 = _context.Units.Where((Units u) => u.IdBuilding == IdBuilding).ToList();
			base.ViewData["IdUnit"] = new SelectList(units2, "IdUnit", "UnitNumber");
			return Json(new SelectList(units2, "IdUnit", "UnitNumber"));
		}

		public JsonResult getUnitKeyInfo(int IdUnit)
		{
			var unitKeyInfo = (from g in _context.Units.Where((Units u) => u.IdUnit == IdUnit).Include((Units p) => p.mMandoob)
							   select new
							   {
								   g.IdMandoob,
								   g.dtTaken,
								   g.dtBack,
								   g.isTheKeyAvailable,
								   g.mMandoob.mandoobName
							   }).FirstOrDefault();
			return Json(unitKeyInfo);
		}

		public JsonResult geMandoobs(int IdMandoob)
		{
			List<Mandoobs> mandoobs = _context.TMandoobs.ToList();
			if (IdMandoob != 0)
			{
				mandoobs = mandoobs.Where((Mandoobs t) => t.IdMandoob == IdMandoob).ToList();
			}
			return Json(new SelectList(mandoobs, "IdMandoob", "mandoobName"));
		}
	}
}
