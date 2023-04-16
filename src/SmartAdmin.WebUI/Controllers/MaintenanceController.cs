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
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user;
        public MaintenanceController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }
        public ActionResult GetCompound(int compoundID)
        {
            var model = _context.Maintenances.Include(e => e.CompoundUnit)
                                             .Include(u => u.CompoundUnit.mCompoundBuilding)
                                             .Include(e => e.User)
                                             .Where(e => e.CompoundUnitID.HasValue && e.CompoundUnit.mCompoundBuilding.IdCompound == compoundID)
                                             .OrderBy(x=>x.MaintenanceEndDate)
                                             .ToList();
            ViewBag.CompoundID = compoundID;
            ViewBag.CompoundName = _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName;
            return View(model);
        }

        public ActionResult GetBuilding(int id)
        {
            ViewBag.id = id;
            var model = _context.Maintenances.Include(e => e.Unit)
                                             .Include(u => u.Unit.mBuilding)
                                             .Include(e => e.User)
                                             .Where(e => e.UnitID.HasValue && e.Unit.idMasterBuilding == id)
                                              .OrderBy(x => x.MaintenanceEndDate)
                                             .ToList();
            return View(model);
        }

        public ActionResult Create(int? id = null,int? compoundID = null)
        {
            ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                      new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding == id).Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName");
            ViewBag.IsCompound = compoundID.HasValue;
            ViewBag.CompoundID = compoundID;
            ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
            ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
            ViewBag.id = id;
            var maxMaintenanceIndex = _context.Maintenances.Count() + 1;
            return View(new Maintenance { InvoiceNo = maxMaintenanceIndex.ToString().PadLeft(6, '0') });
        }

        [HttpPost]
        public ActionResult Create([Bind(new string[]
        {
            "UnitID,CompoundUnitID,Cost,Description,UserID,MaintenanceEndDate,Unit,CompoundUnit,User,BuildingID,Plumbing,PlumbingDesc,Electricity,ElectricityDesc,Paint,PaintDesc,Tiles,TilesDesc,Toilet,ToiletDesc,WaterHeater,WaterHeaterDesc,Kitchen,KitchenDesc,Conditioning," +
            "ConditioningDesc,Carpentry,CarpentryDesc,Waste,WasteDesc,Others,OthersDesc,TotalAmount,InvoiceNo,CreatedOn"
        })] Maintenance model, int? compoundID = null, int? id = null)
        {
            try
            {
                model.CreatedOn = DateTime.Now;
                _context.Add(model);
                _context.SaveChanges();
                return compoundID.HasValue ? RedirectToAction("GetCompound", new { compoundID }) : RedirectToAction("GetBuilding",new { id = id });
            }
            catch (Exception e)
            {
                ViewBag.AlertSaveErr = "UnExpected error, Please try again";
                ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                       new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding==id), "IdBuilding", "BuildingName");
                ViewBag.id = id;
                ViewBag.IsCompound = compoundID.HasValue;
                ViewBag.CompoundID = compoundID;
                ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
                ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
                return View(model);
            }
             
        }
        public ActionResult Edit(int id,int? pageid=null)
        {
            ViewBag.id = pageid;
            var maintenance = _context.Maintenances.Include(u => u.Unit)
                                                   .Include(u => u.CompoundUnit)
                                                   .Include(u => u.CompoundUnit.mCompoundBuilding)
                                                   .FirstOrDefault(e => e.ID == id);
            maintenance.BuildingID = maintenance.CompoundUnitID.HasValue ? maintenance.CompoundUnit.IdBuilding : maintenance.Unit.IdBuilding;
            ViewBag.Buildings = maintenance.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", maintenance.BuildingID) :
                                                                      new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding == pageid).Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", maintenance.BuildingID);
            ViewBag.IsCompound = maintenance.CompoundUnitID.HasValue;
            ViewBag.CompoundID = maintenance?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = maintenance?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
            return View(maintenance);
        }

        [HttpPost]
        public ActionResult Edit(Maintenance model, int? pageid = null, int? compoundID = null)
        {
            try
            {
                _context.Maintenances.Update(model);
                _context.SaveChanges();
                return compoundID.HasValue ? RedirectToAction("GetCompound", new { compoundID }) : RedirectToAction("GetBuilding",new { id = pageid });
            }
            catch (Exception)
            {
                ViewBag.AlertSaveErr = "UnExpected error, Please try again";
                ViewBag.Buildings = compoundID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == compoundID), "IdBuilding", "BuildingNumber") :
                                                          new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding == pageid).Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName");
                ViewBag.IsCompound = compoundID.HasValue;
                ViewBag.CompoundID = compoundID;
                ViewBag.CompoundName = compoundID.HasValue ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName : string.Empty;
                ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
                ViewBag.id = pageid;
                return View(model);
            }
        }
        public ActionResult Details(int id, int? pageid)
        {
            var maintenance = _context.Maintenances.Include(u => u.Unit)
                                             .Include(u => u.CompoundUnit)
                                             .Include(u => u.Unit.mBuilding)
                                             .Include(u => u.CompoundUnit.mCompoundBuilding)
                                             .FirstOrDefault(e => e.ID == id);
            maintenance.BuildingID = maintenance.CompoundUnitID.HasValue ? maintenance.CompoundUnit.IdBuilding : maintenance.Unit.IdBuilding;
            ViewBag.Buildings = maintenance.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", maintenance.BuildingID) :
                                                                new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", maintenance.BuildingID);
            ViewBag.IsCompound = maintenance.CompoundUnitID.HasValue;
            ViewBag.CompoundID = maintenance?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = maintenance?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
            ViewBag.id = pageid;
            return View(maintenance);
        }
        public ActionResult Delete( int id,int? pageid = null)
        {
            ViewBag.id = pageid;
            var maintenance = _context.Maintenances.Include(u => u.Unit)
                                                   .Include(u => u.CompoundUnit)
                                                   .Include(u => u.Unit.mBuilding)
                                                   .Include(u => u.CompoundUnit.mCompoundBuilding)
                                                   .FirstOrDefault(e => e.ID == id);
            maintenance.BuildingID = maintenance.CompoundUnitID.HasValue ? maintenance.CompoundUnit.IdBuilding : maintenance.Unit.IdBuilding;
            ViewBag.Buildings = maintenance.CompoundUnitID.HasValue ? new SelectList(_context.TCompoundBuildings.Where(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound), "IdBuilding", "BuildingNumber", maintenance.BuildingID) :
                                                                      new SelectList(_context.TBuildings.Select(b => new { b.IdBuilding, BuildingName = b.BuildingName + " - " + b.mDistrict.DistrictName }), "IdBuilding", "BuildingName", maintenance.BuildingID);
            ViewBag.IsCompound = maintenance.CompoundUnitID.HasValue;
            ViewBag.CompoundID = maintenance?.CompoundUnit?.mCompoundBuilding.IdCompound;
            ViewBag.CompoundName = maintenance?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            ViewBag.Mandoobs = new SelectList(_context.Users, "Id", "fullName");
            return View(maintenance);
        }
        [HttpPost]
        public ActionResult ConfirmDelete(int id, int? pageid = null, int? compoundID = null)
        {
            try
            {
                var maintenance = _context.Maintenances.FirstOrDefault(e => e.ID == id);
                _context.Maintenances.Remove(maintenance);
                _context.SaveChanges();
                TempData["success"] = "Operation is done successfully";
                return compoundID.HasValue ? RedirectToAction("GetCompound", new { compoundID }) : RedirectToAction("GetBuilding",new { id = pageid });
            }
            catch (Exception)
            {
                TempData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";
                return RedirectToAction("Delete", new { id });
            }
        }
        public JsonResult GetBuildingUnits(int buildingID)
        {
            var result = _context.Units.Where(e => e.IdBuilding == buildingID && e.UnitOwner != Enums.UnitOwner.xSaad)
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
            var result = _context.TCompoundUnits.Where(e => e.IdBuilding == buildingID && e.UnitOwner != Enums.UnitOwner.xSaad)
                                                .OrderBy(e => e.UnitNumber)
                                                .Select(e => new
                                                {
                                                    e.IdUnit,
                                                    e.UnitNumber,
                                                }).ToList();
            var selectUnitList = new SelectList(result, "IdUnit", "UnitNumber");
            return Json(selectUnitList);
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
        public ActionResult Print(int id,int? pageid)
        {
            var maintenance = _context.Maintenances.Include(u => u.Unit)
                                                   .Include(u => u.CompoundUnit)
                                                   .Include(u => u.CompoundUnit.mCompoundBuilding)
                                                   .FirstOrDefault(e => e.ID == id);
            ViewBag.CompoundName = maintenance?.CompoundUnit?.mCompoundBuilding != null ? _context.TCompounds.FirstOrDefault(c => c.IdCompound == maintenance.CompoundUnit.mCompoundBuilding.IdCompound).compoundName : string.Empty;
            ViewBag.id = pageid;
            return View(maintenance);
        }
    }
}
