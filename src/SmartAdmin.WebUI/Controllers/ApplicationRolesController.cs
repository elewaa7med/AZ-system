using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.RolesViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApplicationRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _usr;

        private readonly RoleManager<ApplicationRole> _role;

        public ApplicationRolesController(RoleManager<ApplicationRole> role, UserManager<ApplicationUser> usr, ApplicationDbContext context)
        {
            _context = context;
            _usr = usr;
            _role = role;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationRole.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationRole applicationRole = await _context.ApplicationRole.SingleOrDefaultAsync((ApplicationRole m) => m.Id == id);
            if (applicationRole == null)
            {
                return NotFound();
            }
            return View(applicationRole);
        }

        public IActionResult Create()
        {
            EditRoleViewModel mEditRoleViewModel = new EditRoleViewModel { mRole = new ApplicationRole() };
            ViewData["Users"] = new SelectList(_context.Users, "Id", "fullName");
            return View(mEditRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditRoleViewModel applicationRole)
        {
            if (base.ModelState.IsValid)
            {
                applicationRole.mRole.dtCreated = DateTime.Now;
                applicationRole.mRole.NormalizedName = applicationRole.mRole.Name.ToUpper();
                _context.Add(applicationRole.mRole);
                await _context.SaveChangesAsync();
                foreach (string Idusr in applicationRole.Users)
                {
                    ApplicationUser usrtmp = await _usr.FindByIdAsync(Idusr);
                    if (usrtmp != null)
                        await _usr.AddToRoleAsync(usrtmp, applicationRole.mRole.Name);
                }
                return RedirectToAction("Index");
            }
            return View(applicationRole);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            ApplicationRole applicationRole = await _context.ApplicationRole.SingleOrDefaultAsync((ApplicationRole m) => m.Id == id);
            if (applicationRole == null)
                return NotFound();
            var users = _usr.GetUsersInRoleAsync(applicationRole.Name).Result;

            EditRoleViewModel mEditRoleViewModel = new EditRoleViewModel { mRole = applicationRole, Users = users.Select(u => u.Id).ToList() };
            ViewData["Users"] = new MultiSelectList(_context.Users.ToList(), "Id", "fullName");
            return View(mEditRoleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditRoleViewModel applicationRole)
        {
            if (id != applicationRole.mRole.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    applicationRole.mRole.NormalizedName = applicationRole.mRole.Name.ToUpper();
                    applicationRole.mRole.dtCreated = applicationRole.mRole.dtCreated;
                    _context.Update(applicationRole.mRole);
                    _context.UserRoles.RemoveRange(_context.UserRoles.Where(r => r.RoleId == applicationRole.mRole.Id).ToList());
                    await _context.SaveChangesAsync();

                    foreach (string Idusr in applicationRole.Users)
                    {
                        ApplicationUser usrtmp = await _usr.FindByIdAsync(Idusr);
                        if (usrtmp != null)
                            await _usr.AddToRoleAsync(usrtmp, applicationRole.mRole.Name);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationRoleExists(applicationRole.mRole.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(applicationRole);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationRole applicationRole = await _context.ApplicationRole.SingleOrDefaultAsync((ApplicationRole m) => m.Id == id);
            if (applicationRole == null)
            {
                return NotFound();
            }
            return View(applicationRole);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationRole applicationRole = await _context.ApplicationRole.SingleOrDefaultAsync((ApplicationRole m) => m.Id == id);
            _context.ApplicationRole.Remove(applicationRole);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationRoleExists(string id)
        {
            return _context.ApplicationRole.Any((ApplicationRole e) => e.Id == id);
        }
    }
}
