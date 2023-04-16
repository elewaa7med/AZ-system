using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;

        private readonly IEmailSender _emailSender;

        private readonly IConverter _converter;

        private readonly IHostingEnvironment _hostingEnvironment;

        public TenantsController(ApplicationDbContext context, UserManager<ApplicationUser> user, IEmailSender emailSender, IConverter converter, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _user = user;
            _emailSender = emailSender;
            _converter = converter;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _context.TTenants.Where(x => x.Archived == false).OrderBy(m => m.tenantName).ToListAsync();
            var tenantsHasContractsIDs = await _context.TUnitRentContract
                                                 .Select(t => new
                                                 {
                                                     t.IdTenant,
                                                 }).ToListAsync();
            foreach (var item in tenants.ToList())
            {
                var tenant = tenantsHasContractsIDs.FirstOrDefault(x => x.IdTenant == item.IdTenant);
                if (tenant != null)
                    item.HasContract = true;
            }
            return View(tenants);
        }

        public async Task<IActionResult> Archive(int id)
        {
            var Tenant = await _context.TTenants.Where(x => x.IdTenant == id).FirstOrDefaultAsync();
            Tenant.Archived = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("ArchivedTenants");
        }

        public async Task<IActionResult> Unarchive(int id)
        {
            var Tenant = await _context.TTenants.Where(x => x.IdTenant == id).FirstOrDefaultAsync();
            Tenant.Archived = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ArchivedTenants()
        {
            var tenants = await _context.TTenants.Where(x => x.Archived == true).OrderBy(m => m.tenantName).ToListAsync();
            var tenantsHasContractsIDs = _context.TUnitRentContract
                                               .Select(t => new
                                               {
                                                   t.IdTenant,
                                               }).ToList();
            foreach (var item in tenants.ToList())
            {
                var tenant = tenantsHasContractsIDs.FirstOrDefault(x => x.IdTenant == item.IdTenant);
                if (tenant != null)
                    item.HasContract = true;
            }
            return View(tenants);
        }

        public async Task<IActionResult> Details(int? id, int? pageid)
        {

            if (!id.HasValue)
            {
                return NotFound();
            }
            Tenants tenants = await _context.TTenants.Include((Tenants t) => t.mNationality).Include((Tenants t) => t.mCompany).Include((Tenants t) => t.mUserCreated)
                .Include((Tenants t) => t.mUserModified)
                .SingleOrDefaultAsync((Tenants m) => (int?)m.IdTenant == id);
            if (tenants == null)
            {
                return NotFound();
            }
            ViewBag.id = pageid;
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", tenants.IdNationality);
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName", tenants.IdCompany);
            return View(tenants);
        }

        public IActionResult Create()
        {
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName");
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName");
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName");
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tenants tenants, IFormFile iqamaImageFile)
        {
            if (base.ModelState.IsValid)
            {
                if (iqamaImageFile != null && iqamaImageFile.Length != 0L)
                {
                    string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Tenants\\", Path.GetFileName(iqamaImageFile.FileName));
                    using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                    {
                        await iqamaImageFile.CopyToAsync(imgfileStream);
                    }
                    tenants.IqamaPicture = Path.GetFileName(iqamaImageFile.FileName);
                }
                tenants.dtCreated = DateTime.Now;
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    tenants.IdCreatedBy = _user.GetUserId(base.HttpContext.User);
                }
                _context.Add(tenants);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", tenants.IdNationality);
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName", tenants.IdCompany);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdModifiedBy);
            return View(tenants);
        }

        public async Task<IActionResult> Edit(int? id, int? pageid)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Tenants tenants = await _context.TTenants.SingleOrDefaultAsync((Tenants m) => (int?)m.IdTenant == id);
            if (tenants == null)
            {
                return NotFound();
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", tenants.IdNationality);
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName", tenants.IdCompany);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdModifiedBy);
            ViewBag.id = pageid;
            return View(tenants);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tenants tenants, IFormFile iqamaImageFile, int? pageid)
        {
            if (id != tenants.IdTenant)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                if (iqamaImageFile != null && iqamaImageFile.Length != 0L)
                {
                    string imgFullName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\Tenants\\", Path.GetFileName(iqamaImageFile.FileName));
                    using (FileStream imgfileStream = new FileStream(imgFullName, FileMode.Create))
                    {
                        await iqamaImageFile.CopyToAsync(imgfileStream);
                    }
                    tenants.IqamaPicture = Path.GetFileName(iqamaImageFile.FileName);
                }
                try
                {
                    tenants.dtModified = DateTime.Now;
                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        tenants.IdModifiedBy = _user.GetUserId(base.HttpContext.User);
                    }
                    _context.Update(tenants);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantsExists(tenants.IdTenant))
                    {
                        return NotFound();
                    }
                    throw;
                }
                if (pageid == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("GetTenants", "Home", new { id = pageid });

                }
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", tenants.IdNationality);
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName", tenants.IdModifiedBy);
            base.ViewData["IdCreatedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdCreatedBy);
            base.ViewData["IdModifiedBy"] = new SelectList(_context.Users, "Id", "fullName", tenants.IdModifiedBy);
            return View(tenants);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Tenants tenants = await _context.TTenants.Include((Tenants t) => t.mNationality).Include((Tenants t) => t.mCompany).Include((Tenants t) => t.mUserCreated)
                .Include((Tenants t) => t.mUserModified)
                .SingleOrDefaultAsync((Tenants m) => (int?)m.IdTenant == id);
            if (tenants == null)
            {
                return NotFound();
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", tenants.IdNationality);
            base.ViewData["IdCompany"] = new SelectList(_context.TCompanies, "IdCompany", "companyName", tenants.IdCompany);
            return View(tenants);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Tenants tenants = await _context.TTenants.SingleOrDefaultAsync((Tenants m) => m.IdTenant == id);
            try
            {
                _context.TTenants.Remove(tenants);
                await _context.SaveChangesAsync();
            }
            catch
            {
                base.ViewData["AlertSaveErr"] = "There is an Error When Delete . Please correct and try again.";
            }
            return RedirectToAction("Index");
        }

        private bool TenantsExists(int id)
        {
            return _context.TTenants.Any((Tenants e) => e.IdTenant == id);
        }

        public ActionResult SendEmailsToTenants(List<int> tenantsIDs, int? compoundID = null)
        {
            //var emailsList = _context.TTenants.Where(t => tenantsIDs.Contains(t.IdTenant)).Select(t => new
            //{
            //    t.tenantEmail,
            //    t.IdTenant
            //}).ToList();
            //var electricityData = _context.TUnitRentContract.Where(u => tenantsIDs.Contains(u.IdTenant) && u.IdCompound == compoundID && !string.IsNullOrEmpty(u.mTenant.tenantEmail))
            //                                                .Select(u => new
            //                                                {
            //                                                    u.IdTenant,
            //                                                    u.mTenant.tenantEmail,
            //                                                    UnitID = compoundID.HasValue ? u.mCompoundUnits.IdUnit : u.mUnit.IdUnit,
            //                                                    UnitNumber = compoundID.HasValue ? u.mCompoundUnits.UnitNumber : u.mUnit.UnitNumber,
            //                                                    BuildingNumber = compoundID.HasValue ? u.mCompoundUnits.mCompoundBuilding.BuildingNumber : u.mUnit.mBuilding.BuildingName,
            //                                                    ElectricityData = _context.ElectricityMeters.Where(e => compoundID.HasValue ? e.CompoundUnitID == u.IdUnitCompound : e.UnitID == u.IdUnit)
            //                                                                                                .Select(e => new
            //                                                                                                {
            //                                                                                                    e.PaymentNumber,
            //                                                                                                    e.ElectricityMeterNumber
            //                                                                                                }).FirstOrDefault()

            //                                                }).ToList();
            //var html = x = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/folder/filename.html"))
            //foreach (var data in electricityData)
            //{
            //    _emailSender.SendEmailHtmlBodyAsync(data.tenantEmail, "Electricity meter data",);
            //}
            return Json(new { val = true });
        }
        public ActionResult SendEmailToTenant(int contractID)
        {
            var contract = _context.TUnitRentContract.Where(t => t.IdRentContract == contractID)
                                                     .Include(c => c.mUnit)
                                                     .Include(c => c.mCompoundUnits)
                                                     .Include(c => c.mUnit.mBuilding)
                                                     .Include(c => c.mCompoundUnits.mCompoundBuilding)
                                                     .Include(c => c.mTenant).FirstOrDefault();
            var electricityData = _context.ElectricityMeters.Where(e => contract.IdCompound.HasValue ? e.CompoundUnitID == contract.IdUnitCompound : e.UnitID == contract.IdUnit)
                                                            .Select(e => new
                                                            {
                                                                e.PaymentNumber,
                                                                e.ElectricityMeterNumber
                                                            }).FirstOrDefault();
            var compoundName = contract.IdCompound.HasValue ? _context.TCompounds.Where(c => c.IdCompound == contract.IdCompound).Select(c => c.compoundName).FirstOrDefault() : string.Empty;
            var model = new TenantElectricityData
            {
                TenantEmail = contract.mTenant.tenantEmail,
                UnitNumber = contract.IdCompound.HasValue ? contract.mCompoundUnits.UnitNumber : contract.mUnit.UnitNumber,
                BuildingNumber = contract.IdCompound.HasValue ? contract.mCompoundUnits.mCompoundBuilding.BuildingNumber : contract.mUnit.mBuilding.BuildingName,
                ElectricityMeterNumber = electricityData.ElectricityMeterNumber,
                ElectricityPaymentNumber = electricityData.PaymentNumber,
                CompoundName = compoundName
            };
            var html = this.RenderViewAsync("~/Views/Tenants/SendEmail.cshtml", model, true).Result;
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = {
                            ColorMode = ColorMode.Color,
                            Orientation = Orientation.Landscape,
                            PaperSize = PaperKind.A4Plus,
                            Margins=new MarginSettings{Bottom=0,Left=0,Right=0,Top=0 },
                            DPI=300,
                            ImageQuality=600,
                            ImageDPI=1200
                             },
                Objects = {
                        new ObjectSettings() {
                            HtmlContent = html,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            UseLocalLinks=true
                        }}
            };
            byte[] bytes = _converter.Convert(doc);
            var fileName = $"{Guid.NewGuid().ToString("N")}.pdf";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "TempFiles", fileName);
            System.IO.File.WriteAllBytes(filePath, bytes);
            var attachment = new System.Net.Mail.Attachment(filePath);
            _emailSender.SendEmailWithAttachments(model.TenantEmail, "Electricity meter data", attachment);
            System.IO.File.Delete(filePath);
            return Json(new { val = true });
        }
        public ActionResult SendEmailToTenantHTML(int contractID)
        {
            var contract = _context.TUnitRentContract.Where(t => t.IdRentContract == contractID)
                                                     .Include(c => c.mUnit)
                                                     .Include(c => c.mCompoundUnits)
                                                     .Include(c => c.mUnit.mBuilding)
                                                     .Include(c => c.mCompoundUnits.mCompoundBuilding)
                                                     .Include(c => c.mTenant).FirstOrDefault();
            var electricityData = _context.ElectricityMeters.Where(e => contract.IdCompound.HasValue ? e.CompoundUnitID == contract.IdUnitCompound : e.UnitID == contract.IdUnit)
                                                            .Select(e => new
                                                            {
                                                                e.PaymentNumber,
                                                                e.ElectricityMeterNumber
                                                            }).FirstOrDefault();
            var compoundName = contract.IdCompound.HasValue ? _context.TCompounds.Where(c => c.IdCompound == contract.IdCompound).Select(c => c.compoundName).FirstOrDefault() : string.Empty;
            var model = new TenantElectricityData
            {
                TenantEmail = contract.mTenant.tenantEmail,
                UnitNumber = contract.IdCompound.HasValue ? contract.mCompoundUnits.UnitNumber : contract.mUnit.UnitNumber,
                BuildingNumber = contract.IdCompound.HasValue ? contract.mCompoundUnits.mCompoundBuilding.BuildingNumber : contract.mUnit.mBuilding.BuildingName,
                ElectricityMeterNumber = electricityData.ElectricityMeterNumber,
                ElectricityPaymentNumber = electricityData.PaymentNumber,
                CompoundName = compoundName
            };
            return View("~/Views/Tenants/SendEmail.cshtml", model);
        }
    }
    public class TenantElectricityData
    {
        public string TenantEmail { get; set; }
        public string BuildingNumber { get; set; }
        public string UnitNumber { get; set; }
        public string ElectricityMeterNumber { get; set; }
        public string ElectricityPaymentNumber { get; set; }
        public string CompoundName { get; set; }
    }
}
