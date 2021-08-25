using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class BuildingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;

        public BuildingsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index(int id , int IdBuilding = 0)
        {
            ViewBag.id = id;
            if (IdBuilding != 0)
            {
                return View(await (from m in _context.TBuildings.Where(x => x.idMasterBuilding == id).Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                        .Include((Buildings b) => b.mOwner)
                        .Include((Buildings b) => b.mUserCreated)
                        .Include((Buildings b) => b.mUserModified)
                                   where m.IdBuilding == IdBuilding
                                   select m).ToListAsync());
            }
            return View(await _context.TBuildings.Where(x=>x.idMasterBuilding ==id).Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                .Include((Buildings b) => b.mOwner)
                .Include((Buildings b) => b.mUserCreated)
                .Include((Buildings b) => b.mUserModified)
                .ToListAsync());
        }

        public async Task<IActionResult> LoadAllInPartial(int IdBuilding = 0)
        {
            if (IdBuilding != 0)
            {
                return View(await (from m in _context.TBuildings.Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                        .Include((Buildings b) => b.mOwner)
                        .Include((Buildings b) => b.mUserCreated)
                        .Include((Buildings b) => b.mUserModified)
                                   where m.IdBuilding == IdBuilding
                                   select m).ToListAsync());
            }
            return View(await _context.TBuildings.Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                .Include((Buildings b) => b.mOwner)
                .Include((Buildings b) => b.mUserCreated)
                .Include((Buildings b) => b.mUserModified)
                .ToListAsync());


        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Buildings buildings = await _context.TBuildings.Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                .Include((Buildings b) => b.mOwner)
                .Include((Buildings b) => b.mUserCreated)
                .Include((Buildings b) => b.mUserModified)
                .SingleOrDefaultAsync((Buildings m) => (int?)m.IdBuilding == id);
            if (buildings == null)
            {
                return NotFound();
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", buildings.IdCountry);
            base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities u) => u.IdCountry == buildings.IdCountry), "IdCity", "CityName", buildings.IdCity);
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts u) => u.IdCity == buildings.IdCity), "IdDistrict", "DistrictName", buildings.IdDistrict);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", buildings.IDOwner);
            return View(buildings);
        }

        public IActionResult Create(int id)
        {
            ViewBag.id = id;
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName");
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName");
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(new string[]
        {
            "IdBuilding,BuildingName,idMasterBuilding,BuildingAddress,BuildingValue,BuildingYearlyIncome,BuildingArea,GPSLink,BuildingPicture,BuildingYear,BuildingNotes,ForFamilies,dtCreated,dtModified,IdCreatedBy,IdModifiedBy,IdDistrict,IdCity,IdCountry,IDOwner,MeterWaterNumber"
        })] Buildings buildings, IFormFile buildingImageFile,int pageid)
        {
            string frmIdCity = base.HttpContext.Request.Form["IdCity"].ToString();
            string frmIdDistrict = base.HttpContext.Request.Form["IdDistrict"].ToString();
            ModelState.ValidationState.ToString();
            if (base.ModelState.IsValid)
            {
                try
                {
                    buildings.IdCity = int.Parse(frmIdCity);
                    buildings.IdDistrict = int.Parse(frmIdDistrict);
                }
                catch (Exception)
                {
                    throw;
                }
                if (buildingImageFile != null && buildingImageFile.Length != 0L)
                {
                    string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Buildings\\", Path.GetFileName(buildingImageFile.FileName));
                    using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                    {
                        await buildingImageFile.CopyToAsync(imgfileStream);
                    }
                    buildings.BuildingPicture = Path.GetFileName(buildingImageFile.FileName);
                }
                buildings.dtCreated = DateTime.Now;
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    buildings.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
                }
                _context.Add(buildings);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = buildings.idMasterBuilding });
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", buildings.IdCountry);
            base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities u) => u.IdCountry == buildings.IdCountry), "IdCity", "CityName", buildings.IdCity);
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts u) => u.IdCity == buildings.IdCity), "IdDistrict", "DistrictName", buildings.IdDistrict);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", buildings.IDOwner);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdModifiedBy);
            ViewBag.id = buildings.idMasterBuilding;
            return View(buildings);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Buildings buildings = await _context.TBuildings.SingleOrDefaultAsync((Buildings m) => (int?)m.IdBuilding == id);
            if (buildings == null)
            {
                return NotFound();
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", buildings.IdCountry);
            base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities u) => u.IdCountry == buildings.IdCountry), "IdCity", "CityName", buildings.IdCity);
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts u) => u.IdCity == buildings.IdCity), "IdDistrict", "DistrictName", buildings.IdDistrict);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", buildings.IDOwner);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdModifiedBy);
            return View(buildings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(new string[]
        {
            "IdBuilding,BuildingName,BuildingAddress,BuildingValue,BuildingYearlyIncome,BuildingArea,GPSLink,BuildingPicture,BuildingYear,BuildingNotes,ForFamilies,dtCreated,dtModified,IdCreatedBy,IdModifiedBy,IdDistrict,IdCity,IdCountry,IDOwner,MeterWaterNumber,idMasterBuilding"
        })] Buildings buildings, IFormFile buildingImageFile)
        {
            if (id != buildings.IdBuilding)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                try
                {
                    string frmIdCity = base.HttpContext.Request.Form["IdCity"].ToString();
                    string frmIdDistrict = base.HttpContext.Request.Form["IdDistrict"].ToString();
                    try
                    {
                        buildings.IdCity = int.Parse(frmIdCity);
                        buildings.IdDistrict = int.Parse(frmIdDistrict);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    if (buildingImageFile != null && buildingImageFile.Length != 0L)
                    {
                        string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Buildings\\", Path.GetFileName(buildingImageFile.FileName));
                        using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                        {
                            await buildingImageFile.CopyToAsync(imgfileStream);
                        }
                        buildings.BuildingPicture = Path.GetFileName(buildingImageFile.FileName);
                    }
                    buildings.dtModified = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        buildings.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
                    }
                    _context.Update(buildings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingsExists(buildings.IdBuilding))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index",new { id = buildings.idMasterBuilding });
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", buildings.IdCountry);
            base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities u) => u.IdCountry == buildings.IdCountry), "IdCity", "CityName", buildings.IdCity);
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts u) => u.IdCity == buildings.IdCity), "IdDistrict", "DistrictName", buildings.IdDistrict);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", buildings.IDOwner);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdModifiedBy);
            return View(buildings);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Buildings buildings = await _context.TBuildings.Include((Buildings b) => b.mCity).Include((Buildings b) => b.mCountry).Include((Buildings b) => b.mDistrict)
                .Include((Buildings b) => b.mOwner)
                .Include((Buildings b) => b.mUserCreated)
                .Include((Buildings b) => b.mUserModified)
                .SingleOrDefaultAsync((Buildings m) => (int?)m.IdBuilding == id);
            if (buildings == null)
            {
                return NotFound();
            }
            base.ViewData["IdCountry"] = new SelectList(_context.TCountries, "IdCountry", "CountryName", buildings.IdCountry);
            base.ViewData["IdCity"] = new SelectList(_context.TCities.Where((Cities u) => u.IdCountry == buildings.IdCountry), "IdCity", "CityName", buildings.IdCity);
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts.Where((Districts u) => u.IdCity == buildings.IdCity), "IdDistrict", "DistrictName", buildings.IdDistrict);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", buildings.IDOwner);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", buildings.IdModifiedBy);
            return View(buildings);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int pageid , int id)
        {
            Buildings buildings = await _context.TBuildings.Include(b => b.mUnits).SingleOrDefaultAsync((Buildings m) => m.IdBuilding == id);
            try
            {
                if (buildings.mUnits.Any(u => u.UnitRentContractID.HasValue))
                    throw new InvalidOperationException("This building has some units that has Contacts");
                _context.TBuildings.Remove(buildings);
                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                base.TempData["AlertSaveErr"] = ex.Message;
            }
            catch (Exception ex)
            {
                base.TempData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
            }
            return RedirectToAction("Index",new { id = pageid });
        }

        private bool BuildingsExists(int id)
        {
            return _context.TBuildings.Any((Buildings e) => e.IdBuilding == id);
        }

        public JsonResult getDistrictsByCities(int IdCity)
        {
            List<Districts> Districts2 = new List<Districts>();
            Districts2 = _context.TDistricts.Where((Districts u) => u.IdCity == IdCity).ToList();
            base.ViewData["IdDistrict"] = new SelectList(Districts2, "IdDistrict", "DistrictName");
            return Json(new SelectList(Districts2, "IdDistrict", "DistrictName"));
        }

        public JsonResult getCitiesByCountries(int IdCountry)
        {
            List<Cities> Cities2 = new List<Cities>();
            Cities2 = _context.TCities.Where((Cities u) => u.IdCountry == IdCountry).ToList();
            base.ViewData["IdCity"] = new SelectList(Cities2, "IdCity", "CityName");
            return Json(new SelectList(Cities2, "IdCity", "CityName"));
        }

        public async Task<JsonResult> getBuildingsAutoComplete(string query)
        {
            return Json(await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                               select new
                               {
                                   m.IdBuilding,
                                   m.BuildingName,
                                   m.BuildingAddress,
                                   m.mDistrict.DistrictName
                               } into u
                               where u.BuildingName.ToLower().StartsWith(query) || u.BuildingAddress.ToLower().Contains(query) || u.DistrictName.ToLower().Contains(query)
                               select u).ToListAsync());
        }
    }
}
