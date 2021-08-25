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
    public class CompoundUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;

        public CompoundUnitsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index(int IdCompound)
        {
            IQueryable<CompoundUnits> applicationDbContext = from m in _context.TCompoundUnits.Include((CompoundUnits c) => c.mCompoundBuilding).Include((CompoundUnits c) => c.mFloor).Include((CompoundUnits c) => c.mMandoob)
                    .Include((CompoundUnits c) => c.mPropertyType)
                    .Include((CompoundUnits c) => c.mUserCreated)
                    .Include((CompoundUnits c) => c.mUserModified)
                                                             where m.mCompoundBuilding.IdCompound == IdCompound
                                                             select m;
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            CompoundUnits compoundUnits = await _context.TCompoundUnits.SingleOrDefaultAsync((CompoundUnits m) => (int?)m.IdUnit == id);
            if (compoundUnits == null)
            {
                return NotFound();
            }
            int IdCompound = int.Parse((await (from m in _context.TCompoundBuildings
                                               where m.IdBuilding == compoundUnits.IdBuilding
                                               select new
                                               {
                                                   m.IdCompound
                                               }).SingleOrDefaultAsync()).IdCompound.ToString());
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            var unitElectricityData = _context.ElectricityMeters.Where(e => e.CompoundUnitID == id).Select(e => new
            {
                e.ElectricityMeterNumber,
                e.PaymentNumber,
                e.TransferTheAccountToTenant
            }).FirstOrDefault();
            compoundUnits.ElectricityMeterNumber = unitElectricityData.ElectricityMeterNumber;
            compoundUnits.ElectricityPaymentNumber = unitElectricityData.PaymentNumber;
            compoundUnits.TransferToTenanat = unitElectricityData.TransferTheAccountToTenant;
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings, "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(compoundUnits);
        }

        public IActionResult Create(int IdCompound, string CompoundName)
        {
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings.Where((CompoundBuildings m) => m.IdCompound == IdCompound), "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            base.ViewData["CompoundName"] = CompoundName;
            base.ViewData["IdCompound"] = IdCompound;
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompoundUnits compoundUnits)
        {
            int IdCompound = int.Parse((await (from m in _context.TCompoundBuildings
                                               where m.IdBuilding == compoundUnits.IdBuilding
                                               select new
                                               {
                                                   m.IdCompound
                                               }).SingleOrDefaultAsync()).IdCompound.ToString());
            if (base.ModelState.IsValid)
            {
                compoundUnits.dtCreated = DateTime.Now;
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    compoundUnits.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
                }
                _context.Add(compoundUnits);
                try
                {
                    await _context.SaveChangesAsync();
                    _context.ElectricityMeters.Add(new ElectricityMeter
                    {
                        CompoundUnitID = compoundUnits.IdUnit,
                        PaymentNumber = compoundUnits.ElectricityPaymentNumber,
                        ElectricityMeterNumber = compoundUnits.ElectricityMeterNumber,
                        TransferTheAccountToTenant = compoundUnits.TransferToTenanat
                    });
                    _context.SaveChanges();
                    return RedirectToAction("Index", new
                    {
                        IdCompound
                    });
                }
                catch
                {
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Compound Unit. Please correct and try again.";
                }
            }
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings, "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(compoundUnits);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            CompoundUnits compoundUnits = await _context.TCompoundUnits.SingleOrDefaultAsync((CompoundUnits m) => (int?)m.IdUnit == id);
            if (compoundUnits == null)
            {
                return NotFound();
            }
            int IdCompound = int.Parse((await (from m in _context.TCompoundBuildings
                                               where m.IdBuilding == compoundUnits.IdBuilding
                                               select new
                                               {
                                                   m.IdCompound
                                               }).SingleOrDefaultAsync()).IdCompound.ToString());
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            var unitElectricityData = _context.ElectricityMeters.Where(e => e.CompoundUnitID == id).Select(e => new
            {
                e.ElectricityMeterNumber,
                e.PaymentNumber,
                e.TransferTheAccountToTenant
            }).FirstOrDefault();
            compoundUnits.ElectricityMeterNumber = unitElectricityData.ElectricityMeterNumber;
            compoundUnits.ElectricityPaymentNumber = unitElectricityData.PaymentNumber;
            compoundUnits.TransferToTenanat = unitElectricityData.TransferTheAccountToTenant;
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings.Where((CompoundBuildings m) => m.IdCompound == IdCompound), "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(compoundUnits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompoundUnits compoundUnits)
        {
            if (id != compoundUnits.IdUnit)
            {
                return NotFound();
            }
            int IdCompound = int.Parse((await (from m in _context.TCompoundBuildings
                                               where m.IdBuilding == compoundUnits.IdBuilding
                                               select new
                                               {
                                                   m.IdCompound
                                               }).SingleOrDefaultAsync()).IdCompound.ToString());
            if (base.ModelState.IsValid)
            {
                try
                {
                    compoundUnits.dtModified = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        compoundUnits.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
                    }
                    var electricity = _context.ElectricityMeters.FirstOrDefault(e => e.CompoundUnitID == compoundUnits.IdUnit);
                    electricity.PaymentNumber = compoundUnits.ElectricityPaymentNumber;
                    electricity.TransferTheAccountToTenant = compoundUnits.TransferToTenanat;
                    electricity.ElectricityMeterNumber = compoundUnits.ElectricityMeterNumber;
                    _context.Update(compoundUnits);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompoundUnitsExists(compoundUnits.IdUnit))
                    {
                        return NotFound();
                    }
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the compound unit. Please correct and try again.";
                }
                var CompoundInfo = await (from m in _context.TCompounds
                                          where m.IdCompound == IdCompound
                                          select new
                                          {
                                              m.compoundName,
                                              m.IdCompound
                                          }).SingleOrDefaultAsync();
                base.ViewData["CompoundName"] = CompoundInfo.compoundName;
                base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
                return RedirectToAction("Index", new
                {
                    IdCompound
                });
            }
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings.Where((CompoundBuildings m) => m.IdCompound == IdCompound), "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(compoundUnits);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            CompoundUnits compoundUnits = await _context.TCompoundUnits.Include((CompoundUnits c) => c.mCompoundBuilding).Include((CompoundUnits c) => c.mFloor).Include((CompoundUnits c) => c.mMandoob)
                .Include((CompoundUnits c) => c.mPropertyType)
                .Include((CompoundUnits c) => c.mUserCreated)
                .Include((CompoundUnits c) => c.mUserModified)
                .SingleOrDefaultAsync((CompoundUnits m) => (int?)m.IdUnit == id);
            if (compoundUnits == null)
            {
                return NotFound();
            }
            int IdCompound = int.Parse((await (from m in _context.TCompoundBuildings
                                               where m.IdBuilding == compoundUnits.IdBuilding
                                               select new
                                               {
                                                   m.IdCompound
                                               }).SingleOrDefaultAsync()).IdCompound.ToString());
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IdBuilding"] = new SelectList(_context.TCompoundBuildings.Where((CompoundBuildings m) => m.IdCompound == IdCompound), "IdBuilding", "BuildingNumber");
            base.ViewData["IdFloor"] = new SelectList(_context.TBuildingFloors, "IdBuildingFloor", "PropertyFloorName");
            base.ViewData["IdPropertyType"] = new SelectList(_context.TPropertyTypes, "IdPropertyType", "PropertyTypeName");
            ViewData["IdOwner"] = Enums.GetEnumAsSelectList<Enums.UnitOwner>();
            return View(compoundUnits);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            CompoundUnits compoundUnits = await _context.TCompoundUnits.Include((CompoundUnits m) => m.mCompoundBuilding).SingleOrDefaultAsync((CompoundUnits m) => m.IdUnit == id);
            var electricityMeter = _context.ElectricityMeters.FirstOrDefault(e => e.CompoundUnitID == id);
            var unitContract = _context.TUnitRentContract.FirstOrDefault(f => f.IdUnitCompound == id);
            try
            {
                if (unitContract != null)
                    throw new InvalidOperationException($"This unit has rent contract with number {unitContract.contractNumber}");

                _context.ElectricityMeters.Remove(electricityMeter);
                _context.TCompoundUnits.Remove(compoundUnits);
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
            return RedirectToAction("Index", new
            {
                compoundUnits.mCompoundBuilding.IdCompound
            });
        }

        private bool CompoundUnitsExists(int id)
        {
            return _context.TCompoundUnits.Any((CompoundUnits e) => e.IdUnit == id);
        }
    }
}
