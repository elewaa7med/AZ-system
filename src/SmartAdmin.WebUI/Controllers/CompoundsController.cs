using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.CompoundsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZBMS.Controllers
{
    [Authorize]
    public class CompoundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user;
        public CompoundsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }
        public async Task<IActionResult> Index()
        {
            List<Compounds> applicationDbContext = _context.TCompounds.Include((Compounds m) => m.mDistrict).ToList();
            ApplicationUser user = await _user.GetUserAsync(base.User);

            if (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant")))
            {
                applicationDbContext = (from u in applicationDbContext
                                        join uc in _context.TCompoundsUsers on u.IdCompound equals uc.IdCompound
                                        select new
                                        {
                                            Tcompounds = u,
                                            TCompoundsUsers = uc
                                        } into m
                                        where m.TCompoundsUsers.IdUser == user.Id
                                        select m into z
                                        select z.Tcompounds).ToList();
            }
            return View(applicationDbContext);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Compounds compounds = await _context.TCompounds.Include(c => c.mCompoundUsers).Include((Compounds m) => m.mDistrict).Include((Compounds m) => m.mUserCreated).Include((Compounds m) => m.mUserModified)
                .SingleOrDefaultAsync((Compounds m) => (int?)m.IdCompound == id);
            CompoundsVM compoundvm = new CompoundsVM
            {
                IdDistrict = compounds.IdDistrict,
                IdCreated = compounds.IdCreated,
                IdCompound = compounds.IdCompound,
                waterMeterAllBldNo = compounds.waterMeterAllBldNo,
                oneWaterMeterAllBld = compounds.oneWaterMeterAllBld,
                serviceWaterMeterNo = compounds.serviceWaterMeterNo,
                Notes = compounds.Notes,
                dtCreated = compounds.dtCreated,
                compoundName = compounds.compoundName,
                Users = compounds.mCompoundUsers.Select(u => u.IdUser).ToList()
            };
            if (compounds == null)
            {
                return NotFound();
            }
            base.ViewData["mOutUsers"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName", compounds.IdDistrict);
            return View(compoundvm);
        }
        public IActionResult Create()
        {
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName");
            CompoundsVM compoundvm = new CompoundsVM
            {
                mOutCompoundUsers = _context.Users.ToList(),
                mInCompoundUsers = new List<ApplicationUser>()
            };
            base.ViewData["mInUsers"] = new SelectList(compoundvm.mInCompoundUsers, "Id", "fullName");
            base.ViewData["mOutUsers"] = new SelectList(compoundvm.mOutCompoundUsers, "Id", "fullName");
            return View(compoundvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] inRole, CompoundsVM compoundvm)
        {
            if (base.ModelState.IsValid)
            {
                Compounds compound = new Compounds
                {
                    dtCreated = DateTime.Now,
                    compoundName = compoundvm.compoundName,
                    IdDistrict = compoundvm.IdDistrict,
                    waterMeterAllBldNo = compoundvm.waterMeterAllBldNo,
                    oneWaterMeterAllBld = compoundvm.oneWaterMeterAllBld,
                    serviceWaterMeterNo = compoundvm.serviceWaterMeterNo,
                    Notes = compoundvm.Notes
                };
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    compound.IdCreated = _user.GetUserId(base.HttpContext.User);
                }
                _context.Add(compound);
                await _context.SaveChangesAsync();
                _context.TCompoundsUsers.AddRange(compoundvm.Users.Select(u => new CompoundsUsers
                {
                    IdUser = u,
                    IdCompound = compound.IdCompound
                }));
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName", compoundvm.IdDistrict);
            return View(compoundvm);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Compounds compounds = await _context.TCompounds.Include(c => c.mCompoundUsers).Include((Compounds m) => m.mDistrict).Include((Compounds m) => m.mUserCreated).Include((Compounds m) => m.mUserModified)
                .SingleOrDefaultAsync((Compounds m) => (int?)m.IdCompound == id);
            CompoundsVM compoundvm = new CompoundsVM
            {
                IdDistrict = compounds.IdDistrict,
                IdCreated = compounds.IdCreated,
                IdCompound = compounds.IdCompound,
                waterMeterAllBldNo = compounds.waterMeterAllBldNo,
                oneWaterMeterAllBld = compounds.oneWaterMeterAllBld,
                serviceWaterMeterNo = compounds.serviceWaterMeterNo,
                Notes = compounds.Notes,
                dtCreated = compounds.dtCreated,
                compoundName = compounds.compoundName,
                Users = compounds.mCompoundUsers.Select(u => u.IdUser).ToList()
            };
            if (compounds == null)
            {
                return NotFound();
            }
            base.ViewData["mOutUsers"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName", compounds.IdDistrict);
            return View(compoundvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string[] outRole, string[] inRole, int id, CompoundsVM compoundvm)
        {
            if (id != compoundvm.IdCompound)
            {
                return NotFound();
            }
            Compounds compounds = new Compounds
            {
                IdCompound = id,
                compoundName = compoundvm.compoundName,
                waterMeterAllBldNo = compoundvm.waterMeterAllBldNo,
                oneWaterMeterAllBld = compoundvm.oneWaterMeterAllBld,
                serviceWaterMeterNo = compoundvm.serviceWaterMeterNo,
                Notes = compoundvm.Notes,
                IdDistrict = compoundvm.IdDistrict,
                IdCreated = compoundvm.IdCreated,
                dtCreated = compoundvm.dtCreated,
                dtModified = DateTime.Now
            };
            if (_user.GetUserName(base.HttpContext.User) != null)
                compounds.IdModified = _user.GetUserId(base.HttpContext.User);

            if (base.ModelState.IsValid)
            {
                try
                {
                    _context.Update(compounds);
                    var compoundUsers = _context.TCompoundsUsers.Where(c => c.IdCompound == compounds.IdCompound).ToList();
                    _context.TCompoundsUsers.RemoveRange(compoundUsers);
                    _context.TCompoundsUsers.AddRange(compoundvm.Users.Select(u => new CompoundsUsers
                    {
                        IdUser = u,
                        IdCompound = compounds.IdCompound
                    }));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompoundsExists(compoundvm.IdCompound))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName", compounds.IdDistrict);
            return View(compoundvm);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Compounds compounds = await _context.TCompounds.Include(c => c.mCompoundUsers).Include((Compounds m) => m.mDistrict).Include((Compounds m) => m.mUserCreated).Include((Compounds m) => m.mUserModified)
               .SingleOrDefaultAsync((Compounds m) => (int?)m.IdCompound == id);
            CompoundsVM compoundvm = new CompoundsVM
            {
                IdDistrict = compounds.IdDistrict,
                IdCreated = compounds.IdCreated,
                IdCompound = compounds.IdCompound,
                waterMeterAllBldNo = compounds.waterMeterAllBldNo,
                oneWaterMeterAllBld = compounds.oneWaterMeterAllBld,
                serviceWaterMeterNo = compounds.serviceWaterMeterNo,
                Notes = compounds.Notes,
                dtCreated = compounds.dtCreated,
                compoundName = compounds.compoundName,
                Users = compounds.mCompoundUsers.Select(u => u.IdUser).ToList()
            };
            if (compounds == null)
            {
                return NotFound();
            }
            base.ViewData["mOutUsers"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdDistrict"] = new SelectList(_context.TDistricts, "IdDistrict", "DistrictName", compounds.IdDistrict);
            return View(compoundvm);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Compounds compounds = await _context.TCompounds.SingleOrDefaultAsync((Compounds m) => m.IdCompound == id);
            try
            {
                _context.TCompounds.Remove(compounds);
                await _context.SaveChangesAsync();
            }
            catch
            {
                base.TempData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
            }
            return RedirectToAction("Index");
        }
        private bool CompoundsExists(int id)
        {
            return _context.TCompounds.Any((Compounds e) => e.IdCompound == id);
        }
    }
}
