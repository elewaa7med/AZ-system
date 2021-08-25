using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.ViewModels;
using SmartAdmin.WebUI.Services;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class LegalController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _user;
        public LegalController(ApplicationDbContext context, UserManager<ApplicationUser> user)
        {
            _db = context;
            _user = user;
        }
        // GET: LegalController
        public ActionResult Index(int? id, int? compoundID)
        {

            //string times = "2:25:00+0:45:00+1:15:00+1:56:00+3:30:00+1:30:00+2:15:00+2:30:00+3:15:00+2:15:00+3:00:00+5:15:00+1:59:00+4:00:00+1:59:00+2:00:00+10:59:00";
            //List<TimeSpan> timeSpans = new List<TimeSpan>();
            //foreach (var t in times.Split('+'))
            //{
            //    timeSpans.Add(TimeSpan.Parse(t));
            //}
            //TimeSpan total = new TimeSpan(0, 0, 0);
            //foreach (var item in timeSpans)
            //{
            //    total = total.Add(item);
            //}
            //var c = total;
            var currentTime = DateTime.Now;
            IEnumerable<Legal> query;
            if (id != 0 && (compoundID == null || compoundID == 0))
            {
                ViewBag.page = id;
                query = _db.Legals.Where(x => x.UnitRentContract.mMasterBuilding == id && x.UnitRentContract.IdCompound == null)
                   .Include(e => e.UnitRentContract)
                   .Include(e => e.UnitRentContract.mTenant)
                   .Include(e => e.UnitRentContract.mTenant.mNationality)
                   .Include(e => e.UnitRentContract.Mandoob)
                   .Include(e => e.UnitRentContract.mUnit)
                   .Include(e => e.UnitRentContract.mUnit.mBuilding)
                   .Include(e => e.UnitRentContract.mCompound)
                   .Include(e => e.UnitRentContract.mCompoundUnits)
                   .Include(e => e.UnitRentContract.UnitRentContractNotes)
                   .Include(e => e.UnitRentContract.UnitRentContractPayments);
            }
            else
            {
                ViewBag.page = _db.TCompounds.Where(e => e.IdCompound == compoundID).Select(x => x.compoundName).FirstOrDefault();
                query = _db.Legals.Where(x => x.UnitRentContract.IdCompound == compoundID)
                  .Include(e => e.UnitRentContract)
                  .Include(e => e.UnitRentContract.mTenant)
                  .Include(e => e.UnitRentContract.mTenant.mNationality)
                  .Include(e => e.UnitRentContract.Mandoob)
                  .Include(e => e.UnitRentContract.mUnit)
                  .Include(e => e.UnitRentContract.mUnit.mBuilding)
                  .Include(e => e.UnitRentContract.mCompound)
                  .Include(e => e.UnitRentContract.mCompoundUnits)
                  .Include(e => e.UnitRentContract.UnitRentContractNotes)
                  .Include(e => e.UnitRentContract.UnitRentContractPayments);
            }
            var result = query.Select(e => new LegalListViewModel
            {
                Id = e.Id,
                TenentName = e.UnitRentContract.mTenant.tenantName,
                TenentNationality = e.UnitRentContract.mTenant.mNationality.CountryName,
                IsBulding = e.UnitRentContract.IdUnit != null,
                RentContractId = e.UnitRentContract.IdRentContract,
                NoteId = e.UnitRentContract.UnitRentContractNotes.OrderByDescending(t => t.CreatedOn).Select(t => t.ID).FirstOrDefault(),
                BuldingNumber = e.UnitRentContract.mUnit != null ?
                                e.UnitRentContract.mUnit.mBuilding.BuildingName
                              : e.UnitRentContract.mCompound.compoundName,
                DisplayType = e.UnitRentContract.mUnit == null ? "Compound" : "Appartment",
                UnitNumber = e.UnitRentContract.mUnit != null ?
                            e.UnitRentContract.mUnit.UnitNumber
                          : e.UnitRentContract.mCompoundUnits.UnitNumber,
                ContractDate = e.UnitRentContract.dtLeaseStart.ToShortDateString(),
                DueDate = e.UnitRentContract.UnitRentContractPayments != null && e.UnitRentContract.UnitRentContractPayments.Count() > 0
                              ? (e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid).Count() > 0
                              ? e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString()
                              : string.Empty)
                              : string.Empty,
                DelayedRent = e.UnitRentContract.UnitRentContractPayments != null && e.UnitRentContract.UnitRentContractPayments.Count() > 0
                               ? (e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Count() > 0
                               ? e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Sum(p => p.RemainingAmount)
                               : 0)
                               : 0,
                ElectricityBill = e.ElectricityBill,
                WaterBill = e.WalterBill,
                MandoobName = e.UnitRentContract.Mandoob.fullName,
                RequestRaiseDate = e.RequestRaiseDate.Value.ToShortDateString(),
                RequestSubmitDate = e.RequestSubmitDate.ToShortDateString(),
                Note = e.UnitRentContract.UnitRentContractNotes.OrderByDescending(t => t.CreatedOn).Select(t => t.Note).FirstOrDefault() ?? string.Empty,
                pageId = e.UnitRentContract.mMasterBuilding,
                compoundId = compoundID
            }).ToList();

            return View(result);
        }

        // GET: LegalController/Details/5
        public ActionResult Details(int id, string pageid)
        {
            var currentTime = DateTime.Now;
            var result = _db.Legals
                .Include(e => e.UnitRentContract)
                .Include(e => e.UnitRentContract.mTenant)
                .Include(e => e.UnitRentContract.mTenant.mNationality)
                .Include(e => e.UnitRentContract.Mandoob)
                .Include(e => e.UnitRentContract.mUnit)
                .Include(e => e.UnitRentContract.mUnit.mBuilding)
                .Include(e => e.UnitRentContract.mCompound)
                .Include(e => e.UnitRentContract.mCompoundUnits)
                .Select(e => new LegalListViewModel
                {
                    Id = e.Id,
                    TenentName = e.UnitRentContract.mTenant.tenantName,
                    TenentNationality = e.UnitRentContract.mTenant.mNationality.CountryName,
                    IsBulding = e.UnitRentContract.IdUnit != null,
                    RentContractId = e.UnitRentContract.IdRentContract,
                    NoteId = e.UnitRentContract.UnitRentContractNotes.OrderByDescending(t => t.CreatedOn).Select(t => t.ID).FirstOrDefault(),
                    BuldingNumber = e.UnitRentContract.mUnit != null ?
                                e.UnitRentContract.mUnit.mBuilding.BuildingName
                              : e.UnitRentContract.mCompound.compoundName,
                    UnitNumber = e.UnitRentContract.mUnit != null ?
                                e.UnitRentContract.mUnit.UnitNumber
                              : e.UnitRentContract.mCompoundUnits.UnitNumber,
                    ContractDate = e.UnitRentContract.dtLeaseStart.ToShortDateString(),
                    DueDate = e.UnitRentContract.UnitRentContractPayments != null && e.UnitRentContract.UnitRentContractPayments.Count() > 0
                                 ? (e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid).Count() > 0
                                 ? e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString()
                                 : string.Empty)
                                 : string.Empty,
                    DelayedRent = e.UnitRentContract.UnitRentContractPayments != null && e.UnitRentContract.UnitRentContractPayments.Count() > 0
                                  ? (e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Count() > 0
                                  ? e.UnitRentContract.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate <= currentTime).Sum(p => p.RemainingAmount)
                                  : 0)
                                  : 0,
                    ElectricityBill = e.ElectricityBill,
                    WaterBill = e.WalterBill,
                    MandoobName = e.UnitRentContract.Mandoob.fullName,
                    RequestRaiseDate = e.RequestRaiseDate.Value.ToShortDateString(),
                    RequestSubmitDate = e.RequestSubmitDate.ToShortDateString(),
                    Note = e.UnitRentContract.UnitRentContractNotes.OrderByDescending(t => t.CreatedOn).Select(t => t.Note).FirstOrDefault() ?? string.Empty,
                    pageId = e.UnitRentContract.mMasterBuilding
                }).FirstOrDefault(e => e.Id == id);
            ViewBag.page = pageid;
            return View(result);
        }

        // GET: LegalController/Create
        public ActionResult Create(int? id, int? IdRentContract)
        {
            int value = 0;
            if (id != null)
            {
                int.TryParse(id.ToString(), out value);
            }
            else
            {
                int.TryParse(IdRentContract.ToString(), out value);

            }
            var legalListCount = _db.Legals.Count() + 1;
            var model = new Legal
            {
                RequestNumber = "Leg-" + legalListCount,
                IdRentContract = value,
                UnitRentContract = _db.TUnitRentContract.Include(x => x.mCompound).Include(x => x.mUnit).FirstOrDefault(x => x.IdRentContract == value)
            };
            return View(model);
        }

        // POST: LegalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(new string[]
        {
            "IdRentContract,RequestNumber,RequestSubmitDate,RequestRaiseDate,ElectricityBill,WalterBill,"
        })]Legal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rentContractEntity = _db.TUnitRentContract.FirstOrDefault(e => e.IdRentContract == model.IdRentContract);
                    if (rentContractEntity == null)
                    {
                        return BadRequest();
                    }
                    rentContractEntity.AddedtoCourt = true;
                    AuditCreateModel(model);
                    _db.Legals.Add(model);
                    _db.SaveChanges();
                    rentContractEntity.LegalId = model.Id;
                    _db.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                if (model.UnitRentContract.IdCompound != null)
                {
                    return RedirectToAction(nameof(Index), new { compoundID = model.UnitRentContract.IdCompound });
                }
                else
                {
                    return RedirectToAction(nameof(Index), new { id = model.UnitRentContract.mMasterBuilding });
                }
            }
            catch (Exception ex)
            {
                return View(model);
                throw ex;
            }
        }


        // GET: LegalController/Edit/5
        public ActionResult Edit(int id, string pageid)
        {
            ViewBag.page = pageid;
            var entity = _db.Legals.FirstOrDefault(e => e.Id == id);
            return View(entity);
        }

        // POST: LegalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Legal model, string pageid)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AuditUpdateModel(model);
                    _db.Legals.Update(model);
                    StopTracking(model);
                    _db.SaveChanges();
                }
                else
                {
                    return View(model);
                }
                var value = _db.Legals.Include(x => x.UnitRentContract).FirstOrDefault(x => x.Id == model.Id);
                if (value.UnitRentContract.IdCompound != null)
                {
                    return RedirectToAction(nameof(Index), new { compoundID = value.UnitRentContract.IdCompound });
                }
                else
                {
                    return RedirectToAction(nameof(Index), new { id = pageid });
                }
            }
            catch
            {
                return View(model);
            }
        }

        public ActionResult UndoLegal(int id, string pageid)//rencontractid
        {
            try
            {
                var legalEntity = _db.Legals.Include(e => e.UnitRentContract).FirstOrDefault(e => e.Id == id);
                if (legalEntity == null)
                {
                    return BadRequest();
                }

                legalEntity.UnitRentContract.AddedtoCourt = false;
                legalEntity.UnitRentContract.LegalId = null;
                _db.Legals.Remove(legalEntity);
                _db.SaveChanges();

                if (legalEntity.UnitRentContract.IdCompound != null)
                {
                    return RedirectToAction(nameof(Index), new { compoundID = legalEntity.UnitRentContract.IdCompound });
                }
                else
                {
                    return RedirectToAction(nameof(Index), new { id = pageid });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: LegalController/Delete/5
        public ActionResult Delete(int id)
        {
            UnitRentContract unitRentContract = null;
            try
            {
                var legalEntity = _db.Legals
                     .Include(e => e.UnitRentContract)
                     .Include(e => e.UnitRentContract.mCompoundUnits)
                     .Include(e => e.UnitRentContract.mUnit)
                     .FirstOrDefault(e => e.Id == id);

                if (legalEntity == null)
                {
                    return BadRequest();
                }

                unitRentContract = legalEntity.UnitRentContract;
                //free Unit unit to be rent agian
                if (unitRentContract.mUnit != null)
                {
                    unitRentContract.mUnit.isRented = false;
                    unitRentContract.mUnit.UnitRentContractID = null;
                }
                //free compund unit to be rent agian
                if (unitRentContract.mCompoundUnits != null)
                {
                    unitRentContract.mCompoundUnits.isRented = false;
                    unitRentContract.mCompoundUnits.UnitRentContractID = null;
                }

                _db.Legals.Remove(legalEntity);
                _db.TUnitRentContract.Remove(unitRentContract);
                _db.SaveChanges();
                //await setRentInfoInAllUnits();

                // H1
                TempData["success"] = "Operation is done successfully";
            }
            catch
            {
                TempData["AlertSaveErr"] = "There is an Error when Delete . Please correct and try again.";
            }
            if (unitRentContract.IdCompound != null)
            {
                return RedirectToAction(nameof(Index), new { compoundID = unitRentContract.IdCompound });
            }
            else
            {
                return RedirectToAction(nameof(Index), new
                {
                    id = unitRentContract.mMasterBuilding
                });
            }
        }




        //by auf
        // json
        public JsonResult GetCourtBuildingsDueOver60()
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("AccountantManager") || User.IsInRole("Accountant");
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);
            var dues = _db.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c => (isAdmin ? true : c.IdMandoob == _user.GetUserId(User)) &&
                                                              !c.Archived && c.remainingAmount > 0 && !c.IdCompound.HasValue &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).MinOrDefault(p => p.DueDate) != null &&
                                                               c.UnitRentContractPayments.Where(p => !p.Paid && p.PaidAmount == 0).Min(p => p.DueDate).Subtract(currentTime).Days <= -16
                                                               && !c.Archived
                                                               && c.AddedtoCourt)
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
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault()
                                                  }).OrderBy(t => t.TotalDays).ToList();
            return Json(new DueViewModel
            {
                Due = dues,
                TotalValue = dues.Sum(d => d.Value)
            });
        }

        //private helper

        private void AuditCreateModel(BaseEntity model)
        {
            var currentUserId = _user.GetUserId(base.HttpContext.User);
            model.CreatedBy = Guid.Parse(currentUserId);
            model.CreatedOn = DateTime.Now;

        }
        private void AuditUpdateModel(BaseEntity model)
        {
            var currentUserId = _user.GetUserId(base.HttpContext.User);
            model.UpdatedBy = Guid.Parse(currentUserId);
            model.UpdatedOn = DateTime.Now;

        }
        public void StopTracking(BaseEntity model)
        {
            _db.Entry(model).Property(e => e.CreatedBy).IsModified = false;
            _db.Entry(model).Property(e => e.CreatedOn).IsModified = false;
        }
    }
}
