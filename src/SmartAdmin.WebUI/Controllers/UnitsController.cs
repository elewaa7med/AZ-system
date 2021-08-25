using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class UnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;

        public UnitsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index(int id)
        {
            ViewBag.id = id;
            return View(await (from m in _context.Units.Where(x=>x.idMasterBuilding ==id).Include((Units u) => u.mBuilding).Include((Units u) => u.mFloor).Include((Units u) => u.mPropertyType)
                               orderby m.mBuilding.BuildingName, m.UnitNumber
                               select m).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Units units = await _context.Units.Include((Units u) => u.mBuilding).Include((Units u) => u.mFloor).Include((Units u) => u.mPropertyType)
                .Include((Units u) => u.mUserCreated)
                .Include((Units u) => u.mUserModified)
                .SingleOrDefaultAsync((Units m) => (int?)m.IdUnit == id);
            if (units == null)
            {
                return NotFound();
            }
            var unitElectricityData = _context.ElectricityMeters.Where(e => e.UnitID == id).Select(e => new
            {
                e.ElectricityMeterNumber,
                e.PaymentNumber,
                e.TransferTheAccountToTenant
            }).FirstOrDefault();
            units.ElectricityMeterNumber = unitElectricityData.ElectricityMeterNumber;
            units.ElectricityPaymentNumber = unitElectricityData.PaymentNumber;
            units.TransferToTenanat = unitElectricityData.TransferTheAccountToTenant;
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingName", units.IdBuilding);
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName", units.IdFloor);
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName", units.IdPropertyType);
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(units);
        }

        public IActionResult Create(int id)
        {
            ViewBag.id = id;
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingName");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName");
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Units units)
        {
            if (base.ModelState.IsValid)
            {
                units.dtCreated = DateTime.Now;
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    units.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
                }
                _context.Add(units);
                try
                {
                    await _context.SaveChangesAsync();
                    _context.ElectricityMeters.Add(new ElectricityMeter { UnitID = units.IdUnit, PaymentNumber = units.ElectricityPaymentNumber, ElectricityMeterNumber = units.ElectricityMeterNumber, TransferTheAccountToTenant = units.TransferToTenanat });
                    _context.SaveChanges();
                    return RedirectToAction("Index",new { id = units.idMasterBuilding });
                }
                catch(Exception e)
                {
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Unit. Please correct and try again.";
                }
            }
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingName", units.IdBuilding);
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName", units.IdFloor);
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName", units.IdPropertyType);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdModifiedBy);
            ViewBag.id = units.idMasterBuilding;
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(units);
        }

        public async Task<IActionResult> Edit(int pageid,int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Units units = await _context.Units.Where(x=>x.idMasterBuilding==pageid).SingleOrDefaultAsync((Units m) => (int?)m.IdUnit == id);
            if (units == null)
            {
                return NotFound();
            }
            var unitElectricityData = _context.ElectricityMeters.Where(e => e.UnitID == id).Select(e => new
            {
                e.ElectricityMeterNumber,
                e.PaymentNumber,
                e.TransferTheAccountToTenant
            }).FirstOrDefault();
            units.ElectricityMeterNumber = unitElectricityData.ElectricityMeterNumber;
            units.ElectricityPaymentNumber = unitElectricityData.PaymentNumber;
            units.TransferToTenanat = unitElectricityData.TransferTheAccountToTenant;
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings.Where(x=>x.idMasterBuilding==pageid), "IdBuilding", "BuildingName", units.IdBuilding);
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName", units.IdFloor);
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName", units.IdPropertyType);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdModifiedBy);
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();

            return View(units);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Units units)
        {
            if (id != units.IdUnit)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                try
                {
                    units.dtModified = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        units.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
                    }
                    var electricity = _context.ElectricityMeters.FirstOrDefault(e => e.UnitID == units.IdUnit);
                    electricity.PaymentNumber = units.ElectricityPaymentNumber;
                    electricity.TransferTheAccountToTenant = units.TransferToTenanat;
                    electricity.ElectricityMeterNumber = units.ElectricityMeterNumber;
                    _context.Update(units);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitsExists(units.IdUnit))
                    {
                        return NotFound();
                    }
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Unit. Please correct and try again.";
                }
                return RedirectToAction("Index",new { id = units.idMasterBuilding });
            }
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingName", units.IdBuilding);
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName", units.IdFloor);
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName", units.IdPropertyType);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", units.IdModifiedBy);
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(units);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Units units = await _context.Units.Include((Units u) => u.mBuilding).Include((Units u) => u.mFloor).Include((Units u) => u.mPropertyType)
                .Include((Units u) => u.mUserCreated)
                .Include((Units u) => u.mUserModified)
                .SingleOrDefaultAsync((Units m) => (int?)m.IdUnit == id);
            if (units == null)
            {
                return NotFound();
            }
            base.ViewData["IdBuilding"] = new SelectList(_context.TBuildings, "IdBuilding", "BuildingName", units.IdBuilding);
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName", units.IdFloor);
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName", units.IdPropertyType);
            return View(units);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id ,int pageid)
        {
            Units units = await _context.Units.SingleOrDefaultAsync((Units m) => m.IdUnit == id);
            var electricityMeter = _context.ElectricityMeters.FirstOrDefault(e => e.UnitID == id);
            var unitContract = _context.TUnitRentContract.FirstOrDefault(f => f.IdUnit == id);
            try
            {
                if (unitContract != null)
                    throw new InvalidOperationException($"This unit has rent contract with number {unitContract.contractNumber}");

                _context.ElectricityMeters.Remove(electricityMeter);
                _context.Units.Remove(units);
                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                base.TempData["AlertSaveErr"] = ex.Message;
            }
            catch
            {
                base.TempData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
            }
            return RedirectToAction("Index",new { id = pageid });
        }

        private bool UnitsExists(int id)
        {
            return _context.Units.Any((Units e) => e.IdUnit == id);
        }
    }
}
