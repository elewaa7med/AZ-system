﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Authorization;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.DTO;
using SmartAdmin.WebUI.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class UnitRentContractCompoundController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        private readonly UserManager<ApplicationUser> _user;

        private readonly IFileProvider _fileProvider;
        NumberToTextLogic _numberToTextLogic;
        NumberToTextLogic _numberToTextMonthLogic;

        public UnitRentContractCompoundController(ApplicationDbContext context, UserManager<ApplicationUser> user, IFileProvider fileProvider, IHostingEnvironment env)
        {
            _context = context;
            _user = user;
            _fileProvider = fileProvider;
            _numberToTextLogic = new NumberToTextLogic(new CurrencyInfoLogic(Currency.SaudiArabia));
            _numberToTextMonthLogic = new NumberToTextLogic(0, new CurrencyInfoLogic(Currency.Normal), string.Empty, string.Empty, string.Empty, string.Empty);
            _env = env;
        }

        public async Task<IActionResult> Index(string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            _context.TUnitRentContract.Where((UnitRentContract m) => false);
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                    .Include((UnitRentContract m) => m.mCompoundUnits)
                    .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                    .Include(m => m.mCompoundUnits.mPropertyType)
                                                                              where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound != null && m.Archived == false
                                                                              select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                    .Include((UnitRentContract m) => m.mCompoundUnits)
                    .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                    .Include(m => m.mCompoundUnits.mPropertyType)

                                                                                           where m.IdCompound != null && m.Archived == false && (string.IsNullOrEmpty(representitveId) ? true : m.IdMandoob == representitveId)
                                                                                           select m);

            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Any(s => s == Permission.MeadowParkGarden || s == Permission.DesertRose || s == Permission.DaarResidence || s == Permission.Villa21 || s == Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }
            return View(await applicationDbContext.Where(e => e.AddedtoCourt == false).ToListAsync());
        }

        private async Task setLastPayment(int IdContract)
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
        private void UpdateLastPayment(int IdContract, DateTime date)
        {
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                SqlParameter IdCompoundParameter = new SqlParameter("IdContract", SqlDbType.Int)
                {
                    Value = IdContract
                };
                SqlParameter dateParameter = new SqlParameter("dtlastrentpayment", SqlDbType.DateTime2)
                {
                    Value = date
                };
                command.Parameters.Add(IdCompoundParameter);
                command.Parameters.Add(dateParameter);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "UpdateLastPayment";
                _context.Database.OpenConnection();
                command.ExecuteNonQuery();
            }
        }
        public async Task<IActionResult> LoadCompound(int id)
        {

            var comp = _context.TCompounds.SingleOrDefault(c => c.IdCompound == id);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");

            if (comp != null)
            {
                ViewBag.caption = "All contracts of " + comp.compoundName;

                _context.TUnitRentContract.Where((UnitRentContract m) => false);
                IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                        .Include((UnitRentContract m) => m.mCompoundUnits)
                        .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                        .Include(c => c.mCompoundUnits.mPropertyType)
                                                                                  where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound != null && m.Archived == false
                                                                                  select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
.Include((UnitRentContract m) => m.mCompoundUnits)
.ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                        .Include(c => c.mCompoundUnits.mPropertyType)

                                                                                               where m.IdCompound != null && m.Archived == false
                                                                                               select m);

                var compList = applicationDbContext.Where(c => c.IdCompound == id && c.AddedtoCourt == false);

                return PartialView("_LoadCompound", await compList.ToListAsync());
            }
            else
            {
                return NotFound();
            }

        }

        public async Task<IActionResult> Archived(string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");

            _context.TUnitRentContract.Where((UnitRentContract m) => false);
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                    .Include((UnitRentContract m) => m.mCompoundUnits)
                    .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                                              where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound != null && m.Archived == true
                                                                              select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                                                                              .Include((UnitRentContract m) => m.mCompoundUnits)
                                                                              .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                                                           where m.IdCompound != null && m.Archived == true && (string.IsNullOrEmpty(representitveId) ? true : m.IdMandoob == representitveId)
                                                                                           select m);
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Any(s => s == Permission.MeadowParkGarden || s == Permission.DesertRose || s == Permission.DaarResidence || s == Permission.Villa21 || s == Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> ArchivedCompound(int compoundID, string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy)
                                                                                                                  .Include((UnitRentContract u) => u.mModifiedBy)
                                                                                                                  .Include((UnitRentContract u) => u.mTenant)
                                                                                                                  .Include((UnitRentContract m) => m.mCompoundUnits)
                                                                                                                  .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                                              where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound != null && m.Archived == true && m.IdCompound == compoundID
                                                                              select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                                                                              .Include((UnitRentContract m) => m.mCompoundUnits)
                                                                              .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                                                           where m.IdCompound != null && m.Archived == true && (string.IsNullOrEmpty(representitveId) || m.IdMandoob == representitveId) && m.IdCompound == compoundID
                                                                                           select m);
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Any(s => s == Permission.MeadowParkGarden || s == Permission.DesertRose || s == Permission.DaarResidence || s == Permission.Villa21 || s == Permission.Villa24) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }
            ViewBag.CompoundName = _context.TCompounds.Where(c => c.IdCompound == compoundID).Select(c => c.compoundName).FirstOrDefault().Trim();
            ViewBag.CompoundID = compoundID;
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            UnitRentContract unitRentContract = await _context
                .TUnitRentContract
                .Include((UnitRentContract t) => t.mUnit)
                .Include(x => x.invoices)
                .Include(e => e.UnitRentContractPayments)
                .ThenInclude(e => e.User)
                .Include(e => e.UnitRentContractNotes)
                .ThenInclude(e => e.User)
                .SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            unitRentContract.invoices = _context.Invoices.Where(x => x.ContractId == id).ToList();
            if (unitRentContract == null)
            {
                return NotFound();
            }
            int IdBuilding = (from m in _context.TCompoundUnits
                              where (int?)m.IdUnit == unitRentContract.IdUnitCompound
                              select m.IdBuilding).FirstOrDefault();
            unitRentContract.IdBuilding = IdBuilding;
            unitRentContract.UnitIncluded = new System.Collections.Generic.List<int>();
            if (unitRentContract.waterBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(1);
            }
            if (unitRentContract.electricityBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(2);
            }
            if (unitRentContract.furnishedUnit)
            {
                unitRentContract.UnitIncluded.Add(3);
            }
            if (unitRentContract.attestedRentContract)
            {
                unitRentContract.UnitIncluded.Add(4);
            }
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!base.HttpContext.User.IsInRole("Admin")) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                select new
                                                                                {
                                                                                    IdBuilding = m.IdBuilding,
                                                                                    IdCompound = m.IdCompound,
                                                                                    BuildingName = m.BuildingNumber,
                                                                                    BuildingAddress = m.BuildingAddress,
                                                                                    DistrictName = m.mCompound.compoundName,
                                                                                    buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                } into m
                                                                                where (from l in _context.TCompoundsUsers
                                                                                       select new
                                                                                       {
                                                                                           l.IdCompound,
                                                                                           l.IdUser
                                                                                       } into p
                                                                                       where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                       select p into j
                                                                                       select j.IdCompound).Contains(m.IdCompound)
                                                                                orderby m.DistrictName, m.BuildingName
                                                                                select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                   select new
                                                                                                                   {
                                                                                                                       IdBuilding = m.IdBuilding,
                                                                                                                       IdCompound = m.IdCompound,
                                                                                                                       BuildingName = m.BuildingNumber,
                                                                                                                       BuildingAddress = m.BuildingAddress,
                                                                                                                       DistrictName = m.mCompound.compoundName,
                                                                                                                       buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                   } into m
                                                                                                                   orderby m.DistrictName, m.BuildingName
                                                                                                                   select m).ToListAsync());
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            return View(unitRentContract);
        }

        public async Task<IActionResult> Create(int? compoundID = null)
        {
            string IdCreated = "";
            ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName");
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            if (compoundID.HasValue)
                building = building.Where(b => b.IdCompound == compoundID).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo");
            if (_user.GetUserName(base.HttpContext.User) != null)
            {
                IdCreated = _user.GetUserId(base.HttpContext.User);
            }
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", IdCreated);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitRentContract unitRentContract, IFormFile contractImageFile)
        {
            bool unitrentedFlag = true;
            var rentContractUnit = _context.TUnitRentContract.Include(e => e.mCompoundUnits).FirstOrDefault(e =>
             e.IdUnitCompound == unitRentContract.IdUnitCompound
             && e.mCompoundUnits.isRented == true
             && !e.Archived);
            if (rentContractUnit != null)
            {
                base.ViewData["isRentedErrMsg"] = "This Unit is already rented with contract No.: " + rentContractUnit.contractNumber;
                unitrentedFlag = false;
            }

            if (unitrentedFlag && base.ModelState.IsValid)
            {
                if (unitRentContract.IdUnitCompound.HasValue && unitRentContract.IdUnitCompound > 0)
                {
                    unitRentContract.dtCreated = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        unitRentContract.IdCreated = _user.GetUserId(base.HttpContext.User);
                    }
                    if (unitRentContract.UnitIncluded != null && unitRentContract.UnitIncluded.Any())
                        for (int i = 0; i < unitRentContract.UnitIncluded.Count; i++)
                        {
                            switch (unitRentContract.UnitIncluded[i])
                            {
                                case 1:
                                    unitRentContract.waterBillIncluded = true;
                                    break;
                                case 2:
                                    unitRentContract.electricityBillIncluded = true;
                                    break;
                                case 3:
                                    unitRentContract.furnishedUnit = true;
                                    break;
                                case 4:
                                    unitRentContract.attestedRentContract = true;
                                    break;
                            }
                        }

                    int IdCompound = (from m in _context.TCompoundBuildings
                                      where m.IdBuilding == unitRentContract.IdBuilding
                                      select m.IdCompound).SingleOrDefault();
                    unitRentContract.IdCompound = IdCompound;
                    if (contractImageFile != null && contractImageFile.Length != 0L)
                    {
                        unitRentContract.contractImage = await SaveFileToDirectory(contractImageFile, "contracts");
                    }
                    var PayoldRents = false;
                    var paymentsCount = 0;
                    DateTime lastPay = new DateTime();

                    //add water to yearly rent
                    var yearlyRent = unitRentContract.yearlyRent;

                    if (unitRentContract.paymentMethod == 1)
                    {
                        var rent = yearlyRent / unitRentContract.leasePeriodInMonthes.Value;
                        for (int i = 0; i < unitRentContract.leasePeriodInMonthes.Value; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == unitRentContract.leasePeriodInMonthes.Value - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(i),
                            });
                        }
                        lastPay = unitRentContract.dtLeaseStart.AddMonths(-1);
                        //unitRentContract.paidAmount = paidInstallmentsCount * (unitRentContract.yearlyRent / unitRentContract.leasePeriodInMonthes.Value);
                    }

                    else if (unitRentContract.paymentMethod == 2)
                    {
                        paymentsCount = unitRentContract.leasePeriodInMonthes.Value / 3;
                        var rent = yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 3);
                        var month = 0;
                        for (int i = 0; i < paymentsCount; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == paymentsCount - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(month)
                            });
                            month += 3;
                        }
                        lastPay = unitRentContract.dtLeaseStart.AddMonths(-3);
                        //unitRentContract.paidAmount = paidInstallmentsCount * (unitRentContract.yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 3));
                    }

                    else if (unitRentContract.paymentMethod == 3)
                    {
                        paymentsCount = unitRentContract.leasePeriodInMonthes.Value / 4;
                        var rent = yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 4);
                        var month = 0;
                        for (int i = 0; i < paymentsCount; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == paymentsCount - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(month)
                            });
                            month += 4;
                        }
                        lastPay = unitRentContract.dtLeaseStart.AddMonths(-4);
                        //unitRentContract.paidAmount = paidInstallmentsCount * (unitRentContract.yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 4));
                    }

                    else if (unitRentContract.paymentMethod == 4)
                    {
                        paymentsCount = unitRentContract.leasePeriodInMonthes.Value / 6;
                        var rent = yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 6);
                        var month = 0;
                        for (int i = 0; i < paymentsCount; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == paymentsCount - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(month)
                            });
                            month += 6;
                        }
                        lastPay = unitRentContract.dtLeaseStart.AddMonths(-6);
                        //unitRentContract.paidAmount = paidInstallmentsCount * (unitRentContract.yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 6));
                    }

                    else if (unitRentContract.paymentMethod == 5)
                    {
                        paymentsCount = unitRentContract.leasePeriodInMonthes.Value / 12;
                        var rent = yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 12);
                        var month = 0;
                        for (int i = 0; i < paymentsCount; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == paymentsCount - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(month)
                            });
                            month += 12;
                        }
                        lastPay = unitRentContract.dtLeaseStart.AddMonths(-12);
                    }

                    if (unitRentContract.VerifiedFromGovernment)
                        unitRentContract.NotVerifiedReason = null;
                    _context.Add(unitRentContract);
                    var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.CompoundUnitID == unitRentContract.IdUnitCompound);
                    if (elctricityData == null)
                        elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { CompoundUnitID = unitRentContract.IdUnitCompound }).Entity;
                    elctricityData.PaymentNumber = unitRentContract.PaymentNumber;
                    elctricityData.ElectricityMeterNumber = unitRentContract.ElectricityNumber;
                    elctricityData.StartOfMeter = unitRentContract.MeterStart;
                    elctricityData.TransferTheAccountToTenant = unitRentContract.TransferToTenanat;
                    try
                    {
                        await _context.SaveChangesAsync();
                        UpdateLastPayment(unitRentContract.IdRentContract, lastPay);
                        if (PayoldRents)
                        {
                            await setLastPayment(unitRentContract.IdRentContract);
                        }
                        CompoundUnits compoundUnit = _context.TCompoundUnits.Where((CompoundUnits m) => (int?)m.IdUnit == unitRentContract.IdUnitCompound).FirstOrDefault();
                        compoundUnit.isRented = true;
                        compoundUnit.UnitRentContractID = unitRentContract.IdRentContract;
                        _context.Update(compoundUnit);
                        _context.SaveChanges();
                        //await setRentInfoInAllUnits();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        base.ViewData["AlertSaveErr"] = e.InnerException.Message + "\nThere is an Error Savng the contract. Please correct and try again.";
                    }
                }
                else
                {
                    base.ViewData["isRentedErrMsg"] = "Please Select Unit ";
                }
            }
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            if (unitRentContract.IdCompound.HasValue)
                building = building.Where(b => b.IdCompound == unitRentContract.IdCompound).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo");
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == unitRentContract.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);

            return View(unitRentContract);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            UnitRentContract unitRentContract = await _context.TUnitRentContract.SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            if (unitRentContract == null)
            {
                return NotFound();
            }
            int IdBuilding = (from m in _context.TCompoundUnits
                              where (int?)m.IdUnit == unitRentContract.IdUnitCompound
                              select m.IdBuilding).FirstOrDefault();
            unitRentContract.IdBuilding = IdBuilding;
            unitRentContract.lstchkBoxes = "";
            unitRentContract.UnitIncluded = new System.Collections.Generic.List<int>();
            if (unitRentContract.waterBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(1);
            }
            if (unitRentContract.electricityBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(2);
            }
            if (unitRentContract.furnishedUnit)
            {
                unitRentContract.UnitIncluded.Add(3);
            }
            if (unitRentContract.attestedRentContract)
            {
                unitRentContract.UnitIncluded.Add(4);
            }
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());

            //if (unitRentContract.IdCompound.HasValue)
            //    building = building.Where(b => b.IdCompound == unitRentContract.IdCompound).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"} };
            return View(unitRentContract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UnitRentContract unitRentContract, IFormFile contractImageFile)
        {
            if (id != unitRentContract.IdRentContract)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                if (unitRentContract.IdUnitCompound.HasValue)
                {
                    try
                    {
                        unitRentContract.dtModified = DateTime.Now;
                        if (_user.GetUserName(base.HttpContext.User) != null)
                        {
                            unitRentContract.IdModified = _user.GetUserId(base.HttpContext.User);
                        }
                        if (unitRentContract.UnitIncluded != null && unitRentContract.UnitIncluded.Any())
                            for (int i = 0; i < unitRentContract.UnitIncluded.Count; i++)
                            {
                                switch (unitRentContract.UnitIncluded[i])
                                {
                                    case 1:
                                        unitRentContract.waterBillIncluded = true;
                                        break;
                                    case 2:
                                        unitRentContract.electricityBillIncluded = true;
                                        break;
                                    case 3:
                                        unitRentContract.furnishedUnit = true;
                                        break;
                                    case 4:
                                        unitRentContract.attestedRentContract = true;
                                        break;
                                }
                            }
                        int IdCompound = (from m in _context.TCompoundBuildings
                                          where m.IdBuilding == unitRentContract.IdBuilding
                                          select m.IdCompound).SingleOrDefault();
                        unitRentContract.IdCompound = IdCompound;
                        if (contractImageFile != null && contractImageFile.Length != 0L)
                        {
                            unitRentContract.contractImage = await SaveFileToDirectory(contractImageFile, "contracts");
                        }
                        DateTime date = new DateTime();
                        //add water to yearly rent
                        var yearlyRent = unitRentContract.yearlyRent;
                        unitRentContract.UnitRentContractPayments = _context.TUnitRentContractPayments.Include(x => x.InvoiceRelatedPaymentDates).Where(x => x.UnitRentContractID == unitRentContract.IdRentContract).ToList();
                        switch (unitRentContract.paymentMethod)
                        {
                            case 1:
                                date = ApplyPayments(unitRentContract, yearlyRent / unitRentContract.leasePeriodInMonthes.Value,
                                              unitRentContract.leasePeriodInMonthes.Value, 1);
                                break;
                            case 2:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 3),
                                              unitRentContract.leasePeriodInMonthes.Value / 3, 3);
                                break;
                            case 3:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 4),
                                               unitRentContract.leasePeriodInMonthes.Value / 4, 4);
                                break;
                            case 4:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 6),
                                              unitRentContract.leasePeriodInMonthes.Value / 6, 6);
                                break;
                            case 5:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 12),
                                              unitRentContract.leasePeriodInMonthes.Value / 12, 12);
                                break;
                        }
                        if (unitRentContract.VerifiedFromGovernment)
                            unitRentContract.NotVerifiedReason = null;
                        var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.CompoundUnitID == unitRentContract.IdUnitCompound);
                        if (elctricityData == null)
                            elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { CompoundUnitID = unitRentContract.IdUnitCompound }).Entity;
                        elctricityData.PaymentNumber = unitRentContract.PaymentNumber;
                        elctricityData.ElectricityMeterNumber = unitRentContract.ElectricityNumber;
                        elctricityData.StartOfMeter = unitRentContract.MeterStart;
                        elctricityData.TransferTheAccountToTenant = unitRentContract.TransferToTenanat;
                        _context.Update(unitRentContract);
                        await _context.SaveChangesAsync();
                        //await setRentInfoInAllUnits();
                        UpdateLastPayment(unitRentContract.IdRentContract, date);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UnitRentContractExists(unitRentContract.IdRentContract))
                        {
                            return NotFound();
                        }
                        base.ViewData["AlertSaveErr"] = "There is an Error Savng the contract. Please correct and try again.";
                    }
                    return RedirectToAction("Index");
                }
                base.ViewData["isRentedErrMsg"] = "Please Select Unit ";
            }
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            //if (unitRentContract.IdCompound.HasValue)
            //    building = building.Where(b => b.IdCompound == unitRentContract.IdCompound).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo");
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == unitRentContract.mCompoundUnits.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"} };
            return View(unitRentContract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            UnitRentContract unitRentContract = await _context.TUnitRentContract.SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            if (unitRentContract == null)
            {
                return NotFound();
            }
            int IdBuilding = (from m in _context.TCompoundUnits
                              where (int?)m.IdUnit == unitRentContract.IdUnitCompound
                              select m.IdBuilding).FirstOrDefault();
            unitRentContract.IdBuilding = IdBuilding;
            unitRentContract.lstchkBoxes = "";
            if (unitRentContract.waterBillIncluded)
            {
                unitRentContract.lstchkBoxes += "1";
            }
            if (unitRentContract.electricityBillIncluded)
            {
                if (unitRentContract.lstchkBoxes.Trim() == "")
                {
                    unitRentContract.lstchkBoxes += "2";
                }
                else
                {
                    unitRentContract.lstchkBoxes += ",2";
                }
            }
            if (unitRentContract.furnishedUnit)
            {
                if (unitRentContract.lstchkBoxes.Trim() == "")
                {
                    unitRentContract.lstchkBoxes += "3";
                }
                else
                {
                    unitRentContract.lstchkBoxes += ",3";
                }
            }
            if (unitRentContract.attestedRentContract)
            {
                if (unitRentContract.lstchkBoxes.Trim() == "")
                {
                    unitRentContract.lstchkBoxes += "4";
                }
                else
                {
                    unitRentContract.lstchkBoxes += ",4";
                }
            }
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            return View(unitRentContract);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string returnTo = "Index";

            try
            {
                UnitRentContract unitRentContract = await _context.TUnitRentContract.Include(m => m.mCompoundUnits).SingleOrDefaultAsync((UnitRentContract m) => m.IdRentContract == id);
                if (unitRentContract.mCompoundUnits != null)
                {
                    unitRentContract.mCompoundUnits.isRented = false;
                    unitRentContract.mCompoundUnits.UnitRentContractID = null;
                }
                // H1 ----------------------
                if (unitRentContract.Archived)
                {
                    returnTo = "Archived";
                }
                //-----------------
                _context.TUnitRentContract.Remove(unitRentContract);
                await _context.SaveChangesAsync();
                //await setRentInfoInAllUnits();

                // H1
                TempData["success"] = "Operation is done successfully";
            }
            catch (Exception ex)
            {
                TempData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";

                // H1 : temp
                TempData["devex"] = ex.Message + " / " + ex.InnerException?.Message ?? "";

            }
            return RedirectToAction(returnTo);
        }

        //[HttpPost]
        //[ActionName("Archive")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Archive(int id)
        //{
        //    return View();
        //}

        private bool UnitRentContractExists(int id)
        {
            return _context.TUnitRentContract.Any((UnitRentContract e) => e.IdRentContract == id);
        }

        public async Task<JsonResult> getBuildingUnits(int IdBuilding, int? unitID = null)
        {
            var result = await _context.TCompoundUnits
                .Where(e => e.IdBuilding == IdBuilding && e.UnitOwner != Enums.UnitOwner.xSaad && (e.IdUnit == unitID || e.isRented != true))
                .OrderBy(e => e.UnitNumber)
                .Select(e => new
                {
                    IdUnitCompound = e.IdUnit,
                    e.UnitNumber,
                    e.IdBuilding
                }).ToListAsync();

            var selectUnitList = new SelectList(result, "IdUnitCompound", "UnitNumber");

            return Json(selectUnitList);

            //return Json(new SelectList(await (from m in _context.TCompoundUnits
            //                                  select new
            //                                  {
            //                                      IdUnitCompound = m.IdUnit,
            //                                      UnitNumber = m.UnitNumber,
            //                                      IdBuilding = m.IdBuilding
            //                                  } into m
            //                                  where m.IdBuilding == IdBuilding
            //                                  orderby m.UnitNumber
            //                                  select m).ToListAsync(), "IdUnitCompound", "UnitNumber"));



        }

        public JsonResult getUnitInfo(int IdUnitCompound)
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
                            where m.IdUnitCompound == IdUnitCompound
                            select m).FirstOrDefault();
            //return Json(unitInfo);
            var unit = _context.ElectricityMeters.Where(u => u.CompoundUnitID == IdUnitCompound).Select(u => new
            {
                u.PaymentNumber,
                u.ElectricityMeterNumber,
                u.StartOfMeter,
                u.TransferTheAccountToTenant
            }).FirstOrDefault();
            if (unitInfo == null)
                return Json(null);
            return Json(new
            {
                IdUnitCompound = unitInfo.IdUnitCompound,
                unitArea = unitInfo.unitArea,
                buildingNo = unitInfo.buildingNo,
                buildingAddress = unitInfo.buildingAddress,
                district = unitInfo.district,
                city = unitInfo.city,
                country = unitInfo.country,
                owner = unitInfo.owner,
                compoundName = unitInfo.compoundName,
                payment_number = unit?.PaymentNumber,
                electricity_meter = unit?.ElectricityMeterNumber,
                transfer_tenant = unit?.TransferTheAccountToTenant,
                start_meter = unit?.StartOfMeter
            });
        }

        public JsonResult getTenantInfo(int IdTenant)
        {
            var tenantInfo = (from m in _context.TTenants.Include((Tenants t) => t.mNationality).Include((Tenants t) => t.mCompany).Include((Tenants t) => t.mNationality)
                              select new
                              {
                                  IdTenant = m.IdTenant,
                                  iqamaId = m.IqamaNo,
                                  mobileNumber = m.tenantMobile,
                                  work = m.mCompany.companyName,
                                  companyPhone = m.mCompany.compayPhone,
                                  nationality = m.mNationality.CountryName
                              } into m
                              where m.IdTenant == IdTenant
                              select m).FirstOrDefault();
            return Json(tenantInfo);
        }

        public async Task setRentInfoInAllUnits()
        {
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "setRentInfoInAllUnits";
                _context.Database.OpenConnection();
                await command.ExecuteNonQueryAsync();
            }
        }

        [ActionName("Archive")]
        public async Task<IActionResult> Archive(int id)
        {
            try
            {
                UnitRentContract unitRentContract = await _context.TUnitRentContract.Include(u => u.mCompoundUnits).SingleOrDefaultAsync(m => m.IdRentContract == id);
                unitRentContract.Archived = true;
                if (unitRentContract.mCompoundUnits != null)
                {
                    unitRentContract.mCompoundUnits.isRented = false;
                    unitRentContract.mCompoundUnits.UnitRentContractID = null;
                }
                _context.TUnitRentContract.Update(unitRentContract);
                await _context.SaveChangesAsync();
                TempData["success"] = "Operation completed successfully";
                //await setRentInfoInAllUnits();
            }
            catch
            {
                TempData["AlertSaveErr"] = "There is an Error when archiving . Please correct and try again.";
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UnArchive(int id)
        {
            try
            {

                UnitRentContract unitRentContract = await _context.TUnitRentContract.Include(u => u.mCompoundUnits).SingleOrDefaultAsync(m => m.IdRentContract == id);

                unitRentContract.Archived = false;
                if (unitRentContract.mCompoundUnits != null)
                {
                    if (unitRentContract.mCompoundUnits.isRented == true)
                    {
                        ViewData["AlertSaveErr"] = "This Unit is already rented";
                        return RedirectToAction("Archived");
                    }
                    unitRentContract.mCompoundUnits.isRented = true;
                    unitRentContract.mCompoundUnits.UnitRentContractID = unitRentContract.IdRentContract;
                }
                _context.TUnitRentContract.Update(unitRentContract);
                await _context.SaveChangesAsync();
                TempData["success"] = "Operation completed successfully";
                //await setRentInfoInAllUnits();
            }
            catch
            {
                TempData["AlertSaveErr"] = "There is an Error when archiving . Please correct and try again.";
            }

            return RedirectToAction("Index");
        }

        [ActionName("Renew")]
        public async Task<IActionResult> Renew(int id)
        {

            UnitRentContract unitRentContract = await _context.TUnitRentContract.SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            if (unitRentContract == null)
            {
                return NotFound();
            }
            int IdBuilding = (from m in _context.TCompoundUnits
                              where (int?)m.IdUnit == unitRentContract.IdUnitCompound
                              select m.IdBuilding).FirstOrDefault();
            unitRentContract.IdBuilding = IdBuilding;
            unitRentContract.UnitIncluded = new System.Collections.Generic.List<int>();
            if (unitRentContract.waterBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(1);
            }
            if (unitRentContract.electricityBillIncluded)
            {
                unitRentContract.UnitIncluded.Add(2);
            }
            if (unitRentContract.furnishedUnit)
            {
                unitRentContract.UnitIncluded.Add(3);
            }
            if (unitRentContract.attestedRentContract)
            {
                unitRentContract.UnitIncluded.Add(4);
            }
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            if (unitRentContract.IdCompound.HasValue)
                building = building.Where(b => b.IdCompound == unitRentContract.IdCompound).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", IdBuilding);
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"} };
            return View(unitRentContract);

        }

        [HttpPost]
        [ActionName("Renew")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id, UnitRentContract unitRentContract, IFormFile contractImageFile, string PrevContractNum)
        {
            if (id != unitRentContract.IdRentContract)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                if (unitRentContract.IdUnitCompound.HasValue)
                {
                    try
                    {
                        unitRentContract.IdRentContract = 0;
                        unitRentContract.dtCreated = DateTime.Now;
                        if (_user.GetUserName(base.HttpContext.User) != null)
                        {
                            unitRentContract.IdCreated = _user.GetUserId(base.HttpContext.User);
                        }
                        if (unitRentContract.UnitIncluded != null && unitRentContract.UnitIncluded.Any())
                            for (int i = 0; i < unitRentContract.UnitIncluded.Count; i++)
                            {
                                switch (unitRentContract.UnitIncluded[i])
                                {
                                    case 1:
                                        unitRentContract.waterBillIncluded = true;
                                        break;
                                    case 2:
                                        unitRentContract.electricityBillIncluded = true;
                                        break;
                                    case 3:
                                        unitRentContract.furnishedUnit = true;
                                        break;
                                    case 4:
                                        unitRentContract.attestedRentContract = true;
                                        break;
                                }
                            }
                        int IdCompound = (from m in _context.TCompoundBuildings
                                          where m.IdBuilding == unitRentContract.IdBuilding
                                          select m.IdCompound).SingleOrDefault();
                        unitRentContract.IdCompound = IdCompound;
                        if (contractImageFile != null && contractImageFile.Length != 0L)
                        {
                            unitRentContract.contractImage = await SaveFileToDirectory(contractImageFile, "contracts");
                        }
                        DateTime date = new DateTime();
                        //add water to yearly rent
                        var yearlyRent = unitRentContract.yearlyRent;

                        switch (unitRentContract.paymentMethod)
                        {
                            case 1:
                                date = ApplyPayments(unitRentContract, yearlyRent / unitRentContract.leasePeriodInMonthes.Value, unitRentContract.leasePeriodInMonthes.Value, 1);
                                break;
                            case 2:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 3), unitRentContract.leasePeriodInMonthes.Value / 3, 3);
                                break;
                            case 3:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 4), unitRentContract.leasePeriodInMonthes.Value / 4, 4);
                                break;
                            case 4:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 6), unitRentContract.leasePeriodInMonthes.Value / 6, 6);
                                break;
                            case 5:
                                date = ApplyPayments(unitRentContract, yearlyRent / (unitRentContract.leasePeriodInMonthes.Value / 12), unitRentContract.leasePeriodInMonthes.Value / 12, 12);
                                break;
                        }
                        var oldContract = _context.TUnitRentContract.Include(t => t.mCompoundUnits).SingleOrDefault(c => c.contractNumber == PrevContractNum);
                        var unit = oldContract.mCompoundUnits;
                        if (unit != null)
                        {
                            unit.isRented = false;
                            unit.UnitRentContractID = null;
                        }
                        oldContract.Archived = true;
                        _context.TUnitRentContract.Update(oldContract);

                        if (unitRentContract.VerifiedFromGovernment)
                            unitRentContract.NotVerifiedReason = null;
                        var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.CompoundUnitID == unitRentContract.IdUnitCompound);
                        if (elctricityData == null)
                            elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { CompoundUnitID = unitRentContract.IdUnitCompound }).Entity;
                        elctricityData.PaymentNumber = unitRentContract.PaymentNumber;
                        elctricityData.ElectricityMeterNumber = unitRentContract.ElectricityNumber;
                        elctricityData.StartOfMeter = unitRentContract.MeterStart;
                        elctricityData.TransferTheAccountToTenant = unitRentContract.TransferToTenanat;
                        unitRentContract.Renewed = true;
                        _context.Add(unitRentContract);
                        //unitRentContract.Prev.Archived = true;
                        //_context.Update(unitRentContract.Prev);
                        await _context.SaveChangesAsync();
                        unit.UnitRentContractID = unitRentContract.IdRentContract;
                        unit.isRented = true;
                        _context.SaveChanges();
                        //await setRentInfoInAllUnits();
                        UpdateLastPayment(unitRentContract.IdRentContract, date);
                        TempData["success"] = "Operation completed successfully";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UnitRentContractExists(unitRentContract.IdRentContract))
                        {
                            return NotFound();
                        }
                        base.ViewData["AlertSaveErr"] = "There is an Error Savng the contract. Please correct and try again.";
                    }
                    return RedirectToAction("Index");
                }
                base.ViewData["isRentedErrMsg"] = "Please Select Unit ";
            }
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            await (from m in _context.TCompoundBuildings
                   select new
                   {
                       IdBuilding = m.IdBuilding,
                       IdCompound = m.IdCompound,
                       BuildingName = m.BuildingNumber,
                       BuildingAddress = m.BuildingAddress,
                       DistrictName = m.mCompound.compoundName,
                       buildingInfo = string.Concat(string.Concat(string.Concat(m.BuildingNumber + 45, m.BuildingAddress), 45), m.mCompound.compoundName)
                   } into m
                   where false
                   select m).ToListAsync();
            var building = (!(User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant"))) ? (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                      select new
                                                                                                                                      {
                                                                                                                                          IdBuilding = m.IdBuilding,
                                                                                                                                          IdCompound = m.IdCompound,
                                                                                                                                          BuildingName = m.BuildingNumber,
                                                                                                                                          BuildingAddress = m.BuildingAddress,
                                                                                                                                          DistrictName = m.mCompound.compoundName,
                                                                                                                                          buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                      } into m
                                                                                                                                      where (from l in _context.TCompoundsUsers
                                                                                                                                             select new
                                                                                                                                             {
                                                                                                                                                 l.IdCompound,
                                                                                                                                                 l.IdUser
                                                                                                                                             } into p
                                                                                                                                             where p.IdUser == _user.GetUserId(HttpContext.User)
                                                                                                                                             select p into j
                                                                                                                                             select j.IdCompound).Contains(m.IdCompound)
                                                                                                                                      orderby m.DistrictName, m.BuildingName
                                                                                                                                      select m).ToListAsync()) : (await (from m in _context.TCompoundBuildings.Include((CompoundBuildings m) => m.mCompound)
                                                                                                                                                                         select new
                                                                                                                                                                         {
                                                                                                                                                                             IdBuilding = m.IdBuilding,
                                                                                                                                                                             IdCompound = m.IdCompound,
                                                                                                                                                                             BuildingName = m.BuildingNumber,
                                                                                                                                                                             BuildingAddress = m.BuildingAddress,
                                                                                                                                                                             DistrictName = m.mCompound.compoundName,
                                                                                                                                                                             buildingInfo = string.Concat(m.BuildingNumber, " - ", m.mCompound.compoundName)
                                                                                                                                                                         } into m
                                                                                                                                                                         orderby m.DistrictName, m.BuildingName
                                                                                                                                                                         select m).ToListAsync());
            if (unitRentContract.IdCompound.HasValue)
                building = building.Where(b => b.IdCompound == unitRentContract.IdCompound).ToList();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo");
            base.ViewData["IdUnitCompound"] = new SelectList(_context.TCompoundUnits.Where((CompoundUnits m) => m.IdBuilding == unitRentContract.mCompoundUnits.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnitCompound);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"} };
            return View(unitRentContract);

        }

        #region Payment
        public IActionResult Invoice(int InvoiceId)
        {
            var writer = new QRCodeWriter();
            var resultibt = writer.encode(ConstantValue.BaseUrl + ConstantValue.globalInvoicePath + EncryptString(InvoiceId.ToString(), ConstantValue.EncriptionKey), BarcodeFormat.QR_CODE, 200, 200);

            var matrix = resultibt;
            int scale = 2;

            Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int x = 0; x < matrix.Height; x++)
            {
                for (int y = 0; y < matrix.Width; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < scale; i++)
                        for (int j = 0; j < scale; j++)
                            result.SetPixel(x * scale + i, y * scale + j, pixel);
                }
            }
            string WebRootPath = _env.WebRootPath;
            result.Save(WebRootPath + "\\QRs\\QR" + InvoiceId + ".png");
            ViewBag.Url = "\\QRs\\QR" + InvoiceId + ".png";
            Invoices Model = _context.Invoices.
                Include(x => x.invoiceRelatedPaymentDates)
                .ThenInclude(x => x.unitRentContractPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mCompoundUnits)
                .ThenInclude(x => x.mCompoundBuilding)
                .ThenInclude(x => x.mCompound)
                .Include(x => x.invoiceRelatedPaymentDates)
                .ThenInclude(x => x.unitRentContractPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mCompoundUnits)
                .ThenInclude(x => x.mCompoundBuilding)
                .ThenInclude(x => x.mCompoundUnits)
                .Include(x => x.invoiceRelatedPaymentDates)
                .ThenInclude(x => x.unitRentContractPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mTenant)
                .ThenInclude(x => x.mCompany)
                .FirstOrDefault(x => x.Id == InvoiceId);
            return View("Invoice_Compound", Model);
        }


        public IActionResult Payment(int id)
        {
            RentContractPaymentDTO payments = new RentContractPaymentDTO
            {
                RentContractPaymentList = _context.TUnitRentContractPayments
                                  .Where(c => c.UnitRentContractID == id)
                                  .OrderBy(c => c.DueDate)
                                  .Select(p => new RentContractPayment
                                  {
                                      ID = p.ID,
                                      RemainingAmount = p.RemainingAmount,
                                      PaidAmount = p.PaidAmount,
                                      Paid = p.Paid,
                                      Amount = p.Amount,
                                      DueDate = p.DueDate,
                                      Note = p.Note,
                                      User = p.User != null ? p.User.fullName : string.Empty,
                                      PaymentDate = p.PaymentDate,
                                      TenantName = p.UnitRentContract.mTenant.tenantName,

                                      UnitNumber = p.UnitRentContract.IdUnit != null
                                          ? p.UnitRentContract.mUnit.UnitNumber
                                          : p.UnitRentContract.mCompoundUnits.UnitNumber,

                                      BuldingNumber = p.UnitRentContract.IdUnit != null
                                          ? p.UnitRentContract.mUnit.mBuilding.BuildingName
                                          : p.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber,
                                  }).ToList(),
                RentContractOtherPaymentList = _context.unitRentContractOtherPayment
                                   .Where(c => c.UnitRentContractID == id)
                                   .OrderBy(c => c.PaymentDate)
                                   .Select(p => new RentContractOtherPayment
                                   {
                                       ID = p.ID,
                                       Commession = p.Commession,
                                       Insurence = p.Insurence,
                                       OtherPayment = p.OtherPayment,
                                       MonyType = p.MonyType,
                                       PaidAmount = p.PaidAmount,
                                       Note = p.Note,
                                       User = p.User != null ? p.User.fullName : string.Empty,
                                       PaymentDate = p.PaymentDate,
                                       TenantName = p.UnitRentContract.mTenant.tenantName,
                                       UnitNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.UnitNumber
                                           : p.UnitRentContract.mCompoundUnits.UnitNumber,

                                       BuldingNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.mBuilding.BuildingName
                                           : p.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber,
                                       pageid = p.UnitRentContract.mMasterBuilding,
                                   }).ToList(),
                InvoicesList = _context.Invoices.Include(x => x.UnitRentContractOtherPayment).Where(x => x.ContractId == id && x.Status == null).ToList()
            };

            if (payments.RentContractPaymentList.Count() == 0 || payments == null)
            {
                return NotFound();
            }
            ViewBag.ContractId = id;
            return View(payments);
        }
        #region otherpayment
        public IActionResult EditOtherPayment(int OtherPaymentId)
        {
            RentContractOtherPayment otherPayment = _context.unitRentContractOtherPayment
                                   .Where(c => c.ID == OtherPaymentId)
                                   .OrderBy(c => c.PaymentDate)
                                   .Select(p => new RentContractOtherPayment
                                   {
                                       ID = p.ID,
                                       UnitRentContractID = p.UnitRentContractID,
                                       Commession = p.Commession,
                                       Insurence = p.Insurence,
                                       OtherPayment = p.OtherPayment,
                                       OtherPaymentText = p.OtherPaymentText,
                                       PaidAmount = p.PaidAmount,
                                       Note = p.Note,
                                       User = p.User != null ? p.User.fullName : string.Empty,
                                       PaymentDate = p.PaymentDate,
                                       TenantName = p.UnitRentContract.mTenant.tenantName,
                                       UnitNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.UnitNumber
                                           : p.UnitRentContract.mCompoundUnits.UnitNumber,

                                       BuldingNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.mBuilding.BuildingName
                                           : p.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber,
                                       pageid = p.UnitRentContract.mMasterBuilding,
                                   }).FirstOrDefault();
            if (otherPayment == null)
            {
                return NotFound();
            }

            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectList<Enums.PaymentMehtod>();
            ViewData["MonyType"] = Enums.GetEnumAsSelectList<Enums.MonyType>();
            return View(otherPayment);
        }

        [HttpPost]
        public IActionResult EditOtherPayment(RentContractOtherPayment model)
        {

            UnitRentContractOtherPayment otherPayment = _context.unitRentContractOtherPayment
                                   .Where(c => c.ID == model.ID).FirstOrDefault();
            otherPayment.Insurence = model.Insurence;
            otherPayment.Commession = model.Commession;
            otherPayment.OtherPayment = model.OtherPayment;
            otherPayment.OtherPaymentText = model.OtherPaymentText;
            otherPayment.PaidAmount = model.Insurence + model.Commession + model.OtherPayment;
            otherPayment.MonyType = model.MonyType;
            otherPayment.Note = model.Note;
            _context.Update(otherPayment);
            Invoices InvoicesEntity = _context.Invoices
                                   .Where(c => c.UnitRentContractOtherPayment.ID == model.ID).FirstOrDefault();
            InvoicesEntity.Payment = model.Insurence + model.Commession + model.OtherPayment;
            InvoicesEntity.checkVisaNumber = model.checkVisaNumber;
            InvoicesEntity.PaymentMehtod = model.PaymentMehtod;
            _context.Update(InvoicesEntity);
            _context.SaveChanges();
            return RedirectToAction("Payment", new { id = model.UnitRentContractID });
        }

        public IActionResult DeleteOtherPayment(int OtherPaymentId)
        {
            RentContractOtherPayment otherPayment = _context.unitRentContractOtherPayment
                                   .Where(c => c.ID == OtherPaymentId)
                                   .OrderBy(c => c.PaymentDate)
                                   .Select(p => new RentContractOtherPayment
                                   {
                                       ID = p.ID,
                                       UnitRentContractID = p.UnitRentContractID,
                                       Commession = p.Commession,
                                       Insurence = p.Insurence,
                                       OtherPayment = p.OtherPayment,
                                       OtherPaymentText = p.OtherPaymentText,
                                       PaidAmount = p.PaidAmount,
                                       Note = p.Note,
                                       User = p.User != null ? p.User.fullName : string.Empty,
                                       PaymentDate = p.PaymentDate,
                                       TenantName = p.UnitRentContract.mTenant.tenantName,
                                       UnitNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.UnitNumber
                                           : p.UnitRentContract.mCompoundUnits.UnitNumber,

                                       BuldingNumber = p.UnitRentContract.IdUnit != null
                                           ? p.UnitRentContract.mUnit.mBuilding.BuildingName
                                           : p.UnitRentContract.mCompoundUnits.mCompoundBuilding.BuildingNumber,
                                       pageid = p.UnitRentContract.mMasterBuilding,
                                   }).FirstOrDefault();
            if (otherPayment == null)
            {
                return NotFound();
            }

            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectList<Enums.PaymentMehtod>();
            ViewData["MonyType"] = Enums.GetEnumAsSelectList<Enums.MonyType>();
            return View(otherPayment);
        }

        [HttpPost]
        public IActionResult DeleteOtherPayment(RentContractOtherPayment model)
        {

            UnitRentContractOtherPayment otherPayment = _context.unitRentContractOtherPayment
                                   .Where(c => c.ID == model.ID).FirstOrDefault();

            _context.Remove(otherPayment);
            Invoices InvoicesEntity = _context.Invoices
                                   .Where(c => c.UnitRentContractOtherPayment.ID == model.ID).FirstOrDefault();
            InvoicesEntity.Status = 1;
            _context.Update(InvoicesEntity);
            _context.SaveChanges();
            return RedirectToAction("Payment", new { id = model.UnitRentContractID });
        }

        public IActionResult PartiallyPayOther(int ContractId)
        {
            var payment = _context.TUnitRentContract.Where(x => x.IdRentContract == ContractId)
                    .Include(e => e.UnitRentContractPayments)
                    .Include(e => e.invoices)
                    .Include(e => e.mTenant)
                    .Include(e => e.mCompound)
                    .Include(e => e.mCompoundUnits)
                    .Include(e => e.mUnit)
                    .Include(e => e.mUnit.mBuilding)
                    .Select(p => new RentContractOtherPayment()
                    {
                        UnitRentContractID = p.IdRentContract,
                        pageid = p.mMasterBuilding,
                        TenantName = p.mTenant.tenantName,
                        UnitNumber = p.IdUnit != null
                                       ? p.mUnit.UnitNumber
                                       : p.mCompoundUnits.UnitNumber,
                        BuldingNumber = p.IdUnit != null
                                       ? p.mUnit.mBuilding.BuildingName
                                       : p.mCompound.compoundName,
                        InvoiceId = "E-" + (_context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault() == null ? 5000 : _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault().InvoiceId + 1)
                    }).FirstOrDefault();
            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectList<Enums.PaymentMehtod>();
            ViewData["MonyType"] = Enums.GetEnumAsSelectList<Enums.MonyType>();
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        [HttpPost]
        public IActionResult PartiallyPayOther(RentContractOtherPayment model)
        {
            UnitRentContractOtherPayment otherpayment = new UnitRentContractOtherPayment();
            otherpayment.Commession = model.Commession;
            otherpayment.Insurence = model.Insurence;
            otherpayment.OtherPayment = model.OtherPayment;
            otherpayment.PaidAmount = model.ApplyTax == true ? (model.Commession + model.Insurence + model.OtherPayment) : 0;
            otherpayment.MonyType = model.MonyType;
            otherpayment.PaymentDate = DateTime.Now;
            otherpayment.UnitRentContractID = model.UnitRentContractID;
            otherpayment.Note = model.Note;
            otherpayment.UserID = _user.GetUserId(base.HttpContext.User);
            otherpayment.OtherPaymentText = model.OtherPaymentText;
            otherpayment.ApplyTax = model.ApplyTax;
            _context.Add(otherpayment);

            Invoices InvoicesEntity = new Invoices();
            InvoicesEntity.ContractId = model.UnitRentContractID;
            InvoicesEntity.Payment = model.ApplyTax == true ? (model.Commession + model.Insurence + model.OtherPayment) : 0;
            InvoicesEntity.PaymentDate = DateTime.Now;
            InvoicesEntity.PaymentMehtod = model.PaymentMehtod;
            InvoicesEntity.checkVisaNumber = model.checkVisaNumber;
            InvoicesEntity.Status = null;
            InvoicesEntity.isOtherPayment = true;
            InvoicesEntity.UnitRentContractOtherPayment = otherpayment;
            InvoicesEntity.InvoiceId = _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault() == null ? 5000 : _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault().InvoiceId + 1;
            _context.Add(InvoicesEntity);



            //UnitRentContractAllPaymentLogs unitRentContractAllPaymentLogs = new UnitRentContractAllPaymentLogs();
            //unitRentContractAllPaymentLogs.Action = "Add (Other) Payment";
            //unitRentContractAllPaymentLogs.AllPaidAmount = model.Commession + model.Insurence + model.Insurence;
            //unitRentContractAllPaymentLogs.UnitRentContractID = model.UnitRentContractID;
            //unitRentContractAllPaymentLogs.PaymentDate = DateTime.Now;
            //unitRentContractAllPaymentLogs.UserID = _user.GetUserId(base.HttpContext.User);
            //unitRentContractAllPaymentLogs.InvoiceID = InvoicesEntity.Id;
            //_context.Add(unitRentContractAllPaymentLogs);

            //var payment = _context.TUnitRentContractPayments
            //    .Where(m => m.UnitRentContractID == model.UnitRentContractID && m.Paid == false)
            //    .Include(m => m.UnitRentContract).OrderBy(x => x.DueDate).ToList();
            //foreach (var pay in payment)
            //{
            //    int PaidValue = pay.RemainingAmount >= model.PaidAmount ? model.PaidAmount : pay.RemainingAmount;
            //    pay.PaidAmount += PaidValue;
            //    pay.Paid = pay.PaidAmount == pay.Amount;
            //    pay.UnitRentContract.remainingAmount -= PaidValue;
            //    pay.UnitRentContract.paidAmount += PaidValue;
            //    pay.Note = model.Note;
            //    pay.PaymentDate = DateTime.Now;
            //    pay.UserID = _user.GetUserId(base.HttpContext.User);
            //    pay.UnitRentContractPaymentLogs.Add(new UnitRentContractPaymentLog
            //    {
            //        PaymentDate = DateTime.Now,
            //        PaidAmount = PaidValue,
            //        UserID = _user.GetUserId(base.HttpContext.User),
            //    });

            //    InvoiceRelatedPaymentDates InvoiceRelatedPaymentDatesEntity = new InvoiceRelatedPaymentDates();
            //    InvoiceRelatedPaymentDatesEntity.Amount = PaidValue;
            //    InvoiceRelatedPaymentDatesEntity.InvoiceId = InvoicesEntity.Id;
            //    InvoiceRelatedPaymentDatesEntity.UnitRentContractPaymentId = pay.ID;
            //    InvoiceRelatedPaymentDatesEntity.PaymentState = pay.Paid;
            //    _context.Add(InvoiceRelatedPaymentDatesEntity);
            //    model.PaidAmount -= PaidValue;
            //    if (model.PaidAmount == 0)
            //    {
            //        break;
            //    }
            //}
            _context.SaveChanges();
            return RedirectToAction("Payment", new { id = model.UnitRentContractID });
        }

        public IActionResult OtherInvoice(int InvoiceId)
        {
            var writer = new QRCodeWriter();
            var resultibt = writer.encode(ConstantValue.BaseUrl + ConstantValue.globalOtherInvoicePath + EncryptString(InvoiceId.ToString(), ConstantValue.EncriptionKey), BarcodeFormat.QR_CODE, 200, 200);

            var matrix = resultibt;
            int scale = 2;

            Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int x = 0; x < matrix.Height; x++)
            {
                for (int y = 0; y < matrix.Width; y++)
                {
                    Color pixel = matrix[x, y] ? Color.Black : Color.White;
                    for (int i = 0; i < scale; i++)
                        for (int j = 0; j < scale; j++)
                            result.SetPixel(x * scale + i, y * scale + j, pixel);
                }
            }
            string WebRootPath = _env.WebRootPath;
            result.Save(WebRootPath + "\\QRs\\QR" + InvoiceId + ".png");
            ViewBag.Url = "\\QRs\\QR" + InvoiceId + ".png";
            Invoices Model = _context.Invoices.Include(x => x.UnitRentContractOtherPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mCompoundUnits)
                .ThenInclude(x => x.mCompoundBuilding)
                .ThenInclude(x => x.mCompound)
                .Include(x => x.UnitRentContractOtherPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mCompoundUnits)
                .ThenInclude(x => x.mCompoundBuilding)
                .ThenInclude(x => x.mCompoundUnits)
                .Include(x => x.UnitRentContractOtherPayment)
                .ThenInclude(x => x.UnitRentContract)
                .ThenInclude(x => x.mTenant)
                .ThenInclude(x => x.mCompany)
                .FirstOrDefault(x => x.UnitRentContractOtherPayment.ID == InvoiceId);
            return View("OtherInvoice_Compound", Model);
        }
        #endregion
        //public IActionResult Pay(int id)
        //{
        //    var payment = _context.TUnitRentContractPayments.Where(m => m.ID == id).Include(m => m.UnitRentContract).FirstOrDefault();
        //    var userId = _user.GetUserId(base.HttpContext.User);
        //    payment.UnitRentContract.remainingAmount -= payment.RemainingAmount;
        //    payment.UnitRentContract.paidAmount += payment.RemainingAmount;
        //    payment.PaidAmount += payment.RemainingAmount;
        //    payment.Paid = payment.PaidAmount == payment.Amount;
        //    payment.PaymentDate = DateTime.Now;
        //    payment.UnitRentContractPaymentLogs.Add(new UnitRentContractPaymentLog
        //    {
        //        PaymentDate = DateTime.Now,
        //        PaidAmount = payment.RemainingAmount
        //    });
        //    payment.UserID = _user.GetUserId(base.HttpContext.User);
        //    _context.SaveChanges();
        //    setLastPayment(payment.UnitRentContractID).Wait();
        //    return RedirectToAction("Payment", new { id = payment.UnitRentContractID });
        //}

        public IActionResult PartiallyPay(int ContractId)
        {
            var payment = _context.TUnitRentContract.Where(x => x.IdRentContract == ContractId)
                     .Include(e => e.mTenant)
                     .Include(e => e.mCompound)
                     .Include(e => e.mCompoundUnits)
                     .Include(e => e.mUnit)
                     .Include(e => e.mUnit.mBuilding).Include(e => e.UnitRentContractPayments)
                     .Select(p => new RentContractPayment()
                     {
                         DeservedAmount = p.UnitRentContractPayments.Where(x => x.DueDate < DateTime.Now).Count() == 0 ? 0 : p.UnitRentContractPayments.Where(x => x.DueDate < DateTime.Now).Sum(x => x.RemainingAmount),
                         TotalPaymentValue = p.remainingAmount,
                         UnitRentContractID = p.IdRentContract,
                         pageid = p.mMasterBuilding,
                         TenantName = p.mTenant.tenantName,

                         UnitNumber = p.IdUnit != null
                                        ? p.mUnit.UnitNumber
                                        : p.mCompoundUnits.UnitNumber,

                         BuldingNumber = p.IdUnit != null
                                        ? p.mUnit.mBuilding.BuildingName
                                        : p.mCompound.compoundName,
                         PaymentDate = _context.Invoices.Where(x => x.ContractId == ContractId && x.Status != 0).OrderByDescending(x => x.InvoiceId).Select(x => x.PaymentDate).FirstOrDefault(),
                         InvoiceId = "E-" + (_context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault() == null ? 5000 : _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault().InvoiceId + 1)
                     }).FirstOrDefault();
            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectList<Enums.PaymentMehtod>();
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }
        [HttpPost]
        public IActionResult PartiallyPay(RentContractPayment model)
        {
            Invoices InvoicesEntity = new Invoices();
            InvoicesEntity.ContractId = model.UnitRentContractID;
            InvoicesEntity.Payment = model.PaidAmount;
            InvoicesEntity.PaymentDate = model.PaymentDate ?? DateTime.Now;
            InvoicesEntity.PaymentMehtod = model.PaymentMehtod;
            InvoicesEntity.checkVisaNumber = model.checkVisaNumber;
            InvoicesEntity.Status = null;
            //InvoicesEntity.InvoiceId = long.Parse(model.InvoiceId);
            InvoicesEntity.InvoiceId = _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault() == null ? 5000 : _context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefault().InvoiceId + 1;
            _context.Add(InvoicesEntity);
            UnitRentContractAllPaymentLogs unitRentContractAllPaymentLogs = new UnitRentContractAllPaymentLogs();
            unitRentContractAllPaymentLogs.Action = "Add Payment";
            unitRentContractAllPaymentLogs.AllPaidAmount = model.PaidAmount;
            unitRentContractAllPaymentLogs.UnitRentContractID = model.UnitRentContractID;
            unitRentContractAllPaymentLogs.PaymentDate = model.PaymentDate ?? DateTime.Now;
            unitRentContractAllPaymentLogs.UserID = _user.GetUserId(base.HttpContext.User);
            unitRentContractAllPaymentLogs.InvoiceID = InvoicesEntity.Id;
            _context.Add(unitRentContractAllPaymentLogs);

            var payment = _context.TUnitRentContractPayments
                .Where(m => m.UnitRentContractID == model.UnitRentContractID && m.Paid == false)
                .Include(m => m.UnitRentContract).OrderBy(x => x.DueDate).ToList();

            foreach (var pay in payment)
            {
                var taxtoProperties = _context.taxToProperttypes.Include(x => x.TaxRate).Where(x => x.PropertyTypesId == model.PropertyTypeId && pay.DueDate >= x.TaxRate.StartApplingDate && (x.TaxRate.EndApplingDate == null || pay.DueDate <= x.TaxRate.StartApplingDate));
                int PaidValue = pay.RemainingAmount >= model.PaidAmount ? model.PaidAmount : pay.RemainingAmount;
                pay.PaidAmount += PaidValue;
                pay.Paid = pay.PaidAmount == pay.Amount;
                pay.UnitRentContract.remainingAmount -= PaidValue;
                pay.UnitRentContract.paidAmount += PaidValue;
                pay.Note = model.Note;
                pay.PaymentDate = model.PaymentDate ?? DateTime.Now;
                pay.UserID = _user.GetUserId(base.HttpContext.User);
                pay.UnitRentContractPaymentLogs.Add(new UnitRentContractPaymentLog
                {
                    PaymentDate = model.PaymentDate ?? DateTime.Now,
                    PaidAmount = PaidValue,
                    UserID = _user.GetUserId(base.HttpContext.User)
                });
                InvoiceRelatedPaymentDates InvoiceRelatedPaymentDatesEntity = new InvoiceRelatedPaymentDates();
                InvoiceRelatedPaymentDatesEntity.Amount = PaidValue;
                InvoiceRelatedPaymentDatesEntity.InvoiceId = InvoicesEntity.Id;
                InvoiceRelatedPaymentDatesEntity.UnitRentContractPaymentId = pay.ID;
                InvoiceRelatedPaymentDatesEntity.PaymentState = pay.Paid;
                InvoiceRelatedPaymentDatesEntity.Status = null;

                if (taxtoProperties != null && taxtoProperties.Count() > 0)
                {
                    TaxToProperttypes taxtoProperty = null;
                    if (taxtoProperties.Count() > 1)
                        taxtoProperty = taxtoProperties.FirstOrDefault(x => x.TaxRate.EndApplingDate != null);
                    else
                        taxtoProperty = taxtoProperties.FirstOrDefault();
                    InvoiceRelatedPaymentDatesEntity.TaxAmount = Math.Round((decimal)(PaidValue * taxtoProperty.TaxRate.Rate / 100), 2);
                    InvoiceRelatedPaymentDatesEntity.TaxRateId = taxtoProperty.TaxRate.Id;

                }
                _context.Add(InvoiceRelatedPaymentDatesEntity);
                model.PaidAmount -= PaidValue;
                if (model.PaidAmount == 0)
                {
                    break;
                }
            }
            var rentContract = _context.TUnitRentContract.Include(x => x.Legal).FirstOrDefault(x => x.IdRentContract == model.UnitRentContractID);

            rentContract.AddedtoCourt = false;
            rentContract.LegalId = null;
            _context.Update(rentContract);
            var legalEntity = rentContract.Legal;
            if (legalEntity != null)
                _context.Legals.Remove(legalEntity);
            _context.SaveChanges();
            return RedirectToAction("Payment", new { id = model.UnitRentContractID });
        }
        [NonAction]
        public static string EncryptString(string text, string keyString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(text);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(keyString, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    text = Convert.ToBase64String(ms.ToArray());
                }
            }
            return text;
        }
        public IActionResult UndoLastPayment(int contractId)
        {
            var Invoice = _context.Invoices.Include(x => x.invoiceRelatedPaymentDates).OrderBy(x => x.PaymentDate).LastOrDefault(x => x.ContractId == contractId && x.Status == null);
            UnitRentContractAllPaymentLogs unitRentContractAllPaymentLogs = new UnitRentContractAllPaymentLogs();
            if (Invoice != null)
            {
                Invoice.Status = 0;

                unitRentContractAllPaymentLogs.Action = "undo last Payment";
                unitRentContractAllPaymentLogs.UnitRentContractID = contractId;
                unitRentContractAllPaymentLogs.PaymentDate = DateTime.Now;
                unitRentContractAllPaymentLogs.UserID = _user.GetUserId(base.HttpContext.User);
                unitRentContractAllPaymentLogs.InvoiceID = Invoice.Id;

                var invoiceRelatedPaymentDates = Invoice.invoiceRelatedPaymentDates.Where(x => x.Status == null).Reverse().ToList();
                if (invoiceRelatedPaymentDates.Count() > 0)
                {

                    foreach (var invoiceRelatedPaymentDate in invoiceRelatedPaymentDates)
                    {
                        var pay = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                                    .Include(p => p.UnitRentContractPaymentLogs)
                                                                    .Where(c => c.ID == invoiceRelatedPaymentDate.UnitRentContractPaymentId).FirstOrDefault();
                        unitRentContractAllPaymentLogs.AllPaidAmount += (int)invoiceRelatedPaymentDate.Amount;
                        pay.UnitRentContract.remainingAmount += (int)invoiceRelatedPaymentDate.Amount;
                        pay.UnitRentContract.paidAmount -= (int)invoiceRelatedPaymentDate.Amount;
                        pay.PaidAmount -= (int)invoiceRelatedPaymentDate.Amount;
                        if (pay.PaidAmount < pay.Amount)
                        {
                            pay.Paid = false;
                            pay.PaymentDate = null;
                            pay.UserID = null;
                            pay.Note = null;
                        }
                        _context.UnitRentContractPaymentLogs.RemoveRange(pay.UnitRentContractPaymentLogs.ToList());
                        invoiceRelatedPaymentDate.Status = 0;
                    }
                    _context.Invoices.Update(Invoice);
                    _context.InvoiceRelatedPaymentDates.UpdateRange(Invoice.invoiceRelatedPaymentDates);
                    _context.Add(unitRentContractAllPaymentLogs);

                }
            }
            else
            {
                var payment = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                                           .Include(p => p.UnitRentContractPaymentLogs)
                                                                           .Where(c => c.PaidAmount > 0 && c.UnitRentContractID == contractId)
                                                                           .OrderByDescending(p => p.DueDate)
                                                                           .FirstOrDefault();
                payment.UnitRentContract.remainingAmount += payment.PaidAmount;
                payment.UnitRentContract.paidAmount -= payment.PaidAmount;
                payment.PaidAmount = 0;
                payment.PaymentDate = null;
                payment.UserID = null;
                payment.Note = null;
                payment.Paid = false;
                _context.UnitRentContractPaymentLogs.RemoveRange(payment.UnitRentContractPaymentLogs.ToList());
            }
            _context.SaveChanges();
            return RedirectToAction("Payment", new { id = contractId });
        }
        public IActionResult UndoAllPayments(int contractId)
        {
            var Invoice = _context.Invoices.Where(x => x.ContractId == contractId && x.Status == null).Include(x => x.invoiceRelatedPaymentDates);

            if (Invoice != null)
            {

                var invoiceRelatedPaymentDatesEntity = Invoice.FirstOrDefault().invoiceRelatedPaymentDates.ToList();
                Invoice.ToList().ForEach(x =>
                {
                    UnitRentContractAllPaymentLogs unitRentContractAllPaymentLogs = new UnitRentContractAllPaymentLogs();
                    unitRentContractAllPaymentLogs.Action = "undo All Payment for( ";
                    Invoice.Select(z => z.Id).ToList().ForEach(z => { unitRentContractAllPaymentLogs.Action = unitRentContractAllPaymentLogs.Action + z.ToString() + " "; });
                    unitRentContractAllPaymentLogs.Action = unitRentContractAllPaymentLogs.Action + ")";
                    unitRentContractAllPaymentLogs.UnitRentContractID = contractId;
                    unitRentContractAllPaymentLogs.PaymentDate = DateTime.Now;
                    unitRentContractAllPaymentLogs.UserID = _user.GetUserId(base.HttpContext.User);
                    unitRentContractAllPaymentLogs.InvoiceID = x.Id;
                    unitRentContractAllPaymentLogs.AllPaidAmount = (int)x.Payment;
                    _context.Add(unitRentContractAllPaymentLogs);

                    x.Status = 0;
                });
                invoiceRelatedPaymentDatesEntity.ForEach(x =>
                {
                    x.Status = 0;

                });
                if (invoiceRelatedPaymentDatesEntity.Count() > 0)
                {
                    _context.InvoiceRelatedPaymentDates.UpdateRange(invoiceRelatedPaymentDatesEntity);
                    _context.Invoices.UpdateRange(Invoice);

                }
            }

            var payments = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract).Include(p => p.UnitRentContractPaymentLogs).Where(c => c.PaidAmount > 0 && c.UnitRentContractID == contractId).ToList();
            foreach (var payment in payments)
            {
                payment.UnitRentContract.remainingAmount += payment.PaidAmount;
                payment.UnitRentContract.paidAmount -= payment.PaidAmount;
                payment.PaidAmount = 0;
                payment.PaymentDate = null;
                payment.UserID = null;
                payment.Note = null;
                payment.Paid = false;
                _context.UnitRentContractPaymentLogs.RemoveRange(payment.UnitRentContractPaymentLogs.ToList());
            }
            _context.SaveChanges();
            UpdateLastPayment(contractId, GetLastPaymentData(payments.FirstOrDefault().UnitRentContract.paymentMethod, payments.FirstOrDefault().UnitRentContract.dtLeaseStart));
            return RedirectToAction("Payment", new { id = contractId });
        }
        private DateTime GetLastPaymentData(int paymentMethod, DateTime comparedDate)
        {
            DateTime date = new DateTime();
            switch (paymentMethod)
            {
                case 1:
                    date = comparedDate.AddMonths(-1);
                    break;
                case 2:
                    date = comparedDate.AddMonths(-3);
                    break;
                case 3:
                    date = comparedDate.AddMonths(-4);
                    break;
                case 4:
                    date = comparedDate.AddMonths(-6);
                    break;
                case 5:
                    date = comparedDate.AddMonths(-12);
                    break;
                default:
                    break;
            }
            return date;
        }
        private int PaidAmount(int rent, int paid)
        {
            if (rent <= paid)
                return rent;
            if (paid > 0)
                return paid;
            else return 0;
        }
        private DateTime ApplyPayments(UnitRentContract unitRentContract, int rent, int paymentsCount, int incrmentStep)
        {
            var InvoicesForSameContract = _context.Invoices.Where(x => x.ContractId == unitRentContract.IdRentContract && x.Status == null);


            var payments = _context.TUnitRentContractPayments.Include(p => p.UnitRentContractPaymentLogs).Where(c => c.UnitRentContractID == unitRentContract.IdRentContract).ToList();
            _context.UnitRentContractPaymentLogs.RemoveRange(payments.SelectMany(p => p.UnitRentContractPaymentLogs).ToList());
            _context.TUnitRentContractPayments.RemoveRange(payments);
            var modifiedBy = _user.GetUserId(HttpContext.User);
            var paidAmount = unitRentContract.paidAmount;
            var month = 0;
            int remainingLastContact = unitRentContract.LastContractRemainingAmount;
            int payedFromRemainingLastContact = 0;
            if (unitRentContract.LastContractRemainingAmount > 0)
            {
                if (unitRentContract.LastContractRemainingAmount >= paidAmount)
                {
                    //remainingLastContact = unitRentContract.LastContractRemainingAmount - paidAmount;
                    payedFromRemainingLastContact = paidAmount;
                    paidAmount = 0;
                }
                else
                {
                    paidAmount = paidAmount - unitRentContract.LastContractRemainingAmount;
                    payedFromRemainingLastContact = unitRentContract.LastContractRemainingAmount;
                }
            }
            unitRentContract.UnitRentContractPayments.Clear();
            for (int i = 0; i < paymentsCount; i++)
            {
                var coverAllRent = rent <= paidAmount;
                var rentValue = PaidAmount(rent, paidAmount);
                unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                {
                    Amount = i == paymentsCount - 1 ? (unitRentContract.yearlyRent - rent * i) + remainingLastContact : rent + remainingLastContact,
                    DueDate = unitRentContract.dtLeaseStart.AddMonths(month),
                    PaidAmount = rentValue + payedFromRemainingLastContact,
                    Paid = coverAllRent,
                    UserID = modifiedBy,
                    PaymentDate = InvoicesForSameContract.Count() != 0 ? InvoicesForSameContract.LastOrDefault().PaymentDate : DateTime.Now,
                    Note = (remainingLastContact > 0) ? $"Êã ÇÖÇÝÉ ãÊÃÎÑÇÊ ÇáÚÞÏ ÇáÓÇÈÞ ÈÞíãÉ({ remainingLastContact }) æÊã ÏÝÚ ãäåÇ({ payedFromRemainingLastContact}) æÇáãÊÈÞí Úáíå ãä ÇáãÊÃÎÑÇÊ({remainingLastContact - payedFromRemainingLastContact})" : "",
                    UnitRentContractPaymentLogs = new List<UnitRentContractPaymentLog>
                    {
                        new UnitRentContractPaymentLog
                        {
                            PaidAmount=rentValue + payedFromRemainingLastContact,
                            PaymentDate=InvoicesForSameContract.Count() != 0? InvoicesForSameContract.LastOrDefault().PaymentDate: DateTime.Now,
                            UserID=modifiedBy
                        }
                    }
                });
                paidAmount -= rentValue;
                month += incrmentStep;
                remainingLastContact = 0;
                payedFromRemainingLastContact = 0;
            }


            int counter = 0;
            decimal RemainUnitRentContractAmount = 0;
            var invoiceRelatedPaymentDates = _context.InvoiceRelatedPaymentDates.Include(x => x.unitRentContractPayment).Where(x => x.unitRentContractPayment.UnitRentContractID == unitRentContract.IdRentContract).ToList();
            invoiceRelatedPaymentDates.ToList().ForEach(x => x.Status = 0);
            foreach (var invoice in InvoicesForSameContract)
            {
                decimal invoicePayment = invoice.Payment;
                UnitRentContractPayment NewUnitRentContract = unitRentContract.UnitRentContractPayments.OrderBy(x => x.DueDate).ElementAt(counter);
                NewUnitRentContract.InvoiceRelatedPaymentDates = NewUnitRentContract.InvoiceRelatedPaymentDates ?? new List<InvoiceRelatedPaymentDates>();
                if (RemainUnitRentContractAmount == 0)
                {
                    RemainUnitRentContractAmount = NewUnitRentContract.Amount;
                }

                if (invoicePayment == RemainUnitRentContractAmount)
                {
                    NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                    {
                        Amount = invoicePayment,
                        InvoiceId = invoice.Id,
                        PaymentState = true
                    });
                    RemainUnitRentContractAmount = 0;
                    counter++;
                    continue;
                }
                else if (invoicePayment > RemainUnitRentContractAmount)
                {
                    decimal Remaininvoice = 0;
                    NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                    {
                        Amount = RemainUnitRentContractAmount,
                        InvoiceId = invoice.Id,
                        PaymentState = true
                    });
                    Remaininvoice = invoicePayment - RemainUnitRentContractAmount;
                    RemainUnitRentContractAmount = 0;
                    while (Remaininvoice > 0)
                    {
                        counter++;
                        NewUnitRentContract = unitRentContract.UnitRentContractPayments.OrderBy(x => x.DueDate).ElementAt(counter);
                        NewUnitRentContract.InvoiceRelatedPaymentDates = NewUnitRentContract.InvoiceRelatedPaymentDates ?? new List<InvoiceRelatedPaymentDates>();
                        if (RemainUnitRentContractAmount == 0)
                        {
                            RemainUnitRentContractAmount = NewUnitRentContract.Amount;
                        }
                        if (Remaininvoice == RemainUnitRentContractAmount)
                        {
                            NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                            {
                                Amount = RemainUnitRentContractAmount,
                                InvoiceId = invoice.Id,
                                PaymentState = true
                            });
                            RemainUnitRentContractAmount = 0;
                            counter++;
                            break;
                        }

                        else if (Remaininvoice > RemainUnitRentContractAmount)
                        {
                            NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                            {
                                Amount = RemainUnitRentContractAmount,
                                InvoiceId = invoice.Id,
                                PaymentState = true
                            });
                            Remaininvoice = Remaininvoice - RemainUnitRentContractAmount;
                            continue;
                        }

                        else if (Remaininvoice < RemainUnitRentContractAmount)
                        {
                            NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                            {
                                Amount = Remaininvoice,
                                InvoiceId = invoice.Id,
                                PaymentState = false

                            });
                            RemainUnitRentContractAmount = RemainUnitRentContractAmount - Remaininvoice;
                            break;
                        }
                    }


                }
                else if (invoicePayment < RemainUnitRentContractAmount)
                {
                    NewUnitRentContract.InvoiceRelatedPaymentDates.Add(new InvoiceRelatedPaymentDates()
                    {
                        Amount = invoicePayment,
                        InvoiceId = invoice.Id,
                        PaymentState = false
                    });
                    RemainUnitRentContractAmount = RemainUnitRentContractAmount - invoicePayment;
                    continue;
                }
            }

            return unitRentContract.UnitRentContractPayments.Where(d => d.Paid).OrderByDescending(p => p.DueDate).FirstOrDefault()?.DueDate ?? unitRentContract.dtLeaseStart.AddMonths(-incrmentStep);
        }
        #endregion

        #region Notes
        public IActionResult Notes(int id)
        {
            var notes = _context.TUnitRentContractNotes
                                   .Where(c => c.UnitRentContractID == id)
                                   .OrderByDescending(c => c.CreatedOn)
                                   .Select(p => new RentContractNoteDTO
                                   {
                                       ID = p.ID,
                                       CreatedOn = p.CreatedOn,
                                       Note = p.Note,
                                       User = p.User != null ? p.User.fullName : string.Empty
                                   }).ToList();
            if (notes == null)
                return NotFound();

            ViewBag.ContractId = id;
            return View(notes);
        }
        public IActionResult AddNote(int id)
        {
            return View(new RentContractNoteDTO { ContractID = id });
        }
        [HttpPost]
        public IActionResult AddNote(RentContractNoteDTO model)
        {
            _context.TUnitRentContractNotes.Add(new UnitRentContractNote
            {
                CreatedOn = DateTime.Now,
                UserID = _user.GetUserId(HttpContext.User),
                Note = model.Note,
                UnitRentContractID = model.ContractID
            });
            _context.SaveChanges();
            return RedirectToAction("Notes", new { id = model.ContractID });
        }
        public IActionResult DeleteLastNote(int contractId)
        {
            var note = _context.TUnitRentContractNotes.Where(c => c.UnitRentContractID == contractId).OrderByDescending(p => p.CreatedOn).FirstOrDefault();
            _context.TUnitRentContractNotes.Remove(note);
            _context.SaveChanges();
            return RedirectToAction("Notes", new { id = contractId });
        }
        public IActionResult DeleteAllNotes(int contractId)
        {
            var notes = _context.TUnitRentContractNotes.Where(c => c.UnitRentContractID == contractId).ToList();
            _context.TUnitRentContractNotes.RemoveRange(notes);
            _context.SaveChanges();
            return RedirectToAction("Notes", new { id = contractId });
        }
        #endregion
        public ActionResult ContractPrint(int id)
        {
            var contract = _context.TUnitRentContract.Where(c => c.IdRentContract == id)
                                                     .Include(c => c.mCompound)
                                                     .Include(c => c.mCompoundUnits)
                                                     .Include(c => c.mTenant)
                                                     .Include(c => c.mCompoundUnits.mPropertyType)
                                                     .FirstOrDefault();

            _numberToTextLogic.SetNumber(contract.yearlyRent);
            var contractModel = new PrintContractDTO
            {
                CompoundName = contract.mCompound.compoundName,
                ContractNumber = contract.contractNumber,
                UnitNumber = contract.mCompoundUnits.UnitNumber,
                CreationDate = contract.dtCreated,
                TenantNameEN = contract.mTenant.tenantName,
                TenantNameAR = string.IsNullOrEmpty(contract.mTenant.TenantNameAR) ? contract.mTenant.tenantName : contract.mTenant.TenantNameAR,
                TenantIqamaNo = contract.mTenant.IqamaNo,
                TenantMobileNo = contract.mTenant.tenantMobile,
                TenantEmail = contract.mTenant.tenantEmail,
                PropertyType = contract.mCompoundUnits.mPropertyType.PropertyTypeName,
                LeaseStartDate = contract.dtLeaseStart.ToShortDateString(),
                LeaseEndDate = contract.dtLeaseEnd.ToShortDateString(),
                YearlyRent = contract.yearlyRent,
                YearlyRentAR = _numberToTextLogic.ConvertToArabic(),
                YearlyRentEN = _numberToTextLogic.ConvertToEnglish(),
                Insurance = contract.insurance,
                Commission = contract.rentCommission,
                AdditionalNotesEN = contract.Notes,
                AdditionalNotesAR = contract.NotesAR,
                UnitDescriptionAR = contract.mCompoundUnits.DescriptionAR,
                UnitDescriptionEN = contract.mCompoundUnits.Description
            };
            _numberToTextLogic.SetNumber(contractModel.Insurance);
            contractModel.InsuranceAR = _numberToTextLogic.ConvertToArabic();
            contractModel.InsuranceEN = _numberToTextLogic.ConvertToEnglish();
            _numberToTextLogic.SetNumber(contractModel.Commission);
            contractModel.CommissionAR = _numberToTextLogic.ConvertToArabic();
            contractModel.CommissionEN = _numberToTextLogic.ConvertToEnglish();
            contractModel.CreationDateEN = $"{contractModel.CreationDate.Day}/{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(contractModel.CreationDate.Month)}/{contractModel.CreationDate.Year}";
            contractModel.CreationDateAR = $"{contractModel.CreationDate.Day}\\{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(contractModel.CreationDate.Month)}\\{contractModel.CreationDate.Year}";
            _numberToTextMonthLogic.SetNumber(contract.leasePeriodInMonthes.Value);
            var monthName = GetMonthName(contract.leasePeriodInMonthes.Value);

            contractModel.MonthsAR = $"{_numberToTextMonthLogic.ConvertToArabic()} {monthName.Arabic}";
            contractModel.MonthsEN = $"{_numberToTextMonthLogic.ConvertToEnglish()} {monthName.English}";
            return View(contractModel);
        }
        private dynamic GetMonthName(int monthsCount)
        {
            if (monthsCount == 1)
                return new { English = "Month", Arabic = "شهر" };
            else if (monthsCount <= 10)
                return new { English = "Months", Arabic = "أشهر" };

            return new { English = "Months", Arabic = "شهر" };
        }
        public ActionResult Block(int contractID)
        {
            return PartialView("~/Views/UnitRentContractCompound/_block.cshtml", new BlockContractDTO { ContractID = contractID });
        }
        [HttpPost]
        public ActionResult Block(BlockContractDTO dto)
        {
            var contract = _context.TUnitRentContract.Find(dto.ContractID);
            contract.BlockReason = dto.Reason;
            contract.Blocked = true;
            _context.SaveChanges();
            return Json(new { val = true });
        }
        public ActionResult UnBlock(int contractID)
        {
            var contract = _context.TUnitRentContract.Find(contractID);
            contract.BlockReason = null;
            contract.Blocked = false;
            _context.SaveChanges();
            return Json(new { val = true });
        }
        public async Task<IActionResult> Blocked(int compoundID)
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ?
                   (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy)
                                                        .Include((UnitRentContract u) => u.mModifiedBy)
                                                        .Include((UnitRentContract u) => u.mTenant)
                                                        .Include((UnitRentContract m) => m.mCompoundUnits)
                                                        .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                        .Include(m => m.mCompoundUnits.mPropertyType)
                    where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound == compoundID && m.Archived == false &&
                    m.Blocked
                    select m) :
                    (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy)
                                                         .Include((UnitRentContract u) => u.mModifiedBy)
                                                         .Include((UnitRentContract u) => u.mTenant)
                                                         .Include((UnitRentContract m) => m.mCompoundUnits)
                                                         .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                                                         .Include(m => m.mCompoundUnits.mPropertyType)
                     where m.IdCompound == compoundID && m.Archived == false && m.Blocked
                     select m);
            ViewBag.CompoundName = _context.TCompounds.FirstOrDefault(c => c.IdCompound == compoundID).compoundName;
            return View(await applicationDbContext.Where(e => e.AddedtoCourt == false).ToListAsync());
        }

        public ActionResult UnitHistory(int unitId)
        {
            var Result = _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                .Include((UnitRentContract m) => m.mCompoundUnits)
                .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                        .Include(c => c.mCompoundUnits.mPropertyType).Where(x => x.IdUnitCompound == unitId);
            return View(Result);

        }

        public ActionResult TenantHistory(int TenentId)
        {
            var Result = _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                .Include((UnitRentContract m) => m.mCompoundUnits)
                .ThenInclude((CompoundUnits m) => m.mCompoundBuilding)
                    .Include(c => c.mCompoundUnits.mPropertyType).Where(x => x.IdTenant == TenentId);
            return View(Result);
        }
        #region Privet Helper Methods
        private async Task<string> SaveFileToDirectory(IFormFile file, string folderName = "EmployeeFiles")
        {
            var _fileDeafaultPath = ((PhysicalFileProvider)_fileProvider).Root;
            if (file != null && file.Length != 0L)
            {
                var empImagePath = Path.Combine(_fileDeafaultPath, folderName + "\\");
                Directory.CreateDirectory(empImagePath);
                string imgFullName = Path.Combine(empImagePath, Path.GetFileName(file.FileName));
                using (FileStream target = new FileStream(imgFullName, FileMode.Create))
                {
                    await file.CopyToAsync(target);
                }
                return Path.GetFileName(file.FileName);
            }
            return string.Empty;
        }
        #endregion

        public async Task<ActionResult> SendWhatsAppInvoice(int unitId)
        {
            var dueValues = GetListUsersNotPayedDue(unitId);
            try
            {

                foreach (var item in dueValues)
                {
                    string value = "عزيزنا المستاجر :" +
                                "برجاء سرعة سداد الايجار للوحده رقم: " + item.UnitNumber + " قى المبنى رقم: " + item.Building + " للعقد رقم " + item.ContractNumber;
                    foreach (var date in item.RemainingDates)
                    {
                        value += "\n بتاريخ " + date.DueDate.ToShortDateString() + " بقيمة: " + date.RemainingAmount;

                    }
                    if (!string.IsNullOrEmpty(item.TenantWhatsapp))
                    {
                        try
                        {
                            var client = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, "https://go-wloop.net/api/v1/message/send");
                            request.Headers.Add("Accept", "application/json");
                            request.Headers.Add("AUTHORIZATION", "Bearer 6f8ad056d8d39e466b789264d0960ba4_sXPu0552ZnXd6r6PJFBNX0z0xcYUH1Pr6PQKNGPw");
                            var content = new MultipartFormDataContent();
                            string WhatsApp = item.TenantWhatsapp;
                            content.Add(new StringContent(WhatsApp), "phone");
                            content.Add(new StringContent(value), "body");
                            request.Content = content;
                            if (!string.IsNullOrEmpty(WhatsApp))
                            {
                                var response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();
                            }
                        }
                        catch (Exception ex)
                        {
                            return View();
                            throw;
                        }

                    }
                }
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                return View("index");
                throw;
            }

        }
        private List<DueValue> GetListUsersNotPayedDue(int unitId)
        {
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);

            var due60 = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c =>
                                                               !c.Archived
                                                               && !string.IsNullOrEmpty(c.mTenant.tenantEmail)
                                                               && c.remainingAmount > 0
                                                               && c.UnitRentContractPayments.Where(p => !p.Paid).MinOrDefault(p => p.DueDate) != null
                                                               && c.IdUnitCompound == unitId
                                                              )
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractID = t.IdRentContract,
                                                      ContractNumber = t.contractNumber,
                                                      UnitNumber = t.mUnit.UnitNumber ?? t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      TenantEmail = t.mTenant.tenantEmail,
                                                      TenantWhatsapp = t.mTenant.Whatsapp,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next30DaysTime).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      RemainingDates = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next30DaysTime).OrderBy(e => e.DueDate).Select(e => new DueDatesWithValues { DueDate = e.DueDate, RemainingAmount = e.RemainingAmount }).ToList(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Building = t.mUnit.mBuilding.BuildingName ?? t.mCompound.compoundName,
                                                      Mandoob = t.Mandoob.fullName,
                                                      MandoobPhone = t.Mandoob.PhoneNumber,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault()

                                                  }).OrderBy(t => t.TotalDays).ToList();

            return due60;
        }
    }

    public class PrintContractDTO
    {
        public string CompoundName { get; set; }
        public string ContractNumber { get; set; }
        public string UnitNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public string TenantIqamaNo { get; set; }
        public string TenantMobileNo { get; set; }
        public string TenantEmail { get; set; }
        public string PropertyType { get; set; }
        public string LeaseStartDate { get; set; }
        public string LeaseEndDate { get; set; }
        public int YearlyRent { get; set; }
        public string YearlyRentAR { get; set; }
        public string YearlyRentEN { get; set; }
        public int Insurance { get; set; }
        public string InsuranceEN { get; set; }
        public string InsuranceAR { get; set; }
        public string CommissionEN { get; set; }
        public string CommissionAR { get; set; }
        public decimal Commission { get; set; }
        public string CreationDateEN { get; set; }
        public string CreationDateAR { get; set; }
        public string MonthsAR { get; set; }
        public string MonthsEN { get; set; }
        public string AdditionalNotesEN { get; set; }
        public string AdditionalNotesAR { get; set; }
        public string TenantNameEN { get; set; }
        public string TenantNameAR { get; set; }
        public string UnitDescriptionEN { get; set; }
        public string UnitDescriptionAR { get; set; }
    }
    public class BlockContractDTO
    {
        public int ContractID { get; set; }
        public string Reason { get; set; }
    }
}
