using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    public class PaymentLogsController : Controller
    {
		private readonly ApplicationDbContext _context;

		public PaymentLogsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View(_context.unitRentContractAllPaymentLogs.Include(x => x.UnitRentContract).ThenInclude(x=>x.mCompound).Include(x => x.User));
		}
	}
}
