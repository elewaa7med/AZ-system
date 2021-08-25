using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class CompoundBuildingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;

        public CompoundBuildingsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }

        public async Task<IActionResult> Index(int IdCompound)
        {
            IQueryable<CompoundBuildings> applicationDbContext = from m in _context.TCompoundBuildings.Include((CompoundBuildings c) => c.mCompound).Include((CompoundBuildings c) => c.mOwner).Include((CompoundBuildings c) => c.mUserCreated)
                    .Include((CompoundBuildings c) => c.mUserModified)
                                                                 where m.IdCompound == IdCompound
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
            CompoundBuildings compoundBuildings = await _context.TCompoundBuildings.SingleOrDefaultAsync((CompoundBuildings m) => (int?)m.IdBuilding == id);
            if (compoundBuildings == null)
            {
                return NotFound();
            }
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == compoundBuildings.IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", compoundBuildings.IDOwner);
            return View(compoundBuildings);
        }

        public IActionResult Create(int IdCompound, string CompoundName)
        {
            base.ViewData["CompoundName"] = CompoundName;
            base.ViewData["IdCompound"] = IdCompound;
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompoundBuildings compoundBuildings, IFormFile buildingImageFile)
        {
            if (base.ModelState.IsValid)
            {
                if (buildingImageFile != null && buildingImageFile.Length != 0L)
                {
                    string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Buildings\\", Path.GetFileName(buildingImageFile.FileName));
                    using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                    {
                        await buildingImageFile.CopyToAsync(imgfileStream);
                    }
                    compoundBuildings.BuildingPicture = Path.GetFileName(buildingImageFile.FileName);
                }
                compoundBuildings.dtCreated = DateTime.Now;
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    compoundBuildings.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
                }
                try
                {
                    _context.Add(compoundBuildings);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new
                    {
                        compoundBuildings.IdCompound
                    });
                }
                catch
                {
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the contract. Please correct and try again.";
                }
            }
            base.ViewData["IdCompound"] = new SelectList(_context.TCompounds, "IdCompound", "compoundName", compoundBuildings.IdCompound);
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", compoundBuildings.IDOwner);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "Id", compoundBuildings.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "Id", compoundBuildings.IdModifiedBy);
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == compoundBuildings.IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = compoundBuildings.IdCompound;
            return View(compoundBuildings);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            CompoundBuildings compoundBuildings = await _context.TCompoundBuildings.SingleOrDefaultAsync((CompoundBuildings m) => (int?)m.IdBuilding == id);
            if (compoundBuildings == null)
            {
                return NotFound();
            }
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == compoundBuildings.IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", compoundBuildings.IDOwner);
            return View(compoundBuildings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompoundBuildings compoundBuildings, IFormFile buildingImageFile)
        {
            if (id != compoundBuildings.IdBuilding)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                try
                {
                    if (buildingImageFile != null && buildingImageFile.Length != 0L)
                    {
                        string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Buildings\\", Path.GetFileName(buildingImageFile.FileName));
                        using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                        {
                            await buildingImageFile.CopyToAsync(imgfileStream);
                        }
                        compoundBuildings.BuildingPicture = Path.GetFileName(buildingImageFile.FileName);
                    }
                    compoundBuildings.dtModified = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        compoundBuildings.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
                    }
                    _context.Update(compoundBuildings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompoundBuildingsExists(compoundBuildings.IdBuilding))
                    {
                        return NotFound();
                    }
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Building. Please correct and try again.";
                }
                return RedirectToAction("Index", new
                {
                    compoundBuildings.IdCompound
                });
            }
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", compoundBuildings.IDOwner);
            return View(compoundBuildings);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            CompoundBuildings compoundBuildings = await _context.TCompoundBuildings.Include((CompoundBuildings c) => c.mCompound).Include((CompoundBuildings c) => c.mOwner).Include((CompoundBuildings c) => c.mUserCreated)
                .Include((CompoundBuildings c) => c.mUserModified)
                .SingleOrDefaultAsync((CompoundBuildings m) => (int?)m.IdBuilding == id);
            if (compoundBuildings == null)
            {
                return NotFound();
            }
            var CompoundInfo = await (from m in _context.TCompounds
                                      where m.IdCompound == compoundBuildings.IdCompound
                                      select new
                                      {
                                          m.compoundName,
                                          m.IdCompound
                                      }).SingleOrDefaultAsync();
            base.ViewData["CompoundName"] = CompoundInfo.compoundName;
            base.ViewData["IdCompound"] = CompoundInfo.IdCompound;
            base.ViewData["IDOwner"] = new SelectList(_context.TOwners, "IdOwner", "CompanyName", compoundBuildings.IDOwner);

            return View(compoundBuildings);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            CompoundBuildings compoundBuildings = await _context.TCompoundBuildings.Include(m => m.mCompoundUnits).SingleOrDefaultAsync((CompoundBuildings m) => m.IdBuilding == id);
            try
            {
                if (compoundBuildings.mCompoundUnits.Any(u => u.UnitRentContractID.HasValue))
                    throw new InvalidOperationException("This building has some units that has Contacts");
                _context.TCompoundBuildings.Remove(compoundBuildings);
                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                base.TempData["AlertSaveErr"] = ex.Message;
            }
            catch
            {
                base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
            }
            return RedirectToAction("Index", new
            {
                compoundBuildings.IdCompound
            });
        }

        private bool CompoundBuildingsExists(int id)
        {
            return _context.TCompoundBuildings.Any((CompoundBuildings e) => e.IdBuilding == id);
        }
    }
}
