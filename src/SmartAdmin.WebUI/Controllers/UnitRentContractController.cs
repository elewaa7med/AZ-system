using Microsoft.AspNetCore.Authorization;
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
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class UnitRentContractController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _user;
        private readonly IFileProvider _fileProvider;
        NumberToTextLogic _numberToTextLogic;
        public UnitRentContractController(ApplicationDbContext context, UserManager<ApplicationUser> user, IFileProvider fileProvider, IHostingEnvironment env)
        {
            _context = context;
            _user = user;
            _fileProvider = fileProvider;
            _numberToTextLogic = new NumberToTextLogic(new CurrencyInfoLogic(Currency.SaudiArabia));
            _env = env;
        }

        public async Task<IActionResult> Index(int id, string representitveId = null)
        {
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            ViewBag.id = id;
            _context.TUnitRentContract.Where((UnitRentContract m) => false);
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy)
                                                                                                                  .Include((UnitRentContract u) => u.mModifiedBy)
                                                                                                                  .Include((UnitRentContract u) => u.mTenant)
                                                                                                                  .Include((UnitRentContract u) => u.mUnit)
                                                                                                                  .ThenInclude((Units m) => m.mBuilding)
                                                                                  //by H1
                                                                              where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound == null && m.Archived == false && m.AddedtoCourt == false
                                                                              select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                        .Include((UnitRentContract u) => u.mUnit)
                        .ThenInclude((Units m) => m.mBuilding)
                                                                                           where m.IdCompound == null && m.Archived == false && (string.IsNullOrEmpty(representitveId) ? true : m.IdMandoob == representitveId)
                                                                                           select m);
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions
                .Select(p => p.Permission).Contains(Permission.BuildingAccountant) && mandoobUsersIDs
                .Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;
            }
            var result = await applicationDbContext.Include(x => x.Mandoob).Where(e => e.AddedtoCourt == false && e.mMasterBuilding == id).ToListAsync();
            return View(result);
        }

        //display all archived contract in Table like (index)
        public async Task<IActionResult> Archived(int id, string representitveId = null)
        {
            ViewBag.id = id;
            representitveId = representitveId == "null" ? null : representitveId;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            _context.TUnitRentContract.Where((UnitRentContract m) => false);
            IQueryable<UnitRentContract> applicationDbContext = (!isAdmin) ? (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                    .Include((UnitRentContract u) => u.mUnit)
                    .ThenInclude((Units m) => m.mBuilding)
                                                                                  //by H1
                                                                              where m.IdMandoob == _user.GetUserId(HttpContext.User) && m.IdCompound == null && m.Archived == true && m.AddedtoCourt == false
                                                                              select m) : (from m in _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                                                                                  .Include((UnitRentContract u) => u.mUnit)
                                                                                  .ThenInclude((Units m) => m.mBuilding)
                                                                                           where m.IdCompound == null && m.Archived == true && (string.IsNullOrEmpty(representitveId) ? true : m.IdMandoob == representitveId)
                                                                                           select m);
            if (isAdmin)
            {
                var mandoobUsersIDs = _user.GetUsersInRoleAsync("Mandoob").Result.Select(u => u.Id);
                ViewBag.Representatives = new SelectList(_context.Users.Include(u => u.UserPermissions).Where(m => m.UserPermissions.Select(p => p.Permission).Contains(Permission.BuildingAccountant) && mandoobUsersIDs.Contains(m.Id)), "Id", "fullName", string.IsNullOrEmpty(representitveId) ? null : representitveId);
                ViewBag.RepresentitveId = representitveId;

            }
            return View(await applicationDbContext.Where(x => x.mMasterBuilding == id).ToListAsync());
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
            unitRentContract.IdBuilding = unitRentContract.mUnit.IdBuilding;
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
            var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                  } into m
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.mUnit.IdBuilding);
            base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            ViewBag.ContractId = id;
            ViewBag.id = unitRentContract.mMasterBuilding;
            return View(unitRentContract);
        }

        public async Task<IActionResult> Create(int id)
        {
            string IdCreated = "";
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName");
            var building = await (from m in _context.TBuildings.Where(m => m.idMasterBuilding == id).Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                  } into m
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
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
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitRentContract unitRentContract, IFormFile contractImageFile)
        {
            var rentContractUnit = _context.TUnitRentContract.Include(e => e.mUnit).FirstOrDefault(e => e.IdUnit == unitRentContract.IdUnit && e.mUnit.isRented == true && !e.Archived);
            if (rentContractUnit != null)
            {
                base.ViewData["isRentedErrMsg"] = "This Unit is already rented with contract No.: " + rentContractUnit.contractNumber;
            }

            if (rentContractUnit == null && ModelState.IsValid)
            {

                if (unitRentContract.IdUnit > 0 && unitRentContract.IdUnit.HasValue)
                {
                    unitRentContract.dtCreated = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        unitRentContract.IdCreated = _user.GetUserId(base.HttpContext.User);
                    }

                    if (unitRentContract.UnitIncluded != null && unitRentContract.UnitIncluded.Any())
                    {
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
                    }
                    if (contractImageFile != null && contractImageFile.Length != 0L)
                    {
                        unitRentContract.contractImage = await SaveFileToDirectory(contractImageFile, "contracts");
                    }

                    var paymentsCount = 0;
                    DateTime lastPay = new DateTime();
                    //add water to yearly rent
                    var yearlyRent = unitRentContract.YearlyRentVm + (unitRentContract.WaterBillAmount ?? 0);
                    if (unitRentContract.paymentMethod == 1)
                    {
                        var rent = yearlyRent / unitRentContract.leasePeriodInMonthes.Value;
                        for (int i = 0; i < unitRentContract.leasePeriodInMonthes.Value; i++)
                        {
                            unitRentContract.UnitRentContractPayments.Add(new UnitRentContractPayment
                            {
                                Amount = i == unitRentContract.leasePeriodInMonthes.Value - 1 ? (yearlyRent - rent * i) : rent,
                                DueDate = unitRentContract.dtLeaseStart.AddMonths(i)
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

                    var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.UnitID == unitRentContract.IdUnit);
                    if (elctricityData == null)
                        elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { UnitID = unitRentContract.IdUnit }).Entity;
                    elctricityData.PaymentNumber = unitRentContract.PaymentNumber;
                    elctricityData.ElectricityMeterNumber = unitRentContract.ElectricityNumber;
                    elctricityData.StartOfMeter = unitRentContract.MeterStart;
                    elctricityData.TransferTheAccountToTenant = unitRentContract.TransferToTenanat;
                    try
                    {
                        await _context.SaveChangesAsync();
                        var rentedUnit = _context.Units.Where(m => (int?)m.IdUnit == unitRentContract.IdUnit).FirstOrDefault();
                        rentedUnit.isRented = true;
                        rentedUnit.UnitRentContractID = unitRentContract.IdRentContract;
                        rentedUnit.IdRentContract = unitRentContract.IdRentContract;
                        await _context.SaveChangesAsync();
                        UpdateLastPayment(unitRentContract.IdRentContract, lastPay);
                        return RedirectToAction("Index", new { id = unitRentContract.mMasterBuilding });
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
            var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                  } into m
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.IdBuilding);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            return View(unitRentContract);
        }


        public async Task<IActionResult> Edit(int pageid, int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            UnitRentContract unitRentContract = await _context.TUnitRentContract.Where(x => x.mMasterBuilding == pageid).Include((UnitRentContract t) => t.mUnit).SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            if (unitRentContract == null)
            {
                return NotFound();
            }
            unitRentContract.IdBuilding = unitRentContract.mUnit.IdBuilding;
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
            var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName),
                                      page = m.idMasterBuilding
                                  } into m
                                  where m.page == pageid
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.mUnit.IdBuilding);
            //tog get current rented unit also so it can be displayed            
            base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding && (m.isRented != true || m.IdUnit == unitRentContract.IdUnit)), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
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

                if (unitRentContract.IdUnit.HasValue)
                {
                    Units unit = _context.Units.Where(x => x.UnitRentContractID == unitRentContract.IdRentContract).FirstOrDefault();
                    try
                    {
                        if (unit.IdUnit != unitRentContract.IdUnit)
                        {
                            unit.IdRentContract = null;
                            unit.UnitRentContractID = null;
                            unit.isRented = false;
                            _context.Update(unit);

                            unit = _context.Units.Where(x => x.IdUnit == unitRentContract.IdUnit).FirstOrDefault();
                            unit.IdRentContract = unitRentContract.IdRentContract;
                            unit.UnitRentContractID = unitRentContract.IdRentContract;
                            unit.isRented = true;
                            _context.Update(unit);
                        }


                        unitRentContract.dtModified = DateTime.Now;
                        string modifiedBy = null;
                        if (_user.GetUserName(base.HttpContext.User) != null)
                        {
                            modifiedBy = _user.GetUserId(base.HttpContext.User);
                            unitRentContract.IdModified = modifiedBy;
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
                        if (contractImageFile != null && contractImageFile.Length != 0L)
                        {
                            unitRentContract.contractImage = await SaveFileToDirectory(contractImageFile, "contracts");
                        }
                        DateTime date = new DateTime();
                        //add water to yearly rent
                        var yearlyRent = unitRentContract.YearlyRentVm + (unitRentContract.WaterBillAmount ?? 0);
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
                                //case 6:
                                //    date = ApplyPayments(unitRentContract,yearlyRent / 52,
                                //                  unitRentContract.leasePeriodInMonthes.Value / 7, 7);
                                //    break;
                        }
                        if (unitRentContract.VerifiedFromGovernment)
                            unitRentContract.NotVerifiedReason = null;
                        var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.UnitID == unitRentContract.IdUnit);
                        if (elctricityData == null)
                            elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { UnitID = unitRentContract.IdUnit }).Entity;
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
                    return RedirectToAction("Index", new { id = unitRentContract.mMasterBuilding });
                }
                base.ViewData["isRentedErrMsg"] = "Please Select Unit ";
            }
            base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
            base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
            base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
            var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                  } into m
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.IdBuilding);
            base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
            ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
            return View(unitRentContract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            UnitRentContract unitRentContract = await _context.TUnitRentContract.Include((UnitRentContract t) => t.mUnit).SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
            if (unitRentContract == null)
            {
                return NotFound();
            }
            unitRentContract.IdBuilding = unitRentContract.mUnit.IdBuilding;
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
            var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                  select new
                                  {
                                      IdBuilding = m.IdBuilding,
                                      BuildingName = m.BuildingName,
                                      BuildingAddress = m.BuildingAddress,
                                      DistrictName = m.mDistrict.DistrictName,
                                      buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                  } into m
                                  orderby m.DistrictName, m.BuildingName
                                  select m).ToListAsync();
            base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.mUnit.IdBuilding);
            base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
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
            UnitRentContract unitRentContract = null;
            string returnTo = null;
            try
            {
                unitRentContract = await _context.TUnitRentContract.Include(u => u.mUnit).SingleOrDefaultAsync((UnitRentContract m) => m.IdRentContract == id);
                Tenants tenant = await _context.TTenants.Where(x => x.IdTenant == unitRentContract.IdTenant).FirstOrDefaultAsync();
                tenant.Archived = true;
                if (unitRentContract.mUnit != null && !unitRentContract.Archived)
                {
                    unitRentContract.mUnit.isRented = false;
                    unitRentContract.mUnit.UnitRentContractID = null;
                    unitRentContract.mUnit.IdRentContract = null;
                }
                if (unitRentContract.Archived)
                {
                    returnTo = "Archived";

                }
                else
                {
                    returnTo = "index";
                }
                _context.TUnitRentContract.Remove(unitRentContract);
                _context.TTenants.Update(tenant);
                await _context.SaveChangesAsync();
                TempData["success"] = "Operation is done successfully";
            }
            catch (Exception ex)
            {
                TempData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";
                TempData["devex"] = ex.Message + " / " + ex.InnerException?.Message ?? "";
            }
            return RedirectToAction(returnTo, new { id = unitRentContract.mMasterBuilding });
        }

        private bool UnitRentContractExists(int id)
        {
            return _context.TUnitRentContract.Any((UnitRentContract e) => e.IdRentContract == id);
        }

        public async Task<JsonResult> getBuildingUnits(int IdBuilding, int? unitID = null)
        {
            var result = await _context.Units.Where(e => e.IdBuilding == IdBuilding && e.UnitOwner != Enums.UnitOwner.xSaad && (e.IdUnit == unitID || e.isRented != true)).OrderBy(e => e.UnitNumber)
                .Select(e => new
                {
                    e.IdUnit,
                    e.UnitNumber,
                    e.IdBuilding
                }).ToListAsync();
            var selectUnitList = new SelectList(result, "IdUnit", "UnitNumber");
            return Json(selectUnitList);
        }

        public JsonResult getUnitInfo(int IdUnit)
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
                            where m.IdUnit == IdUnit
                            select m).FirstOrDefault();
            var unit = _context.ElectricityMeters.Where(u => u.UnitID == IdUnit).Select(u => new
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
                IdUnit = unitInfo.IdUnit,
                unitArea = unitInfo.unitArea,
                buildingNo = unitInfo.buildingNo,
                buildingAddress = unitInfo.buildingAddress,
                district = unitInfo.district,
                city = unitInfo.city,
                country = unitInfo.country,
                owner = unitInfo.owner,
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
            UnitRentContract unitRentContract = null;
            try
            {
                unitRentContract = await _context.TUnitRentContract.Include(u => u.mUnit).SingleOrDefaultAsync((UnitRentContract m) => m.IdRentContract == id);
                Units currnetUnit = await _context.Units.FindAsync(unitRentContract.mUnit.IdUnit);
                var tenant = await _context.TTenants.Where(x => x.IdTenant == unitRentContract.IdTenant).FirstOrDefaultAsync();
                if (currnetUnit != null)
                {
                    currnetUnit.isRented = false;
                    currnetUnit.UnitRentContractID = null;
                    await _context.SaveChangesAsync();
                }
                unitRentContract.Archived = true;
                tenant.Archived = true;
                await _context.SaveChangesAsync();
                TempData["success"] = "Operation completed successfully";
            }
            catch
            {
                TempData["AlertSaveErr"] = "There is an Error when archiving . Please correct and try again.";
            }

            return RedirectToAction("Index", new { id = unitRentContract.mMasterBuilding });
        }

        public async Task<IActionResult> UnArchive(int id, int pageid)
        {
            UnitRentContract unitRentContract = null;
            try
            {
                unitRentContract = await _context.TUnitRentContract.Include(u => u.mUnit).SingleOrDefaultAsync((UnitRentContract m) => m.IdRentContract == id);
                var tenant = await _context.TTenants.Where(x => x.IdTenant == unitRentContract.IdTenant).FirstOrDefaultAsync();

                Units currnetUnit = await _context.Units.FindAsync(unitRentContract.mUnit.IdUnit);
                if (currnetUnit != null)
                {
                    if (currnetUnit.isRented == true)
                    {
                        ViewData["AlertSaveErr"] = "This Unit is already rented";
                        return RedirectToAction("Archived");
                    }
                    currnetUnit.isRented = true;
                    currnetUnit.UnitRentContractID = unitRentContract.IdRentContract;
                }
                unitRentContract.Archived = false;
                tenant.Archived = false;
                await _context.SaveChangesAsync();
                TempData["success"] = "Operation completed successfully";
            }
            catch
            {
                TempData["AlertSaveErr"] = "There is an Error when archiving . Please correct and try again.";
            }

            return RedirectToAction("Index", new { id = pageid });
        }


        [ActionName("Renew")]
        public async Task<IActionResult> Renew(int id)
        {
            UnitRentContract unitRentContract = null;
            try
            {
                unitRentContract = await _context.TUnitRentContract.Include((UnitRentContract t) => t.mUnit).SingleOrDefaultAsync((UnitRentContract m) => (int?)m.IdRentContract == id);
                if (unitRentContract == null)
                {
                    return NotFound();
                }
                unitRentContract.IdBuilding = unitRentContract.mUnit.IdBuilding;
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
                var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                      select new
                                      {
                                          IdBuilding = m.IdBuilding,
                                          BuildingName = m.BuildingName,
                                          BuildingAddress = m.BuildingAddress,
                                          DistrictName = m.mDistrict.DistrictName,
                                          buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                      } into m
                                      orderby m.DistrictName, m.BuildingName
                                      select m).ToListAsync();
                base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.mUnit.IdBuilding);
                base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
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
            catch
            {
                base.ViewData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";
                return RedirectToAction("Index", new { id = unitRentContract.mMasterBuilding });
            }

        }

        [HttpPost]
        [ActionName("Renew")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id, UnitRentContract unitRentContract, IFormFile contractImageFile, string PrevContractNum)
        {
            try
            {
                if (id != unitRentContract.IdRentContract)
                {
                    return NotFound();
                }
                if (base.ModelState.IsValid)
                {
                    if (unitRentContract.IdUnit.HasValue)
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
                                case 6:
                                    date = ApplyPayments(unitRentContract, yearlyRent / 52,
                                                  unitRentContract.leasePeriodInMonthes.Value / 7, 7);
                                    break;
                            }
                            var oldContract = _context.TUnitRentContract.Include(t => t.mUnit).SingleOrDefault(c => c.contractNumber == PrevContractNum);
                            var tenant = _context.TTenants.Where(x => x.Archived == true).FirstOrDefault();
                            var unit = oldContract.mUnit;
                            if (unit != null)
                            {
                                unit.isRented = false;
                                unit.UnitRentContractID = null;
                                unit.IdRentContract = null;
                            }
                            oldContract.Archived = true;
                            _context.TUnitRentContract.Update(oldContract);
                            if (unitRentContract.VerifiedFromGovernment)
                                unitRentContract.NotVerifiedReason = null;

                            var elctricityData = _context.ElectricityMeters.FirstOrDefault(u => u.UnitID == unitRentContract.IdUnit);
                            if (elctricityData == null)
                                elctricityData = _context.ElectricityMeters.Add(new ElectricityMeter { UnitID = unitRentContract.IdUnit }).Entity;
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
                            unit.IdRentContract = unitRentContract.IdRentContract;
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
                            base.ViewData["AlertSaveErr"] = "Contract Number is already used > There is an Error Savng the contract. Please correct and try again.";
                        }
                        return RedirectToAction("Index", new { id = unitRentContract.mMasterBuilding });
                    }
                    base.ViewData["isRentedErrMsg"] = "Please Select Unit ";
                }
                // base.ViewData["IdCreated"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdCreated);
                base.ViewData["IdModified"] = new SelectList(_context.Users, "Id", "fullName", unitRentContract.IdModified);
                base.ViewData["IdTenant"] = new SelectList(_context.TTenants.OrderBy((Tenants t) => t.tenantName), "IdTenant", "tenantName", unitRentContract.IdTenant);
                var building = await (from m in _context.TBuildings.Include((Buildings m) => m.mDistrict)
                                      select new
                                      {
                                          IdBuilding = m.IdBuilding,
                                          BuildingName = m.BuildingName,
                                          BuildingAddress = m.BuildingAddress,
                                          DistrictName = m.mDistrict.DistrictName,
                                          buildingInfo = string.Concat(m.BuildingName, " - ", m.mDistrict.DistrictName)
                                      } into m
                                      orderby m.DistrictName, m.BuildingName
                                      select m).ToListAsync();
                base.ViewData["IdBuildingsDDL"] = new SelectList(building, "IdBuilding", "buildingInfo", unitRentContract.IdBuilding);
                base.ViewData["IdUnit"] = new SelectList(_context.Units.Where((Units m) => m.IdBuilding == unitRentContract.mUnit.IdBuilding), "IdUnit", "UnitNumber", unitRentContract.IdUnit);
                ViewData["Items"] = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem{Text="Water Bill Included",Value="1"},
                new SelectListItem{Text="Electricity Bill Included",Value="2"},
                new SelectListItem{Text="Furnished Unit",Value="3"},
                new SelectListItem{Text="Attested Rent Contract",Value="4"}
            };
                TempData["success"] = "Operation completed successfully";

            }
            catch (Exception ex)
            {
                base.ViewData["AlertSaveErr"] = "There is an Error . Please correct and try again.";
            }
            return View(unitRentContract);
        }
        #region Payment

        public IActionResult Invoice(int InvoiceId)
        {

            var writer = new QRCodeWriter();
            string returnvalue = EncryptString(InvoiceId.ToString(), ConstantValue.EncriptionKey);
            var resultibt = writer.encode(ConstantValue.BaseUrl + ConstantValue.globalInvoicePath + returnvalue, BarcodeFormat.QR_CODE, 200, 200);

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
                Include(x => x.invoiceRelatedPaymentDates).ThenInclude(x => x.unitRentContractPayment).ThenInclude(x => x.UnitRentContract).ThenInclude(x => x.mUnit).ThenInclude(x => x.mBuilding).FirstOrDefault(x => x.Id == InvoiceId);
            Model.invoiceRelatedPaymentDates.FirstOrDefault().unitRentContractPayment.UnitRentContract.mTenant = _context.TTenants.Include(x => x.mCompany).FirstOrDefault(x => x.IdTenant == Model.invoiceRelatedPaymentDates.FirstOrDefault().unitRentContractPayment.UnitRentContract.IdTenant);


            return View("Invoice_2", Model);
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
                                       pageid = p.UnitRentContract.mMasterBuilding,
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
                                       MonyType = p.MonyType,
                                       PaymentMehtod = _context.Invoices.Where(x => x.UnitRentContractOtherPayment.ID == OtherPaymentId).Select(x => x.PaymentMehtod).FirstOrDefault(),
                                       checkVisaNumber = _context.Invoices.Where(x => x.UnitRentContractOtherPayment.ID == OtherPaymentId).Select(x => x.checkVisaNumber).FirstOrDefault()
                                   }).FirstOrDefault();
            if (otherPayment == null)
            {
                return NotFound();
            }

            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectListWithSelectedValue<Enums.PaymentMehtod>(otherPayment.PaymentMehtod);
            ViewData["MonyType"] = Enums.GetEnumAsSelectListWithSelectedValue<Enums.MonyType>(otherPayment.MonyType);
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
            InvoicesEntity.checkVisaNumber = (model.PaymentMehtod == 1 || model.PaymentMehtod == 2) ? null : model.checkVisaNumber;
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
                                       MonyType = p.MonyType,
                                       PaymentMehtod = _context.Invoices.Where(x => x.UnitRentContractOtherPayment.ID == OtherPaymentId).Select(x => x.PaymentMehtod).FirstOrDefault(),
                                       checkVisaNumber = _context.Invoices.Where(x => x.UnitRentContractOtherPayment.ID == OtherPaymentId).Select(x => x.checkVisaNumber).FirstOrDefault()
                                   }).FirstOrDefault();
            if (otherPayment == null)
            {
                return NotFound();
            }

            ViewData["PaymentMethod"] = Enums.GetEnumAsSelectListWithSelectedValue<Enums.PaymentMehtod>(otherPayment.PaymentMehtod);
            ViewData["MonyType"] = Enums.GetEnumAsSelectListWithSelectedValue<Enums.MonyType>(otherPayment.MonyType);
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
            otherpayment.PaidAmount = model.Commession + model.Insurence + model.OtherPayment;
            otherpayment.MonyType = model.MonyType;
            otherpayment.PaymentDate = DateTime.Now;
            otherpayment.UnitRentContractID = model.UnitRentContractID;
            otherpayment.Note = model.Note;
            otherpayment.UserID = _user.GetUserId(base.HttpContext.User);
            otherpayment.OtherPaymentText = model.OtherPaymentText;
            _context.Add(otherpayment);

            Invoices InvoicesEntity = new Invoices();
            InvoicesEntity.ContractId = model.UnitRentContractID;
            InvoicesEntity.Payment = model.Commession + model.Insurence + model.OtherPayment;
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

            Invoices Model = _context.Invoices.Include(x => x.UnitRentContractOtherPayment).ThenInclude(x => x.UnitRentContract).ThenInclude(x => x.mUnit).ThenInclude(x => x.mBuilding).FirstOrDefault(x => x.UnitRentContractOtherPayment.ID == InvoiceId);
            Model.UnitRentContractOtherPayment.UnitRentContract.mTenant = _context.TTenants.Include(x => x.mCompany).FirstOrDefault(x => x.IdTenant == Model.UnitRentContractOtherPayment.UnitRentContract.IdTenant);
            result.Save(WebRootPath + "\\QRs\\QR" + Model.Id + ".png");
            ViewBag.Url = "\\QRs\\QR" + Model.Id + ".png";
            return View("OtherInvoice_2", Model);
        }
        #endregion



        public IActionResult PartiallyPay(int ContractId)
        {
            var payment = _context.TUnitRentContract.Where(x => x.IdRentContract == ContractId)
                    .Include(e => e.UnitRentContractPayments)
                    .Include(e => e.invoices)
                    .Include(e => e.mTenant)
                    .Include(e => e.mCompound)
                    .Include(e => e.mCompoundUnits)
                    .Include(e => e.mUnit).ThenInclude(x => x.mPropertyType)
                    .Include(e => e.mUnit.mBuilding)
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
                        PropertyTypeId = p.mUnit.IdPropertyType,
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
                    UserID = _user.GetUserId(base.HttpContext.User),
                });

                InvoiceRelatedPaymentDates InvoiceRelatedPaymentDatesEntity = new InvoiceRelatedPaymentDates();
                InvoiceRelatedPaymentDatesEntity.Amount = PaidValue;
                InvoiceRelatedPaymentDatesEntity.InvoiceId = InvoicesEntity.Id;
                InvoiceRelatedPaymentDatesEntity.UnitRentContractPaymentId = pay.ID;
                InvoiceRelatedPaymentDatesEntity.PaymentState = pay.Paid;
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

        public IActionResult UndoLastPayment(int contractId)
        {
            var Invoice = _context.Invoices.Include(x => x.invoiceRelatedPaymentDates).OrderBy(x => x.PaymentDate).LastOrDefault(x => x.ContractId == contractId && x.Status == null);
            Invoice.Status = 0;
            UnitRentContractAllPaymentLogs unitRentContractAllPaymentLogs = new UnitRentContractAllPaymentLogs();
            if (Invoice != null)
            {
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
                    _context.SaveChanges();

                }
            }
            else
            {
                var payment = _context.TUnitRentContractPayments.Include(c => c.UnitRentContract)
                                                                           .Include(p => p.UnitRentContractPaymentLogs)
                                                                           .Where(c => c.PaidAmount > 0 && c.UnitRentContractID == contractId)
                                                                           .OrderByDescending(p => p.DueDate)
                                                                           .FirstOrDefault();
                unitRentContractAllPaymentLogs.AllPaidAmount = payment.PaidAmount;
                payment.UnitRentContract.remainingAmount += payment.PaidAmount;
                payment.UnitRentContract.paidAmount -= payment.PaidAmount;
                payment.PaidAmount = 0;
                payment.PaymentDate = null;
                payment.UserID = null;
                payment.Note = null;
                payment.Paid = false;
                _context.UnitRentContractPaymentLogs.RemoveRange(payment.UnitRentContractPaymentLogs.ToList());

            }

            _context.Add(unitRentContractAllPaymentLogs);
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
        private int CalcRemianingLastContract(int lastcontract, int paid)
        {
            int result = 0;
            return result;
        }

        private DateTime ApplyPayments(UnitRentContract unitRentContract, int rent, int paymentsCount, int incrmentStep)
        {
            var InvoicesForSameContract = _context.Invoices.Where(x => x.ContractId == unitRentContract.IdRentContract && x.Status == null);

            var payments = _context.TUnitRentContractPayments.Include(p => p.UnitRentContractPaymentLogs).Where(c => c.UnitRentContractID == unitRentContract.IdRentContract).ToList();
            _context.UnitRentContractPaymentLogs.RemoveRange(payments.SelectMany(p => p.UnitRentContractPaymentLogs).ToList());
            _context.TUnitRentContractPayments.RemoveRange(payments);
            var modifiedBy = _user.GetUserId(HttpContext.User);

            int paidAmount = unitRentContract.paidAmount;
            int remainingLastContact = unitRentContract.LastContractRemainingAmount;
            int payedFromRemainingLastContact = 0;

            //decimal InvoicePayment = invoice.Payment;
            if (unitRentContract.LastContractRemainingAmount > 0)
            {
                if (unitRentContract.LastContractRemainingAmount >= paidAmount)
                {
                    payedFromRemainingLastContact = paidAmount;
                    paidAmount = 0;
                }
                else
                {
                    paidAmount = paidAmount - unitRentContract.LastContractRemainingAmount;
                    payedFromRemainingLastContact = unitRentContract.LastContractRemainingAmount;
                }

            }

            int month = 0;
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
                            PaymentDate=InvoicesForSameContract.Count() != 0 ? InvoicesForSameContract.LastOrDefault().PaymentDate: DateTime.Now,
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
                                       User = p.User != null ? p.User.fullName : string.Empty,
                                       pageid = p.UnitRentContract.mMasterBuilding
                                   }).ToList();
            if (notes == null)
                return NotFound();

            ViewBag.ContractId = id;
            return View(notes);
        }
        public IActionResult AddNote(int id)
        {
            return View(new RentContractNoteDTO { ContractID = id, pageid = _context.TUnitRentContract.FirstOrDefault(c => c.IdRentContract == id).mMasterBuilding });
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
        public ActionResult ContractPrint(int id)
        {
            var contract = _context.TUnitRentContract.Where(c => c.IdRentContract == id)
                                                     .Include(c => c.mUnit)
                                                     .Include(c => c.mUnit.mBuilding)
                                                     .Include(c => c.mTenant)
                                                     .Include(c => c.mTenant.mCompany)
                                                     .Include(c => c.mTenant.mNationality)
                                                     .Include(c => c.mUnit.mBuilding.mDistrict)
                                                     .FirstOrDefault();

            _numberToTextLogic.SetNumber(contract.yearlyRent);
            var unitElectricity = _context.ElectricityMeters.FirstOrDefault(e => e.UnitID == contract.IdUnit);
            var contractModel = new PrintUnitContractDTO
            {
                BuildingNo = contract.mUnit.mBuilding.BuildingName,
                ContractNo = contract.contractNumber,
                CompanyName = contract.mTenant.TenantCompany,
                ElectricityNo = unitElectricity.ElectricityMeterNumber,
                ElectricitySadadNo = unitElectricity.PaymentNumber,
                IqamaNumber = contract.mTenant.IqamaNo,
                LeaseDay = contract.dtLeaseStart.Day,
                LeaseMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(contract.dtLeaseStart.Month),
                LeaseYear = contract.dtLeaseStart.Year,
                LeasePeriod = $"{contract.leasePeriodInMonthes} {GetMonthName(contract.leasePeriodInMonthes.Value).Arabic}",
                LeaseValue = _numberToTextLogic.ConvertToArabic(),
                Nationality = contract.mTenant.mNationality.CountryName,
                TenantAdress = "",
                TenantName = string.IsNullOrEmpty(contract.mTenant.TenantNameAR) ? contract.mTenant.tenantName : contract.mTenant.TenantNameAR,
                UnitNo = contract.mUnit.UnitNumber,
                WaterCost = $"{contract.WaterBillAmount ?? 0} ريال",
                TenantMobileNo = contract.mTenant.tenantMobile,
                Notes = contract.Notes,
                BuildingAdress = contract.mUnit.mBuilding.BuildingAddress,
                buildingDistrict = contract.mUnit.mBuilding.mDistrict.DistrictName
            };
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

        public ActionResult UnitHistory(int unitId)
        {
            var Result = _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                         .Include((UnitRentContract u) => u.mUnit)
                         .ThenInclude((Units m) => m.mBuilding).Include((UnitRentContract x) => x.Mandoob).Where(x => x.IdUnit == unitId);
            return View(Result);

        }

        public ActionResult TenantHistory(int TenentId)
        {
            var Result = _context.TUnitRentContract.Include((UnitRentContract u) => u.mCreatedBy).Include((UnitRentContract u) => u.mModifiedBy).Include((UnitRentContract u) => u.mTenant)
                       .Include((UnitRentContract u) => u.mUnit)
                       .ThenInclude((Units m) => m.mBuilding).Include((UnitRentContract x) => x.Mandoob).Where(x => x.IdTenant == TenentId);
            return View(Result);
        }

        public async Task<ActionResult> SendWhatsAppInvoice(int unitId, int buildingId, int pageid)
        {
            Helper helper = new Helper();
            var dueValue = helper.GetListUsersNotPayedDue(_context, unitId, buildingId);
            string WhatsAppMessage = helper.buildMessageBody(dueValue);
            HttpResponseMessage ReturnValue = new HttpResponseMessage();
            string WhatsAppNumber = helper.ValidateWhatsAppNumber(dueValue.TenantWhatsapp);
            if (WhatsAppNumber != null)
                ReturnValue = await helper.SendWhatsappAction(WhatsAppNumber, WhatsAppMessage);
            return RedirectToAction("index", new { id = pageid });
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
    }


    public class PrintUnitContractDTO
    {
        public string ContractNo { get; set; }
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        public int LeaseDay { get; set; }
        public string LeaseMonth { get; set; }
        public int LeaseYear { get; set; }
        public string LeaseValue { get; set; }
        public string LeasePeriod { get; set; }
        public string WaterCost { get; set; }
        public string IqamaNumber { get; set; }
        public string Nationality { get; set; }
        public string BuildingNo { get; set; }
        public string UnitNo { get; set; }
        public string TenantAdress { get; set; }
        public string ElectricityNo { get; set; }
        public string ElectricitySadadNo { get; set; }
        public string TenantMobileNo { get; set; }
        public string Notes { get; set; }
        public string NotesAR { get; set; }
        public string BuildingAdress { get; set; }
        public string buildingDistrict { get; set; }
    }
}
