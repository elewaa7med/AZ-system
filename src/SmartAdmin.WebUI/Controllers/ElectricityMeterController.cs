using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Authorization;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class ElectricityMeterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user;
        public ElectricityMeterController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }
        public ActionResult GetBuildingMeters(int id ,string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var model = new List<ElectricityMeter>();
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.BuildingAccountant) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
                var query = _context.ElectricityMeters.Where(x=>x.Unit.idMasterBuilding == id).Include(e => e.Unit).ThenInclude(e => e.UnitRentContract).ThenInclude(u => u.Mandoob).Include(e => e.Unit.mBuilding).Where(e => e.UnitID.HasValue && e.Unit.idMasterBuilding == id);
                if (!string.IsNullOrEmpty(representitveId))
                    query = query.Where(q => q.Unit.isRented == true && q.Unit.UnitRentContract.IdMandoob == representitveId);

                model = query.ToList();
            }
            else
                model = _context.ElectricityMeters.Include(e => e.Unit).ThenInclude(e => e.UnitRentContract).ThenInclude(u => u.Mandoob).Include(e => e.Unit.mBuilding).Where(e => e.UnitID.HasValue && e.Unit.idMasterBuilding == id).ToList();
            ViewBag.id = id;
            return View(model);
        }
        public ActionResult GetCompoundMeters(int compoundID, string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var model = new List<ElectricityMeter>();
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Any(s => s == Permission.MeadowParkGarden || s == Permission.DesertRose || s == Permission.DaarResidence || s == Permission.Villa21 || s == Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
                var query = _context.ElectricityMeters.Include(e => e.CompoundUnit).ThenInclude(e => e.UnitRentContract).ThenInclude(u => u.Mandoob).Include(e => e.CompoundUnit.mCompoundBuilding).Include(c => c.CompoundUnit.mPropertyType).Where(e => e.CompoundUnitID.HasValue && e.CompoundUnit.mCompoundBuilding.IdCompound == compoundID);
                if (!string.IsNullOrEmpty(representitveId))
                    query = query.Where(q => q.CompoundUnit.isRented == true && q.CompoundUnit.UnitRentContract.IdMandoob == representitveId);

                model = query.ToList();
            }
            else
                model = _context.ElectricityMeters.Include(e => e.CompoundUnit).ThenInclude(e => e.UnitRentContract).ThenInclude(u => u.Mandoob).Include(e => e.CompoundUnit.mCompoundBuilding).Include(c => c.CompoundUnit.mPropertyType).Where(e => e.CompoundUnitID.HasValue && e.CompoundUnit.mCompoundBuilding.IdCompound == compoundID).ToList();

            ViewBag.CompoundID = compoundID;

            ViewBag.CompoundName = _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName;
            return View(model);
        }
        public ActionResult Create(int? compoundID = null)
        {
            ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                      new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName");
            ViewBag.IsCompound = compoundID.HasValue;
            ViewBag.CompoundID = compoundID;
            ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
            return View(new ElectricityMeter());
        }
        [HttpPost]
        public ActionResult Create(ElectricityMeter model, int? compoundID = null)
        {
            try
            {
                _context.ElectricityMeters.Add(model);
                _context.SaveChanges();
                return compoundID.HasValue ? RedirectToAction("GetCompoundMeters", new { compoundID = compoundID }) : RedirectToAction("GetBuildingMeters");
            }
            catch (Exception)
            {
                ViewBag.AlertSaveErr = "UnExpected error, Please try again";
                ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                      new SelectList(_context.TBuildings, "IdBuilding", "BuildingName");
                ViewBag.IsCompound = compoundID.HasValue;
                ViewBag.CompoundID = compoundID;
                ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
                return View(model);
            }
        }
        public JsonResult GetBuildingUnits(int buildingID)
        {
            var result = _context.Units.Where(e => e.IdBuilding == buildingID && e.UnitOwner != Enums.UnitOwner.Saad)
                                       .OrderBy(e => e.UnitNumber)
                                       .Select(e => new
                                       {
                                           e.IdUnit,
                                           e.UnitNumber,
                                       }).ToList();
            var selectUnitList = new SelectList(result, "IdUnit", "UnitNumber");
            return Json(selectUnitList);
        }
        public JsonResult GetCompoundBuildingUnits(int buildingID)
        {
            var result = _context.TCompoundUnits.Where(e => e.IdBuilding == buildingID && e.UnitOwner != Enums.UnitOwner.Saad)
                                                .OrderBy(e => e.UnitNumber)
                                                .Select(e => new
                                                {
                                                    e.IdUnit,
                                                    e.UnitNumber,
                                                }).ToList();
            var selectUnitList = new SelectList(result, "IdUnit", "UnitNumber");
            return Json(selectUnitList);
        }
        public ActionResult Edit(int id, int pageid)
        {
            var meter = _context.ElectricityMeters.Include(u => u.Unit).Include(u => u.CompoundUnit).ThenInclude(u => u.mCompoundBuilding)
                                                  .Include(u => u.Unit.UnitRentContract)
                                                  .Include(u => u.CompoundUnit.UnitRentContract)
                                                  .Include(u => u.Unit.UnitRentContract)
                                                  .FirstOrDefault(e => e.ID == id);
            meter.BuildingID = meter.CompoundUnitID.HasValue ? meter.CompoundUnit.IdBuilding : meter.Unit.IdBuilding;
            ViewBag.Buildings = meter.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", meter.BuildingID) :
                                                                new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding == pageid).Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", meter.BuildingID);
            ViewBag.IsCompound = meter.CompoundUnitID.HasValue;
            ViewBag.CompoundID = meter?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = meter?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            return View(meter);
        }
        [HttpPost]
        public ActionResult Edit(ElectricityMeter model,int pageid, int? compoundID = null)
        {
            try
            {
                _context.ElectricityMeters.Update(model);
                _context.SaveChanges();
                return compoundID.HasValue ? RedirectToAction("GetCompoundMeters", new { compoundID = compoundID }) : RedirectToAction("GetBuildingMeters" ,new { id = pageid });
            }
            catch (Exception)
            {
                ViewBag.AlertSaveErr = "UnExpected error, Please try again";
                ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                          new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName");
                ViewBag.IsCompound = compoundID.HasValue;
                ViewBag.CompoundID = compoundID;
                ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
                return View(model);
            }
        }
        public ActionResult Details(int id , int pageid)
        {
            var meter = _context.ElectricityMeters.Include(u => u.Unit)
                                                  .Include(u => u.CompoundUnit).ThenInclude(u => u.mCompoundBuilding)
                                                  .Include(u => u.CompoundUnit.UnitRentContract)
                                                  .Include(u => u.Unit.UnitRentContract)
                                                  .FirstOrDefault(e => e.ID == id);
            meter.BuildingID = meter.CompoundUnitID.HasValue ? meter.CompoundUnit.IdBuilding : meter.Unit.IdBuilding;
            ViewBag.Buildings = meter.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", meter.BuildingID) :
                                                                new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", meter.BuildingID);
            ViewBag.IsCompound = meter.CompoundUnitID.HasValue;
            ViewBag.CompoundID = meter?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = meter?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            ViewBag.id = pageid;
            return View(meter);
        }
        public ActionResult Delete(int id)
        {
            var meter = _context.ElectricityMeters.Include(u => u.Unit).Include(u => u.CompoundUnit).ThenInclude(u => u.mCompoundBuilding).FirstOrDefault(e => e.ID == id);
            meter.BuildingID = meter.CompoundUnitID.HasValue ? meter.CompoundUnit.IdBuilding : meter.Unit.IdBuilding;
            ViewBag.Buildings = meter.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", meter.BuildingID) :
                                                                new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", meter.BuildingID);
            ViewBag.IsCompound = meter.CompoundUnitID.HasValue;
            ViewBag.CompoundID = meter?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = meter?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == meter.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            return View(meter);
        }
        [HttpPost]
        public ActionResult ConfirmDelete(int id, int pageid, int? compoundID = null)
        {
            try
            {
                var meter = _context.ElectricityMeters.FirstOrDefault(e => e.ID == id);
                _context.ElectricityMeters.Remove(meter);
                _context.SaveChanges();
                TempData["success"] = "Operation is done successfully";
                return compoundID.HasValue ? RedirectToAction("GetCompoundMeters", new { compoundID = compoundID }) : RedirectToAction("GetBuildingMeters",new
                {
                    id = pageid
                });
            }
            catch (Exception)
            {
                TempData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";
                return RedirectToAction("Delete", new { id });
            }
        }
        public JsonResult GetUnitInfo(int unitID)
        {
            var unitInfo = (from m in _context.Units.Include((Units t) => t.mBuilding).ThenInclude((Buildings t) => t.mDistrict).ThenInclude((Districts t) => t.mCity)
                    .ThenInclude((Cities t) => t.mBuildingCountry)
                    .Include((Units t) => t.mBuilding)
                    .ThenInclude((Buildings t) => t.mOwner)
                            select new
                            {
                                IdUnit = m.IdUnit,
                                unitArea = m.UnitArea,
                                buildingNo = m.mBuilding.BuildingName,
                                buildingAddress = m.mBuilding.BuildingAddress,
                                district = m.mBuilding.mDistrict.DistrictName,
                                city = m.mBuilding.mDistrict.mCity.CityName,
                                country = m.mBuilding.mDistrict.mCity.mBuildingCountry.CountryName,
                                owner = m.mBuilding.mOwner.CompanyName
                            } into m
                            where m.IdUnit == unitID
                            select m).FirstOrDefault();
            return Json(unitInfo);
        }
        public JsonResult GetCompoundUnitInfo(int unitID)
        {
            var unitInfo = (from m in _context.TCompoundUnits.Include((CompoundUnits t) => t.mCompoundBuilding).ThenInclude((CompoundBuildings t) => t.mCompound).ThenInclude((Compounds t) => t.mDistrict)
                    .ThenInclude((Districts m) => m.mCity)
                    .ThenInclude((Cities m) => m.mBuildingCountry)
                    .Include((CompoundUnits m) => m.mCompoundBuilding)
                    .ThenInclude((CompoundBuildings t) => t.mOwner)
                            select new
                            {
                                IdUnitCompound = m.IdUnit,
                                unitArea = m.UnitArea,
                                buildingNo = m.mCompoundBuilding.BuildingNumber,
                                buildingAddress = m.mCompoundBuilding.BuildingAddress,
                                district = m.mCompoundBuilding.mCompound.mDistrict.DistrictName,
                                city = m.mCompoundBuilding.mCompound.mDistrict.mCity.CityName,
                                country = m.mCompoundBuilding.mCompound.mDistrict.mCity.mBuildingCountry.CountryName,
                                owner = m.mCompoundBuilding.mOwner.CompanyName,
                                compoundName = m.mCompoundBuilding.mCompound.compoundName
                            } into m
                            where m.IdUnitCompound == unitID
                            select m).FirstOrDefault();
            return Json(unitInfo);
        }
        public JsonResult GetTenantInfo(int tenantID)
        {
            var tenantInfo = (from m in _context.TTenants.Include(t => t.mNationality).Include(t => t.mCompany).Include(t => t.mNationality)
                              select new
                              {
                                  IdTenant = m.IdTenant,
                                  iqamaId = m.IqamaNo,
                                  mobileNumber = m.tenantMobile,
                                  work = m.mCompany.companyName,
                                  companyPhone = m.mCompany.compayPhone,
                                  nationality = m.mNationality.CountryName
                              } into m
                              where m.IdTenant == tenantID
                              select m).FirstOrDefault();
            return Json(tenantInfo);
        }
    }
}