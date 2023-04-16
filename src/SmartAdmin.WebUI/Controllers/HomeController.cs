using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Authorization;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.DTO;
using SmartAdmin.WebUI.Models.ViewModels;
using SmartAdmin.WebUI.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const string _compundViewName = "~/Views/Home/compound.cshtml";
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _user;
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> user, IAuthorizationService authorizationService)
        {
            _context = context;
            _user = user;
            _authorizationService = authorizationService;
        }
        //by auf
        public IActionResult AddToCort(int id)
        {
            var rentContractEntity = _context.TUnitRentContract.FirstOrDefault(e => e.IdRentContract == id);
            if (rentContractEntity == null)
            {
                return NotFound();
            }
            rentContractEntity.AddedtoCourt = true;
            _context.SaveChanges();
            return RedirectToAction("BuildingsDues");
        }
        public IActionResult CortContructs()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IndexAdmin()
        {
            return View();
        }
        public IActionResult IndexAdmin2()
        {
            return View();
        }
        #region Compound Dashboard
        public IActionResult MeadowPark(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 2, CompoundName = "Meadow Park Garden", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult DesertApartment(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 7, CompoundName = "Desert Apartments", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult DesertRose(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 3, CompoundName = "Desert Rose", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult DaarResident(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 4, CompoundName = "Daar Residence", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult Villa24(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 5, CompoundName = "24 Villa", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult Villa21(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 6, CompoundName = "Opal Compound", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult OasisGardens(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 9, CompoundName = "Oasis Gardens Compound", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult OasisResorts(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 10, CompoundName = "Oasis Resorts Compound", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public IActionResult Sanus(string representitveId = null)
        {
            return GetCompound(new CompoundDTO { CompoundID = 8, CompoundName = "Sanus Compound", RepresentitveId = representitveId == "null" ? null : representitveId });
        }
        public ActionResult GetNonRentedUnits(int compoundID)
        {
            return View("~/Views/Home/CompundNonRented.cshtml", GetCompoundData(compoundID));
        }
        public ActionResult ContractExpiry(int compoundID, string representitveId = null)
        {
            var compound = GetCompoundData(compoundID);
            compound.RepresentitveId = representitveId == "null" ? null : representitveId;
            if (User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                switch (compoundID)
                {
                    case 2:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.MeadowParkGarden) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 3:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.DesertRose) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 4:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.DaarResidence) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 5:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 6:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.Villa21) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    default:
                        break;
                }
            }
            return View("~/Views/Home/CompoundExpiry.cshtml", compound);
        }
        public ActionResult GetCompoundContracts(int compoundID)
        {
            return View("~/Views/Home/CompoundContracts.cshtml", GetCompoundData(compoundID));
        }
        private CompoundDTO GetCompoundData(int compoundID)
        {
            if (compoundID == 2)
                return new CompoundDTO { CompoundID = 2, CompoundName = "Meadow Park Garden" };
            else if (compoundID == 3)
                return new CompoundDTO { CompoundID = 3, CompoundName = "Desert Rose" };
            else if (compoundID == 4)
                return new CompoundDTO { CompoundID = 4, CompoundName = "Daar Residence" };
            else if (compoundID == 5)
                return new CompoundDTO { CompoundID = 5, CompoundName = "24 Villa" };
            else if (compoundID == 6)
                return new CompoundDTO { CompoundID = 6, CompoundName = "Opal Compound" };
            else if (compoundID == 7)
                return new CompoundDTO { CompoundID = 7, CompoundName = "Desert Apartments" };
            else if (compoundID == 8)
                return new CompoundDTO { CompoundID = 8, CompoundName = "Sanus Compound" };
            else if (compoundID == 9)
                return new CompoundDTO { CompoundID = 9, CompoundName = "Oasis Gardens Compound" };
            else if (compoundID == 10)
                return new CompoundDTO { CompoundID = 10, CompoundName = "Oasis Resorts Compound" };
            return null;
        }
        #endregion

        public IActionResult BuildingsDues(int id, string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            ViewBag.id = id;
            if (User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.BuildingAccountant) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);

            }
            return View(model: representitveId);
        }

        public IActionResult NonRentUnits(int id, string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            ViewBag.id = id;
            if (User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.BuildingAccountant) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
            }
            ViewBag.id = id;
            return View(model: representitveId);
        }

        public IActionResult About()
        {
            base.ViewData["Message"] = "Your application description page.";
            return View();
        }
        public IActionResult Contact()
        {
            base.ViewData["Message"] = "Your contact page.";
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = (Activity.Current?.Id ?? base.HttpContext.TraceIdentifier)
            });
        }
        public async Task<JsonResult> getStats()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getStats2";
                command.Parameters.Add(new SqlParameter("IdUser", _user.GetUserId(base.HttpContext.User)));
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows[0]);
        }
        public async Task<JsonResult> getContractExpiry()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getContractExpiry";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getCompoundStats()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getAdminStats";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getBuildingsStats()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getAdminStatsBld";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getCommissions()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getCommissions";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getDueRents()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getDueRents";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getBuildingsDues()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getBuildingsDues";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getCompoundsDues()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getCompoundsDues";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getCompoundsDues60Days()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getCompoundsDues60Days";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getUnitsGrandTotals()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getUnitsGrandTotals";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getDuesPerCompound60days(int idCompound)
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdCompound", SqlDbType.Int);
                IdCompoundParameter.Value = idCompound;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getDuesPerCompound60days";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getContractExpiryPerCompound(int idCompound, string representitveId = null)
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdCompound", SqlDbType.Int);
                IdCompoundParameter.Value = idCompound;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getContractExpiryPerCompound";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var filteredRows = !isAdmin ? (from DataRow myRow in tbl.Rows
                                           where myRow[14].ToString() == _user.GetUserId(User) && ((bool)myRow[19] == false)
                                           select myRow).ToList() :
                                         (from DataRow myRow in tbl.Rows select myRow).ToList();
            if (!string.IsNullOrEmpty(representitveId))
                filteredRows = (from DataRow myRow in tbl.Rows
                                where myRow[14].ToString() == representitveId
                                select myRow).ToList();
            //where addtocourt == 0
            filteredRows = filteredRows.Where(e => ((bool)e[19] == false)).Select(e => e).ToList();
            return Json(filteredRows);
        }
        public async Task setLastPayment(int IdContract)
        {
            new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdContract", SqlDbType.Int);
                IdCompoundParameter.Value = IdContract;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "setLastPayment";
                _context.Database.OpenConnection();
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task setundoLastPayment(int IdContract)
        {
            new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdContract", SqlDbType.Int);
                IdCompoundParameter.Value = IdContract;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "setundoLastPayment";
                _context.Database.OpenConnection();
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task<JsonResult> getBuildingsOverDues()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getBuildingsOverDues";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getContractExpiryofBuildings(int id, string representitveId = null)
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getContractExpiryofBuildings";

                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var filteredRows = !isAdmin ? (from DataRow myRow in tbl.Rows
                                           where myRow[13].ToString() == _user.GetUserId(User) && ((int)myRow[8]) == id
                                           select myRow).ToList() :
                                         (from DataRow myRow in tbl.Rows where ((int)myRow[8]) == id select myRow).ToList();
            var value = filteredRows.Where(r => (int)r.ItemArray[8] == id).FirstOrDefault();
            var value2 = filteredRows.Where(r => r.ItemArray[13].ToString() == representitveId).FirstOrDefault();
            if (!string.IsNullOrEmpty(representitveId))
                filteredRows = filteredRows.Where(r => r.ItemArray[13].ToString() == representitveId && (bool)r.ItemArray[17] == false && (int)r.ItemArray[8] == id).ToList();
            return Json(filteredRows);
        }
        public async Task<JsonResult> getOverDuesPerCompound(int idCompound)
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdCompound", SqlDbType.Int);
                IdCompoundParameter.Value = idCompound;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getOverDuesPerCompound";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getBuildingsRented()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getBuildingsRented";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getRentedPerCompound(int idCompound)
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdCompound", SqlDbType.Int);
                IdCompoundParameter.Value = idCompound;
                command.Parameters.Add(IdCompoundParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getRentedPerCompound";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public async Task<JsonResult> getBuildingsDues60days()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getBuildingsDues60days";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
        public ActionResult Migrate()
        {
            foreach (var unitRentContract in _context.TUnitRentContract.Where(c => !c.Archived).Include(t => t.mUnit).Include(t => t.mCompoundUnits).ToList())
            {
                if (unitRentContract.mCompoundUnits != null)
                {
                    unitRentContract.mCompoundUnits.isRented = true;
                    unitRentContract.mCompoundUnits.UnitRentContractID = unitRentContract.IdRentContract;
                }
                if (unitRentContract.mUnit != null)
                {
                    unitRentContract.mUnit.isRented = true;
                    unitRentContract.mUnit.UnitRentContractID = unitRentContract.IdRentContract;
                }
                _context.SaveChanges();
            }
            return Json(new { val = true });
        }
        private int PaidAmount(int rent, int paid)
        {
            if (rent <= paid)
                return rent;
            if (paid > 0)
                return paid;
            else return 0;
        }
        private void ApplyPayments(UnitRentContract unitRentContract, int rent, int paymentsCount, int incrmentStep)
        {
            _context.TUnitRentContractPayments.RemoveRange(_context.TUnitRentContractPayments.Where(c => c.UnitRentContractID == unitRentContract.IdRentContract).ToList());
            var paidAmount = unitRentContract.paidAmount;
            var month = 0;
            for (int i = 0; i < paymentsCount; i++)
            {
                var coverAllRent = rent <= paidAmount;
                var rentValue = PaidAmount(rent, paidAmount);
                unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                {
                    Amount = rent,
                    DueDate = unitRentContract.dtLeaseStart.AddMonths(month),
                    PaidAmount = rentValue,
                    Paid = coverAllRent
                });
                paidAmount -= rentValue;
                month += incrmentStep;
            }
        }
        public JsonResult GetCompoundsDue(int compoundID, string representitveId = null)
        {
            var currentTime = DateTime.Now;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var next30DaysTime = currentTime.AddMonths(2);
            var pariallyPaidIds = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                         .Where(c => !c.Archived && c.remainingAmount > 0 && c.IdCompound == compoundID && c.AddedtoCourt == false &&
                                                                     c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime))
                                                         .Select(p => p.IdRentContract).ToList();
            var due60 = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Include(u => u.mCompoundUnits.mPropertyType)
                                                   .Where(c => c.dtLeaseEnd > currentTime)
                                                  .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                               !c.Archived && c.remainingAmount > 0 && c.IdCompound == compoundID &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddMonths(-2) <= currentTime &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days >= -60 &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddMonths(2) > currentTime &&
                                                               !pariallyPaidIds.Contains(c.IdRentContract) &&
                                                               (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractID = t.IdRentContract,
                                                      UnitNumber = t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate.AddMonths(-2) <= currentTime && p.DueDate.AddMonths(2) > currentTime).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                      NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString(),
                                                      Mandoob = t.Mandoob.fullName,
                                                      MandoobID = t.IdMandoob,
                                                      PropertyType = t.mCompoundUnits.mPropertyType.PropertyTypeName
                                                  }).OrderBy(t => t.TotalDays).ToList();
            return Json(new DueViewModel
            {
                Due = due60,
                TotalValue = due60.Sum(d => d.Value)
            });
        }
        public JsonResult GetCompoundsDueOver60(int compoundID, string representitveId = null)
        {
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var pariallyPaidIds = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                         .Where(c => !c.Archived && c.remainingAmount > 0 && c.IdCompound == compoundID && c.AddedtoCourt == false &&
                                                                      c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime))
                                                         .Select(p => p.IdRentContract).ToList();
            var dues = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Include(u => u.mCompoundUnits.mPropertyType)
                                                   .Where(c => c.dtLeaseEnd > currentTime)
                                                  .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                               !c.Archived && c.remainingAmount > 0 && c.IdCompound == compoundID &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days < -29 &&
                                                               !pariallyPaidIds.Contains(c.IdRentContract) &&
                                                               (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractID = t.IdRentContract,
                                                      UnitNumber = t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString(),
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                      Mandoob = t.Mandoob.fullName,
                                                      PropertyType = t.mCompoundUnits.mPropertyType.PropertyTypeName
                                                  }).OrderBy(t => t.TotalDays).ToList();
            return Json(new DueViewModel
            {
                Due = dues,
                TotalValue = dues.Sum(d => d.Value)
            });
        }
        public ActionResult GetComoundNonRentedUnits(int compoundID)
        {
            var nonRentedUnits = _context.TCompoundUnits.Where(u => !u.UnitRentContractID.HasValue && u.mCompoundBuilding.IdCompound == compoundID).Include(u => u.mPropertyType).Select(r => new CompoundUnitDTO
            {
                ID = r.IdUnit,
                Unit = r.UnitNumber,
                AnnualRent = r.RentRate,
                Floor = r.mFloor.PropertyFloorName,
                NumberOfRooms = r.NUmberofRooms,
                NumberOfMajlis = r.NumberofMajlis,
                NoOfBaths = r.NoOfBaths,
                UnitArea = r.UnitArea ?? 0,
                Building = r.mCompoundBuilding.BuildingNumber ?? string.Empty,
                PropertyType = r.mPropertyType.PropertyTypeName
            }).ToList();
            return PartialView(nonRentedUnits);
        }
        public JsonResult GetBuildingsDue(int id, string representitveId = null)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var currentTime = DateTime.Now;
            if (id == 4)
            {
                var next7DaysTime = currentTime.AddDays(7);
                var due60PartiallyPaid = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                  c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next7DaysTime))
                                                      .Select(s => s.IdRentContract).ToList();

                var due60 = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => c.dtLeaseEnd > currentTime)
                                                      .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                                   !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddDays(-7) <= currentTime &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days >= -7 &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddDays(7) > currentTime &&
                                                                   !due60PartiallyPaid.Contains(c.IdRentContract) &&
                                                                   (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                      .Select(t => new DueValue
                                                      {
                                                          ContractID = t.IdRentContract,
                                                          UnitNumber = t.mUnit.UnitNumber ?? string.Empty,
                                                          TenantName = t.mTenant.tenantName,
                                                          Mobile = t.mTenant.tenantMobile,
                                                          AnnualRent = t.yearlyRent,
                                                          RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                          Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate.AddDays(-7) <= currentTime && p.DueDate.AddDays(7) > currentTime).Sum(p => p.RemainingAmount),
                                                          CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                          ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                          TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                          Building = t.mUnit.mBuilding.BuildingName,
                                                          Mandoob = t.Mandoob.fullName,
                                                          Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                          NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                          NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString()
                                                      }).OrderBy(t => t.TotalDays).ToList();
                return Json(new DueViewModel
                {
                    Due = due60,
                    TotalValue = due60.Sum(d => d.Value),

                });
            }
            else
            {
                var next30DaysTime = currentTime.AddMonths(2);
                var due60PartiallyPaid = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                  c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime))
                                                      .Select(s => s.IdRentContract).ToList();

                var due60 = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => c.dtLeaseEnd > currentTime)
                                                      .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                                   !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddMonths(-2) <= currentTime &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days >= -60 &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).AddMonths(2) > currentTime &&
                                                                   !due60PartiallyPaid.Contains(c.IdRentContract) &&
                                                                   (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                      .Select(t => new DueValue
                                                      {
                                                          ContractID = t.IdRentContract,
                                                          UnitNumber = t.mUnit.UnitNumber ?? string.Empty,
                                                          TenantName = t.mTenant.tenantName,
                                                          Mobile = t.mTenant.tenantMobile,
                                                          AnnualRent = t.yearlyRent,
                                                          RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                          Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate.AddMonths(-2) <= currentTime && p.DueDate.AddMonths(2) > currentTime).Sum(p => p.RemainingAmount),
                                                          CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                          ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                          TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                          Building = t.mUnit.mBuilding.BuildingName,
                                                          Mandoob = t.Mandoob.fullName,
                                                          Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                          NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                          NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString()

                                                      }).OrderBy(t => t.TotalDays).ToList();
                return Json(new DueViewModel
                {
                    Due = due60,
                    TotalValue = due60.Sum(d => d.Value),

                });
            }

        }
        public JsonResult GetBuildingsDueOver60(int id, string representitveId = null)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var currentTime = DateTime.Now;
            if (id == 4)
            {
                var next30DaysTime = currentTime.AddDays(7);
                var due60PartiallyPaid = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                  c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime))
                                                      .Select(s => s.IdRentContract).ToList();
                var dues = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => c.dtLeaseEnd > currentTime)
                                                      .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                                  !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days <= -4 &&
                                                                   !due60PartiallyPaid.Contains(c.IdRentContract) &&
                                                                    !c.AddedtoCourt &&
                                                                   (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                      .Select(t => new DueValue
                                                      {
                                                          ContractID = t.IdRentContract,
                                                          UnitNumber = t.mUnit.UnitNumber ?? string.Empty,
                                                          TenantName = t.mTenant.tenantName,
                                                          Mobile = t.mTenant.tenantMobile,
                                                          AnnualRent = t.yearlyRent,
                                                          RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                          Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Sum(p => p.RemainingAmount),
                                                          CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                          ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                          TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                          Building = t.mUnit.mBuilding.BuildingName,
                                                          Mandoob = t.Mandoob.fullName,
                                                          Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                          NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                          NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString()
                                                      }).OrderBy(t => t.TotalDays).ToList();
                return Json(new DueViewModel
                {
                    Due = dues,
                    TotalValue = dues.Sum(d => d.Value)
                });
            }
            else
            {
                var next30DaysTime = currentTime.AddMonths(1);
                var due60PartiallyPaid = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                  c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime))
                                                      .Select(s => s.IdRentContract).ToList();
                var dues = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                      .Include(t => t.mTenant)
                                                      .Where(c => c.dtLeaseEnd > currentTime)
                                                      .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                                  !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                                   c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days < -30 &&
                                                                   !due60PartiallyPaid.Contains(c.IdRentContract) &&
                                                                    !c.AddedtoCourt &&
                                                                   (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                      .Select(t => new DueValue
                                                      {
                                                          ContractID = t.IdRentContract,
                                                          UnitNumber = t.mUnit.UnitNumber ?? string.Empty,
                                                          TenantName = t.mTenant.tenantName,
                                                          Mobile = t.mTenant.tenantMobile,
                                                          AnnualRent = t.yearlyRent,
                                                          RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                          Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Sum(p => p.RemainingAmount),
                                                          CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                          ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                          TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                          Building = t.mUnit.mBuilding.BuildingName,
                                                          Mandoob = t.Mandoob.fullName,
                                                          Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                          NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                          NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString()
                                                      }).OrderBy(t => t.TotalDays).ToList();
                return Json(new DueViewModel
                {
                    Due = dues,
                    TotalValue = dues.Sum(d => d.Value)
                });
            }
        }
        public ActionResult GetBuildingNonRentedUnits(int id)
        {
            ViewBag.id = id;
            var nonRentedUnits = _context.Units.Where(u => !u.UnitRentContractID.HasValue && u.idMasterBuilding == id).Select(r => new UnitDTO
            {
                ID = r.IdUnit,
                Unit = r.UnitNumber,
                AnnualRent = r.RentRate,
                Floor = r.mFloor.PropertyFloorName,
                NumberOfRooms = r.NUmberofRooms,
                NumberOfMajlis = r.NumberofMajlis,
                NoOfBaths = r.NoOfBaths,
                UnitArea = r.UnitArea ?? 0,
                District = r.mBuilding.mDistrict.DistrictName,
                City = r.mBuilding.mCity.CityName,
                Building = r.mBuilding.BuildingName
            }).ToList();
            return PartialView(nonRentedUnits);
        }

        public JsonResult GetCompoundsPatrtialPaymentDue(int compoundID, string representitveId = null)
        {
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var due60 = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Include(t => t.mCompoundUnits.mPropertyType)
                                                   .Where(c => c.dtLeaseEnd > currentTime)
                                                  .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                              !c.Archived && c.remainingAmount > 0 && c.IdCompound == compoundID &&
                                                               c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime) &&
                                                               (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractID = t.IdRentContract,
                                                      UnitNumber = t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime && p.PaidAmount >= 0).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                      NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString(),

                                                      Mandoob = t.Mandoob.fullName,
                                                      PropertyType = t.mCompoundUnits.mPropertyType.PropertyTypeName

                                                  }).OrderBy(t => t.TotalDays).ToList();
            return Json(new DueViewModel
            {
                Due = due60,
                TotalValue = due60.Sum(d => d.Value)
            });
        }
        public JsonResult GetBuildingssPatrtialPaymentDue(int id, string representitveId = null)
        {
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var due60 = _context.TUnitRentContract.Where(x => x.mMasterBuilding == id).Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c => c.dtLeaseEnd > currentTime)
                                                  .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false &&
                                                              !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                              c.UnitRentContractPayments.Any(p => !p.Paid && p.PaidAmount > 0 && p.DueDate < next30DaysTime) &&
                                                               c.AddedtoCourt == false &&
                                                               (string.IsNullOrEmpty(representitveId) ? true : c.IdMandoob == representitveId))
                                                  .Select(t => new DueValue
                                                  {
                                                      Building = t.mUnit.mBuilding.BuildingName,
                                                      ContractID = t.IdRentContract,
                                                      UnitNumber = t.mUnit.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime && p.PaidAmount >= 0).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteDate = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString() == "01/01/0001" ? string.Empty : t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.CreatedOn).FirstOrDefault().ToShortDateString(),
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault(),
                                                      Mandoob = t.Mandoob.fullName

                                                  }).OrderBy(t => t.TotalDays).ToList();
            return Json(new DueViewModel
            {
                Due = due60,
                TotalValue = due60.Sum(d => d.Value)
            });
        }
        public void MigrateMandoobs()
        {
            foreach (var contract in _context.TUnitRentContract)
                contract.IdMandoob = contract.IdCreated;
            _context.SaveChanges();
        }
        public ActionResult GetNote(int noteID)
        {
            var note = _context.TUnitRentContractNotes.Where(n => n.ID == noteID).Select(n => new NoteDTO { Note = n.Note, Mandoob = n.User.fullName, Created = n.CreatedOn.ToShortDateString() }).FirstOrDefault();
            return PartialView(note);
        }
        public async Task<JsonResult> CreateManoobRole()
        {
            var mandoobRole = new ApplicationRole()
            {
                Name = "Mandoob",
                NormalizedName = "Mandoob",
                Description = "Mandoob",
                dtCreated = DateTime.Now
            };
            _context.Add(mandoobRole);
            _context.Add(new ApplicationRole()
            {
                Name = "HR",
                NormalizedName = "HR",
                Description = "Human Resources",
                dtCreated = DateTime.Now
            });
            _context.SaveChanges();
            var users = await _user.GetUsersInRoleAsync("Buildings-Role");
            foreach (var user in users)
            {
                await _user.AddToRoleAsync(user, "Mandoob");
                await _user.RemoveFromRoleAsync(user, "Buildings-Role");
            }
            users = _user.GetUsersInRoleAsync("Compounds-Role").Result;
            foreach (var user in users)
            {
                await _user.AddToRoleAsync(user, "Mandoob");
                await _user.RemoveFromRoleAsync(user, "Compounds-Role");
            }
            _context.ApplicationRole.Remove(_context.ApplicationRole.FirstOrDefault(r => r.Name == "Buildings-Role"));
            _context.ApplicationRole.Remove(_context.ApplicationRole.FirstOrDefault(r => r.Name == "Compounds-Role"));
            _context.SaveChanges();
            return null;
        }
        private ActionResult GetCompound(CompoundDTO compound)
        {
            if (User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                switch (compound.CompoundID)
                {
                    case 2:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.MeadowParkGarden) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 3:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.DesertRose) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 4:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.DaarResidence) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 5:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 6:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.Villa21) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 7:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.DesertApartments) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    case 8:
                        ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.SanusCompound) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(compound.RepresentitveId) ? null : compound.RepresentitveId);
                        break;
                    default:
                        break;
                }
            }
            return View(_compundViewName, compound);
        }
        public ActionResult UserHome()
        {
            return View();
        }
        public IActionResult GetUnitInfoByContractId(int contractId)
        {
            var unitInfo = _context.TUnitRentContract
                 .Include(e => e.mTenant)
                 .Include(e => e.mCompound)
                 .Include(e => e.mCompoundUnits)
                 .Include(e => e.mUnit)
                 .Include(e => e.mUnit.mBuilding)
                 .Where(e => e.IdRentContract == contractId)
                 .Select(p => new UnitInfoViewModel
                 {
                     IdRentContract = p.IdRentContract,
                     TenantName = p.mTenant.tenantName,

                     UnitNumber = p.IdUnit != null
                                    ? p.mUnit.UnitNumber
                                    : p.mCompoundUnits.UnitNumber,

                     BuldingNumber = p.IdUnit != null
                                    ? p.mUnit.mBuilding.BuildingName
                                    : p.mCompound.compoundName,

                 }).FirstOrDefault();

            return PartialView("_UnitInfo", unitInfo);
            // return Json(new { data = unitInfo });
        }
        public ActionResult MigrateElectricity()
        {
            foreach (var unit in _context.Units)
                _context.ElectricityMeters.Add(new ElectricityMeter
                {
                    UnitID = unit.IdUnit
                });
            foreach (var unit in _context.TCompoundUnits)
                _context.ElectricityMeters.Add(new ElectricityMeter
                {
                    CompoundUnitID = unit.IdUnit
                });
            _context.SaveChanges();
            return null;
        }
        public ActionResult GetBuildingDashboard()
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");

            var firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var currentYear = firstDayInMonth.AddMonths(11);
            var values = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                          .Where(c => (isAdmin || c.UnitRentContract.IdMandoob == _user.GetUserId(User)) &&
                                                                      !c.UnitRentContract.AddedtoCourt && c.DueDate >= firstDayInMonth && c.DueDate <= currentYear &&
                                                                      !c.UnitRentContract.Archived && !c.UnitRentContract.IdCompound.HasValue &&
                                                                      c.UnitRentContract.dtLeaseEnd >= DateTime.Now)
                                                          .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                          .Select(m => new
                                                          {
                                                              Seconds = new DateTime(m.Key.Year, m.Key.Month, 1),
                                                              Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                          }).ToList();
            return Json(values);
        }
        public ActionResult GetCompoundsDashboard(int compoundID)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");

            var firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var currentYear = firstDayInMonth.AddMonths(11);
            var values = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                           .Where(c => (isAdmin || c.UnitRentContract.IdMandoob == _user.GetUserId(User)) &&
                                                                       !c.UnitRentContract.AddedtoCourt && c.DueDate >= firstDayInMonth && c.DueDate <= currentYear &&
                                                                       c.UnitRentContract.IdCompound == compoundID && !c.UnitRentContract.Archived &&
                                                                       c.UnitRentContract.dtLeaseEnd >= DateTime.Now)
                                                           .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                           .Select(m => new
                                                           {
                                                               Seconds = new DateTime(m.Key.Year, m.Key.Month, 1),
                                                               Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                           }).ToList();
            return Json(values);
        }
        public ActionResult Dashboard()
        {
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 00, 00, 00);
            var endDayOfMont = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month), 23, 59, 59);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager");
            var isAccountant = User.IsInRole("Accountant");
            var isMandoob = User.IsInRole("Mandoob");
            var userID = _user.GetUserId(User);
            string compoundName;

            var groups = _context.TPropertyTypes.Select(p => p.PropertyTypeName)
                                                .ToList()
                                                .Select(e => GetName(e))
                                                .Distinct()
                                                .SelectMany(x => Enum.GetValues(typeof(Enums.UnitOwner)).Cast<Enums.UnitOwner>()
                                                .ToList(), (x, y) => new
                                                {
                                                    Name = x,
                                                    Owner = y
                                                }).OrderBy(e => e.Owner.GetDisplayOrder()).ThenByDescending(e => e.Name).ToList();

            var groupsWithOneProperty = _context.TPropertyTypes.Where(p => p.IdPropertyType == 1
            || p.IdPropertyType == 6 || p.IdPropertyType == 7 || p.IdPropertyType == 8
            ).Select(p => p.PropertyTypeName)
                                                .ToList();


            var paymentLogs = _context.UnitRentContractPaymentLogs.Include(r => r.UnitRentContractPayment)
                                                                  .ThenInclude(c => c.UnitRentContract)
                                                                  .Where(c => (isAdmin || isAccountant || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                              c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                              !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived);
            var paymentLogsPerDay = _context.UnitRentContractPaymentLogs.Include(r => r.UnitRentContractPayment)
                                                                  .ThenInclude(c => c.UnitRentContract)
                                                                  .Where(c => (isAdmin || isAccountant || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                              c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month && c.PaymentDate.ToShortDateString() == currentDate.ToShortDateString() &&
                                                                              !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived);

            var units = _context.TCompoundUnits.Include(c => c.mCompoundBuilding)
                                               .Include(c => c.mPropertyType)
                                               .Select(e => new
                                               {
                                                   e.UnitRentContractID,
                                                   e.UnitOwner,
                                                   e.mCompoundBuilding.IdCompound,
                                                   e.mPropertyType.PropertyTypeName
                                               }).ToList();

            var firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var currentYear = firstDayInMonth.AddMonths(11);
            var values = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                           .Where(c => (isAdmin || c.UnitRentContract.IdMandoob == _user.GetUserId(User)) &&
                                                                       !c.UnitRentContract.AddedtoCourt && c.DueDate >= firstDayInMonth && c.DueDate <= currentYear &&
                                                                       !c.UnitRentContract.Archived &&
                                                                       c.UnitRentContract.dtLeaseEnd >= DateTime.Now);
            var monthsList = Enumerable.Range(currentDate.Month, 12)
                                        .Select(e => new
                                        {
                                            Month = e > 12 ? e - 12 : e,
                                            Name = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(e > 12 ? e - 12 : e)
                                        }).ToList();
            var totalIncoms = monthsList.Select(e => 0M).ToList();
            ExpectedIncome expectedIncome;
            var model = new DashboardViewModel { MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(currentDate.Month) };
            model.MonthsNames = monthsList.Select(m => m.Name).ToList();
            model.date = currentDate.ToShortDateString();
            model.day = CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(currentDate.DayOfWeek);

            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.DaarResidenceDashboard))
            {
                compoundName = "Daar Residence";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-warning-400",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 4).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 4).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=4"
                });
                var group = units.Where(u => u.IdCompound == 4).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 4 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 4 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 4)
                                               .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                               .Select(m => new
                                               {
                                                   Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                   Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                               }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.DesertRoseDashboard))
            {
                compoundName = "Desert Rose";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-success-200",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 3).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 3).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=3"
                });
                var group = units.Where(u => u.IdCompound == 3).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 3 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 3 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 3)
                                               .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                               .Select(m => new
                                               {
                                                   Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                   Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                               }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.MeadowParkGardenDashboard))
            {
                compoundName = "Meadow Park Garden";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 2).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 2).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=2"
                });
                var group = units.Where(u => u.IdCompound == 2).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 2 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 2 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 2)
                                              .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                              .Select(m => new
                                              {
                                                  Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                  Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                              }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.Villa24Dashboard))
            {
                compoundName = "24 Villa";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-warning-400",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 5).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 5).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=5"
                });
                var group = units.Where(u => u.IdCompound == 5).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 5 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 5 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = "villa24"
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 5)
                                              .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                              .Select(m => new
                                              {
                                                  Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                  Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                              }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.Villa21Dashboard))
            {
                compoundName = "Opal Compound";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-info-200",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 6).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 6).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=6"
                });
                var group = units.Where(u => u.IdCompound == 6).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 6 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 6 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 6)
                                              .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                              .Select(m => new
                                              {
                                                  Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                  Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                              }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.DesertApartmentsDashboard))
            {
                compoundName = "Desert Apartments";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 7).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 7).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=7"
                });
                var group = units.Where(u => u.IdCompound == 7).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 7 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 7 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 7)
                                              .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                              .Select(m => new
                                              {
                                                  Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                  Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                              }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.SanusCompoundDashboard))
            {
                compoundName = "Sanus Compound";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-200",
                    Revenu = paymentLogs.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 8).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => c.UnitRentContractPayment.UnitRentContract.IdCompound.Value == 8).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?compoundID=8"
                });
                var group = units.Where(u => u.IdCompound == 8).GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                });
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = units.Count(u => u.IdCompound == 8 && u.UnitRentContractID.HasValue),
                    Vacant = units.Count(u => u.IdCompound == 8 && !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup = values.Where(c => c.UnitRentContract.IdCompound == 8)
                                              .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                              .Select(m => new
                                              {
                                                  Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                  Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                              }).ToList();
                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
            }
            if (isAdmin || (isAccountant || isMandoob) && HasAccess(Permission.BuildingDashboard))
            {
                #region occupiedRegion
                model.occupancyRegions = _context.Units.Include(x => x.mBuilding).ThenInclude(x => x.mDistrict)
                    .GroupBy(x => x.mBuilding.IdDistrict).Select(x => new OccupancyRegion
                    {
                        Region = x.Select(z => z.mBuilding.mDistrict.DistrictName).FirstOrDefault(),
                        AppartmentNumber = x.Where(z => z.isDeleted == false).Count(),
                        occupiedNumber = x.Where(z => z.isRented == true && z.isDeleted == false).Count(),
                        NonOccupiedNumber = x.Where(z => (z.isRented == false || z.isRented == null) && z.isDeleted == false).Count()
                    }).ToList();
                #endregion

                #region buildings
                compoundName = "Buildings";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 1).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 1).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?pageid=1"
                });

                var group1 = _context.Units.Where(x => x.idMasterBuilding == 1).Include(u => u.mPropertyType).Select(e => new
                {
                    e.UnitRentContractID,
                    e.IdBuilding,
                    e.UnitOwner,
                    e.mPropertyType.PropertyTypeName
                }).AsEnumerable().GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                }).ToList();
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groups.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group1.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group1.Where(a => a.Key.PropertyTypeName == e.Name && a.Key.UnitOwner == e.Owner).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e.Name } {GetDisplayName(e.Owner.GetDisplayName())}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = _context.Units.Where(x => x.idMasterBuilding == 1).Count(u => u.UnitRentContractID.HasValue),
                    Vacant = _context.Units.Where(x => x.idMasterBuilding == 1).Count(u => !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup1 = values.Where(c => !c.UnitRentContract.IdCompound.HasValue && c.UnitRentContract.mMasterBuilding == 1)
                                                .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                .Select(m => new
                                                {
                                                    Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                    Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                }).ToList();

                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup1.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
                #endregion
                #region srooh
                compoundName = "Buildings Srooh";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 2).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 2).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?pageid=2"
                });

                var group2 = _context.Units.Where(x => x.idMasterBuilding == 2).Include(u => u.mPropertyType).Select(e => new
                {
                    e.UnitRentContractID,
                    e.IdBuilding,
                    e.UnitOwner,
                    e.mPropertyType.PropertyTypeName
                }).AsEnumerable().GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                }).ToList();

                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groupsWithOneProperty.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group2.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group2.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e}"
                    }).ToList()
                });

                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = _context.Units.Where(x => x.idMasterBuilding == 2).Count(u => u.UnitRentContractID.HasValue),
                    Vacant = _context.Units.Where(x => x.idMasterBuilding == 2).Count(u => !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup2 = values.Where(c => !c.UnitRentContract.IdCompound.HasValue && c.UnitRentContract.mMasterBuilding == 2)
                                                .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                .Select(m => new
                                                {
                                                    Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                    Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                }).ToList();

                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup2.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
                #endregion
                #region hyam
                compoundName = "Buildings Hyam Alrasheed";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 3).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 3).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?pageid=3"
                });
                var group3 = _context.Units.Where(x => x.idMasterBuilding == 3).Include(u => u.mPropertyType).Select(e => new
                {
                    e.UnitRentContractID,
                    e.IdBuilding,
                    e.UnitOwner,
                    e.mPropertyType.PropertyTypeName
                }).AsEnumerable().GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                }).ToList();
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groupsWithOneProperty.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group3.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group3.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = _context.Units.Where(x => x.idMasterBuilding == 3).Count(u => u.UnitRentContractID.HasValue),
                    Vacant = _context.Units.Where(x => x.idMasterBuilding == 3).Count(u => !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup3 = values.Where(c => !c.UnitRentContract.IdCompound.HasValue && c.UnitRentContract.mMasterBuilding == 3)
                                                .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                .Select(m => new
                                                {
                                                    Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                    Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                }).ToList();

                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup3.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
                #endregion
                #region UK
                compoundName = "London Properties";
                model.Items.Add(new DashboardListItem
                {
                    Name = compoundName,
                    BGColor = "bg-primary-300",
                    Revenu = paymentLogs.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 4).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    RevenuOFDay = paymentLogsPerDay.Where(c => !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue && c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == 4).Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum(),
                    ActionURL = "/Home/GetPaymentDetails?pageid=4"
                });
                var group4 = _context.Units.Where(x => x.idMasterBuilding == 4).Include(u => u.mPropertyType).Select(e => new
                {
                    e.UnitRentContractID,
                    e.IdBuilding,
                    e.UnitOwner,
                    e.mPropertyType.PropertyTypeName
                }).AsEnumerable().GroupBy(g => new
                {
                    PropertyTypeName = GetName(g.PropertyTypeName),
                    g.UnitOwner
                }).ToList();
                model.UnitsSummary.Add(new CompoundUnitsSummary
                {
                    CompoundName = compoundName,
                    Items = groupsWithOneProperty.Select(e => new CompoundUnitsSummaryDetail
                    {
                        Occupied = group4.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => s.UnitRentContractID.HasValue) ?? 0,
                        Vacant = group4.Where(a => a.Key.PropertyTypeName == e).FirstOrDefault()?.Count(s => !s.UnitRentContractID.HasValue) ?? 0,
                        Type = $"{e}"
                    }).ToList()
                });
                model.UnitsPieChartSummary.Add(new UnitsPieChartSummary
                {
                    Occupied = _context.Units.Where(x => x.idMasterBuilding == 4).Count(u => u.UnitRentContractID.HasValue),
                    Vacant = _context.Units.Where(x => x.idMasterBuilding == 4).Count(u => !u.UnitRentContractID.HasValue),
                    PieChartName = compoundName,
                    PieChartID = compoundName.Replace(" ", string.Empty).ToLower()
                });
                var expectedIncomeGroup4 = values.Where(c => !c.UnitRentContract.IdCompound.HasValue && c.UnitRentContract.mMasterBuilding == 4)
                                                .GroupBy(d => new { d.DueDate.Month, d.DueDate.Year })
                                                .Select(m => new
                                                {
                                                    Month = new DateTime(m.Key.Year, m.Key.Month, 1).Month,
                                                    Value = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum()
                                                }).ToList();

                expectedIncome = new ExpectedIncome { CompoundName = compoundName };
                for (int i = 0; i < monthsList.Count; i++)
                {
                    var e = monthsList[i];
                    var total = expectedIncomeGroup4.FirstOrDefault(a => a.Month == e.Month)?.Value ?? 0;
                    expectedIncome.Items.Add(new ExpectedIncomeItem
                    {
                        MonthName = e.Name,
                        Income = total
                    });
                    totalIncoms[i] += total;
                }
                model.ExpectedIncomes.Add(expectedIncome);
                #endregion
            }
            var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Where(u => isAdmin || u.Id == userID).Select(u => u.Id);
            var representatives = new SelectList(_context.Users.Where(m => mandoobUsersIDs.Contains(m.Id)), "Id", "fullName");
            model.RepresentativeTable = new RepresentativeTable
            {
                Items = representatives.Select(r => new RepresentativeTableItem
                {
                    Name = r.Text,
                    RenewedContracts = _context.TUnitRentContract.Count(c => c.Renewed && c.IdMandoob == r.Value && c.dtCreated.Month == currentDate.Month && c.dtCreated.Year == currentDate.Year),
                    NewContracts = _context.TUnitRentContract.Count(c => !c.Renewed && c.IdMandoob == r.Value && c.dtCreated.Month == currentDate.Month && c.dtCreated.Year == currentDate.Year),
                    CollectedAmount = _context.UnitRentContractPaymentLogs.Where(p => p.PaymentDate.Month == currentDate.Month && p.PaymentDate.Year == currentDate.Year && p.UnitRentContractPayment.UnitRentContract.IdMandoob == r.Value && !p.UnitRentContractPayment.UnitRentContract.Archived && !p.UnitRentContractPayment.UnitRentContract.AddedtoCourt).Sum(s => s.PaidAmount),
                    DueAmount = _context.TUnitRentContractPayments.Where(u => u.UnitRentContract.IdMandoob == r.Value && u.DueDate <= endDayOfMont && !u.UnitRentContract.Archived && !u.UnitRentContract.AddedtoCourt).Select(s => s.DueDate < startOfMonth ? s.RemainingAmount : s.Amount).DefaultIfEmpty(0).Sum(),
                    Commission = Math.Round(_context.UnitRentContractPaymentLogs.Where(p => p.PaymentDate.Month == currentDate.Month && p.PaymentDate.Year == currentDate.Year && p.UnitRentContractPayment.UnitRentContract.IdMandoob == r.Value && !p.UnitRentContractPayment.UnitRentContract.Archived && !p.UnitRentContractPayment.UnitRentContract.AddedtoCourt).Include(s => s.UnitRentContractPayment.UnitRentContract).Sum(s => s.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue ? !s.UnitRentContractPayment.UnitRentContract.Renewed ? s.PaidAmount * .01M : 0M : s.UnitRentContractPayment.UnitRentContract.Renewed ? s.PaidAmount * .005M : s.PaidAmount * .01M), 0)
                }).ToList()
            };
            if (User.IsInRole("Admin"))
                ViewBag.Representatives = representatives;

            model.TotalExpectedIncome = totalIncoms;
            return View(model);
        }
        private string GetName(string name)
        {
            if (name.Contains("Villa"))
                return "Villa";
            return name;
        }
        private string GetDisplayName(string name)
        {
            if (name == "AZ")
                return string.Empty;

            return name;
        }
        public ActionResult GetCompoundInsurance(int compoundID)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var currentDate = DateTime.Now.Date;
            var compoundInsurance = _context.TUnitRentContract.Where(c => !c.Archived && c.IdCompound == compoundID && (isAdmin || c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false)
                                                              .Select(t => new InsuranceItem
                                                              {
                                                                  Building = t.mCompoundUnits.mCompoundBuilding.BuildingNumber,
                                                                  ContractID = t.IdRentContract,
                                                                  ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                                  TotalDays = t.dtLeaseEnd.Subtract(currentDate).Days,
                                                                  Value = t.insurance,
                                                                  UnitNumber = t.mCompoundUnits.UnitNumber
                                                              }).ToList();
            return View("_Insurance", new InsuranceViewModel
            {
                Items = compoundInsurance,
                Name = _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName
            });
        }
        public ActionResult GetBuildingInsurance(int id)
        {
            ViewBag.id = id;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var currentDate = DateTime.Now.Date;
            var insurance = _context.TUnitRentContract.Where(c => c.mMasterBuilding == id && !c.Archived && !c.IdCompound.HasValue && (isAdmin || c.IdMandoob == _user.GetUserId(User)) && c.AddedtoCourt == false)
                                                              .Select(t => new InsuranceItem
                                                              {
                                                                  Building = t.mUnit.mBuilding.BuildingName,
                                                                  ContractID = t.IdRentContract,
                                                                  ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                                  TotalDays = t.dtLeaseEnd.Subtract(currentDate).Days,
                                                                  Value = t.insurance,
                                                                  UnitNumber = t.mUnit.UnitNumber,
                                                                  pageid = t.mMasterBuilding
                                                              }).ToList();
            return View("_Insurance", new InsuranceViewModel
            {
                Items = insurance,
                Name = "Building"
            });
        }
        public ActionResult MigratePaymentLogs()
        {
            var payments = _context.UnitRentContractPaymentLogs.Include(p => p.UnitRentContractPayment).ToList();
            foreach (var payment in payments)
            {
                payment.UserID = payment.UnitRentContractPayment.UserID;
            }
            _context.SaveChanges();
            return Json(new { val = "success" });
        }
        public ActionResult GetPaymentDetails(int? compoundID = null, int? pageid = null, bool? TotalToday = null, string representitveId = null)
        {

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            representitveId = representitveId == "null" ? null : representitveId;

            var userID = _user.GetUserId(User);
            var currentDate = DateTime.Now;
            var paymentDetails = _context.UnitRentContractPaymentLogs.Include(r => r.UnitRentContractPayment)
                                                                     .Include(r => r.UnitRentContractPayment.User)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract.Mandoob)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract.mCompoundUnits)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract.mCompoundUnits.mCompoundBuilding)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract.mUnit)
                                                                     .Include(c => c.UnitRentContractPayment.UnitRentContract.mUnit.mBuilding);

            var paymentDetailsAfterWhere = pageid != null ? paymentDetails.Where(c => (isAdmin || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                              c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                              !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived &&
                                                                              c.UnitRentContractPayment.UnitRentContract.mMasterBuilding == pageid
                                                                              && c.PaidAmount > 0)
                                                          : compoundID != null ? paymentDetails.Where(c => (isAdmin || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                               c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                               !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived &&
                                                                               c.UnitRentContractPayment.UnitRentContract.IdCompound == compoundID
                                                                               && c.PaidAmount > 0)
                                                          : TotalToday == true ? paymentDetails.Where(c => (isAdmin || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                              c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                              !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived &&
                                                                               c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                              c.PaymentDate.ToShortDateString() == currentDate.ToShortDateString()
                                                                              && c.PaidAmount > 0)
                                                          : paymentDetails.Where(c => (isAdmin || c.UnitRentContractPayment.UnitRentContract.IdMandoob == userID) &&
                                                                                c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month &&
                                                                                !c.UnitRentContractPayment.UnitRentContract.AddedtoCourt && !c.UnitRentContractPayment.UnitRentContract.Archived &&
                                                                                c.PaymentDate.Year == currentDate.Year && c.PaymentDate.Month == currentDate.Month
                                                                                && c.PaidAmount > 0);
            var result = paymentDetailsAfterWhere.OrderByDescending(b => b.PaymentDate)
                                                                     .Select(c => new PaymentLogDTO
                                                                     {
                                                                         PaidBy = string.IsNullOrEmpty(c.UserID) ? string.Empty : c.User.fullName,
                                                                         PaidAmount = c.PaidAmount,
                                                                         PaymentDate = c.PaymentDate.ToShortDateString(),
                                                                         Mandoob = c.UnitRentContractPayment.UnitRentContract.Mandoob.fullName,
                                                                         ContractNumber = c.UnitRentContractPayment.UnitRentContract.contractNumber,
                                                                         ContractID = c.UnitRentContractPayment.UnitRentContractID,
                                                                         IsBuilding = !c.UnitRentContractPayment.UnitRentContract.IdCompound.HasValue,
                                                                         Building = c.UnitRentContractPayment.UnitRentContract.mCompoundUnits != null ? c.UnitRentContractPayment.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber : c.UnitRentContractPayment.UnitRentContract.mUnit.mBuilding.BuildingName,
                                                                         Unit = c.UnitRentContractPayment.UnitRentContract.mCompoundUnits != null ? c.UnitRentContractPayment.UnitRentContract.mCompoundUnits.UnitNumber : c.UnitRentContractPayment.UnitRentContract.mUnit.UnitNumber,
                                                                     }).ToList();
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }
            if (representitveId != null)
            {
                result = result.Where(x => x.Mandoob == representitveId).ToList();
            }
            string name = compoundID.HasValue ? _context.TCompounds.Where(c => c.IdCompound == compoundID).Select(c => c.compoundName).FirstOrDefault() : "Buildings";
            return View(new PaymentLogViewModel { PaymentLogs = result, Name = name });
        }
        public ActionResult ExpectedPaymentDetails(ExpectedPaymentDetails model)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            //representitveId = representitveId == "null" ? null : representitveId;
            DateTime.TryParse(model.start, out DateTime start);
            DateTime.TryParse(model.end, out DateTime end);
            var userID = _user.GetUserId(User);
            
            var paymentDetails = _context.TUnitRentContractPayments.Where(x => x.DueDate >= start && x.DueDate <= end);
            var paymentDetailsAfterWhere = model.pageId != null ? paymentDetails.Where(c => (isAdmin || c.UnitRentContract.IdMandoob == userID) &&
                                                                              !c.UnitRentContract.AddedtoCourt && !c.UnitRentContract.Archived &&
                                                                              c.UnitRentContract.mMasterBuilding == model.pageId &&
                                                                       c.UnitRentContract.dtLeaseEnd >= DateTime.Now)
                                                          : model.compoundId != null ? paymentDetails.Where(c => (isAdmin || c.UnitRentContract.IdMandoob == userID) &&
                                                                               !c.UnitRentContract.AddedtoCourt && !c.UnitRentContract.Archived &&
                                                                               c.UnitRentContract.IdCompound == model.compoundId &&
                                                                      c.UnitRentContract.dtLeaseEnd >= DateTime.Now)
                                                          : paymentDetails.Where(c => (isAdmin || c.UnitRentContract.IdMandoob == userID) &&
                                                                               !c.UnitRentContract.AddedtoCourt && !c.UnitRentContract.Archived &&
                                                                       c.UnitRentContract.dtLeaseEnd >= DateTime.Now);
          

            var result = paymentDetailsAfterWhere.OrderBy(b => b.DueDate)
                                                                     .Select(c => new PaymentLogDTO
                                                                     {
                                                                         PaidBy = string.IsNullOrEmpty(c.UserID) ? string.Empty : c.User.fullName,
                                                                         PaidAmount = c.Amount,
                                                                         PaymentDate = c.DueDate.ToShortDateString(),
                                                                         Mandoob = c.UnitRentContract.Mandoob.fullName,
                                                                         ContractNumber = c.UnitRentContract.contractNumber,
                                                                         ContractID = c.UnitRentContractID,
                                                                         IsBuilding = !c.UnitRentContract.IdCompound.HasValue,
                                                                         Building = c.UnitRentContract.mCompoundUnits != null ? c.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber : c.UnitRentContract.mUnit.mBuilding.BuildingName,
                                                                         Unit = c.UnitRentContract.mCompoundUnits != null ? c.UnitRentContract.mCompoundUnits.UnitNumber : c.UnitRentContract.mUnit.UnitNumber,
                                                                     }).ToList();
            //if (isAdmin)
            //{
            //    var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
            //    ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
            //    ViewBag.RepresentitveId = representitveId;
            //}
            //if (representitveId != null)
            //{
            //    result = result.Where(x => x.Mandoob == representitveId).ToList();
            //}

            string name = model.compoundId.HasValue ? _context.TCompounds.Where(c => c.IdCompound == model.compoundId).Select(c => c.compoundName).FirstOrDefault() : model.pageId.HasValue ? "Buildings" : "All";
            return Json( result);
        }
        private bool HasAccess(Permission permission)
        {
            return _authorizationService.AuthorizeAsync(User, null, new OperationAuthorizationRequirement { Permission = permission }).Result.Succeeded;
        }
        public ActionResult UpdateUnits()
        {
            foreach (var item in _context.Units)
            {
                item.UnitOwner = Enums.UnitOwner.SaadGroup;
            }
            foreach (var item in _context.TCompoundUnits)
            {
                item.UnitOwner = Enums.UnitOwner.SaadGroup;
            }
            _context.SaveChanges();
            return Json(new { val = true });
        }
        public ActionResult GetRepresentitaveDashboard(string representitaveID)
        {
            var currentYear = DateTime.Today.Year;
            var values = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                           .Where(c => c.UnitRentContract.IdMandoob == representitaveID &&
                                                                       !c.UnitRentContract.AddedtoCourt && c.DueDate.Year == currentYear &&
                                                                       !c.UnitRentContract.Archived && c.UnitRentContract.dtLeaseEnd >= DateTime.Now)
                                                           .GroupBy(d => d.DueDate.Month)
                                                           .Select(m => new
                                                           {
                                                               Seconds = new DateTime(currentYear, m.Key, 1),
                                                               Target = m.Select(S => S.Amount).DefaultIfEmpty(0).Sum(),
                                                               Actual = m.Select(s => s.PaidAmount).DefaultIfEmpty(0).Sum()
                                                           }).ToList();
            return Json(values);
        }
        public ActionResult GetTenants(int id, int? compoundID = null)
        {
            var tenants = _context.TUnitRentContract.Include(c => c.mTenant).Include(c => c.mCompoundUnits).Include(c => c.mUnit)
                                                    .Where(t => t.mMasterBuilding == id && t.IdCompound == compoundID && !t.Archived && !t.AddedtoCourt)
                                                    .Select(t => new Tenant
                                                    {
                                                        ContractID = t.IdRentContract,
                                                        Email = t.mTenant.tenantEmail,
                                                        EmergencyContact = t.mTenant.emergencyPhone,
                                                        MobileNumber = t.mTenant.tenantMobile,
                                                        Name = t.mTenant.tenantName,
                                                        UnitNumber = t.IdUnitCompound.HasValue ? t.mCompoundUnits.UnitNumber : t.mUnit.UnitNumber,
                                                        BuildingNumber = t.IdUnit.HasValue ? t.mUnit.mBuilding.BuildingName : null,
                                                        ID = t.IdTenant,
                                                        pageid = t.mMasterBuilding,
                                                    }).ToList();
            ViewBag.CompoundName = compoundID.HasValue ? GetCompoundData(compoundID.Value).CompoundName : "Building";
            ViewBag.id = id;
            return View(tenants);
        }
        
       
    }
    public class Tenant
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string UnitNumber { get; set; }
        public int ContractID { get; set; }
        public string Email { get; set; }
        public string EmergencyContact { get; set; }
        public string BuildingNumber { get; set; }
        public int ID { get; set; }
        public int pageid { get; set; }
    }
    public class DueValues
    {
        public double Due { get; set; }
        public double DueOver60 { get; set; }
    }
    public class DueValue
    {
        public int ContractID { get; set; }
        public string UnitNumber { get; set; }
        public string TenantName { get; set; }
        public string Mobile { get; set; }
        public int AnnualRent { get; set; }
        public int RemainingRents { get; set; }
        public int Value { get; set; }
        public string CollectionDate { get; set; }
        public string ExpiryDate { get; set; }
        public int TotalDays { get; set; }
        public string Building { get; set; }
        public string Note { get; set; }
        public string Mandoob { get; set; }
        public int NoteID { get; set; }
        public List<DueDatesWithValues> RemainingDates { get; set; }
        public string TenantEmail { get; internal set; }
        public string MandoobPhone { get; internal set; }
        public string MandoobID { get; set; }
        public string PropertyType { get; set; }
        public string NoteDate { get; set; }
    }
    public class DueDatesWithValues
    {
        public DateTime DueDate { get; set; }
        public int RemainingAmount { get; set; }
    }
    public class DueViewModel
    {
        public int TotalValue { get; set; }
        public List<DueValue> Due { get; set; } = new List<DueValue>();
        public int TotalAnnualRent { get { return Due.Sum(t => t.AnnualRent); } }
        public int TotalRentValue { get { return Due.Sum(t => t.Value); } }
    }
    public class NoteDTO
    {
        public string Note { get; set; }
        public string Mandoob { get; set; }
        public string Created { get; set; }
    }
    public class InsuranceViewModel
    {
        public int Total { get { return Items.Sum(i => i.Value); } }
        public List<InsuranceItem> Items { get; set; } = new List<InsuranceItem>();
        public string Name { get; set; }
    }
    public class InsuranceItem
    {
        public int ContractID { get; set; }
        public string UnitNumber { get; set; }
        public string TenantName { get; set; }
        public string ExpiryDate { get; set; }
        public int TotalDays { get; set; }
        public string Building { get; set; }
        public int Value { get; set; }
        public int pageid { get; set; }
    }
    public class DashboardViewModel
    {
        public string MonthName { get; set; }
        public string day { get; set; }
        public string date { get; set; }
        public decimal TotalRevenuToday { get { return Items.Sum(i => i.RevenuOFDay); } }
        public decimal TotalRevenu { get { return Items.Sum(i => i.Revenu); } }
        public List<DashboardListItem> Items { get; set; } = new List<DashboardListItem>();
        public List<CompoundUnitsSummary> UnitsSummary { get; set; } = new List<CompoundUnitsSummary>();
        public List<UnitsPieChartSummary> UnitsPieChartSummary { get; set; } = new List<UnitsPieChartSummary>();
        public RepresentativeTable RepresentativeTable { get; set; }
        public List<ExpectedIncome> ExpectedIncomes { get; set; } = new List<ExpectedIncome>();
        public List<decimal> TotalExpectedIncome { get; set; } = new List<decimal>();
        public List<string> MonthsNames { get; set; }
        public List<OccupancyRegion> occupancyRegions { get; set; }
    }
    public class OccupancyRegion
    {
        public string Region { get; set; }
        public int AppartmentNumber { get; set; }
        public int occupiedNumber { get; set; }
        public int NonOccupiedNumber { get; set; }
    }
    public class CompoundUnitsSummary
    {
        public string CompoundName { get; set; }
        public int TotalVacant { get { return Items.Sum(i => i.Vacant); } }
        public int TotalOccupied { get { return Items.Sum(i => i.Occupied); } }
        public int Total { get { return Items.Sum(i => i.Total); } }
        public List<CompoundUnitsSummaryDetail> Items { get; set; } = new List<CompoundUnitsSummaryDetail>();
        public string ColumnClass { get; set; }
    }
    public class CompoundUnitsSummaryDetail
    {
        public string Type { get; set; }
        public int Vacant { get; set; }
        public int Occupied { get; set; }
        public int Total { get { return Vacant + Occupied; } }
    }
    public class DashboardListItem
    {
        public string Name { get; set; }
        public decimal Revenu { get; set; }
        public decimal RevenuOFDay { get; set; }
        public string BGColor { get; set; }
        public string ActionURL { get; set; }
    }
    public class PaymentLogDTO
    {
        public string PaidBy { get; set; }
        public int PaidAmount { get; set; }
        public string PaymentDate { get; set; }
        public string Mandoob { get; set; }
        public string ContractNumber { get; set; }
        public int ContractID { get; set; }
        public bool IsBuilding { get; set; }
        public string Building { get; set; }
        public string Unit { get; set; }
    }
    public class PaymentLogViewModel
    {
        public List<PaymentLogDTO> PaymentLogs { get; set; }
        public string Name { get; set; }
    }
    public class UnitsPieChartSummary
    {
        public int Vacant { get; set; }
        public int Occupied { get; set; }
        public string PieChartID { get; set; }
        public string PieChartName { get; set; }
    }
    public class RepresentativeTable
    {
        public List<RepresentativeTableItem> Items { get; set; } = new List<RepresentativeTableItem>();
        public decimal TotalCollectedAmount { get { return Math.Round(Items.Sum(s => s.CollectedAmount), 2); } }
        public decimal TotalCommission { get { return Math.Round(Items.Sum(s => s.Commission), 0); } }
        public decimal TotalTargetAmount { get { return Math.Round(Items.Sum(s => s.DueAmount), 2); } }
        public decimal TotalDifferenceAmount { get { return Math.Round(Items.Sum(s => s.Difference), 2); } }
        public int TotalNewContracts { get { return Items.Sum(s => s.NewContracts); } }
        public int TotalRenewedContracts { get { return Items.Sum(s => s.RenewedContracts); } }
        public bool ShowTable { get { return Items.Count > 0; } }
    }
    public class RepresentativeTableItem
    {
        public string Name { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal Difference { get { return CollectedAmount - DueAmount; } }
        public decimal Commission { get; set; }
        public int NewContracts { get; set; }
        public int RenewedContracts { get; set; }
    }
    public class ExpectedIncome
    {
        public string CompoundName { get; set; }
        public List<ExpectedIncomeItem> Items { get; set; } = new List<ExpectedIncomeItem>();
    }
    public class ExpectedIncomeItem
    {
        public string MonthName { get; set; }
        public decimal Income { get; set; }
    }
    public class ExpectedPaymentDetails
    {
        public string start { get; set; }
        public string end { get; set; }
        public int? pageId { get; set; }
        public int? compoundId { get; set; }
    }
}
