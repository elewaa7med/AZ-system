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
    public class PaymentLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user;
        public PaymentLogsController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }

        public IActionResult Index(string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            if (isAdmin)
            {
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }

            var result = _context.unitRentContractAllPaymentLogs.OrderBy(x => x.PaymentDate).Include(x => x.UnitRentContract).ThenInclude(x => x.mUnit).ThenInclude(x => x.mBuilding).Include(x => x.UnitRentContract).ThenInclude(x => x.mCompoundUnits).ThenInclude(x => x.mCompoundBuilding).ThenInclude(x => x.mCompound).Include(x => x.User).Include(x => x.Invoice).ToList();
            if(representitveId != null)
            {
                result = result.Where(x => x.UserID == representitveId.ToString()).ToList();
            }
            return View(result);
        }
    }
}
