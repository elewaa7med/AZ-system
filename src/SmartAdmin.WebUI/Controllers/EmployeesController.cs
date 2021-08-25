using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using SmartAdmin.WebUI.Models.EmployeeVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _user;
        private readonly IFileProvider _fileProvider;

        public EmployeesController(ApplicationDbContext context, UserManager<ApplicationUser> user, IFileProvider fileProvider)
        {
            _context = context;
            _user = user;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            DataTable dt = new DataTable();
            List<EmployeeTbl> empList = new List<EmployeeTbl>();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getVisaExpiries";
                _context.Database.OpenConnection();
                DataTable dataTable = dt;
                dataTable.Load(await command.ExecuteReaderAsync());
                empList = (from DataRow dr in dt.Rows
                           select new EmployeeTbl
                           {
                               IdEmployee = Convert.ToInt32(dr["IdEmployee"]),
                               dtIqamaExpiryDate = dr["dtIqamaExpiryDate"].ToString(),
                               fullName = dr["fullName"].ToString(),
                               iqamaNumber = dr["iqamaNumber"].ToString(),
                               mobile = dr["mobile"].ToString(),
                               CountryName = dr["CountryName"].ToString(),
                               remaining_days = Convert.ToInt32(dr["remaining_days"]),
                               jobTitle = dr["jobTitle"].ToString(),
                               passportNumber = dr["passportNumber"].ToString(),
                               workplace = dr["workplace"].ToString(),
                               IsCompanykafala = (bool)dr["IsCompanykafala"]
                           }).ToList();
            }
            return View(empList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Employees employees = await _context.TEmployees.Include((Employees e) => e.Tnationality).SingleOrDefaultAsync((Employees m) => (int?)m.IdEmployee == id);
            if (employees == null)
            {
                return NotFound();
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", employees.IdNationality);
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace", employees.IdWorkPlace);
            return View(employees);
        }

        public IActionResult Create()
        {
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName");
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(new string[]
        {
            "IdEmployee,fullName,eMail,mobile,gender,IdNationality,dtDateOfBirth,iqamaPic,iqamaNumber,dtIqamaExpiryDate,dtpassPortExpiry,dtcontractExpiryDate,jobTitle,empPicture,Notes,isDeleted,passportNumber,IdWorkPlace,dtJoiningDate,passPortPic,FlatContractFile,WorkContractFile,IsCompanykafala"
        })] Employees employees, IFormFile empImageFile, IFormFile iqamaImageFile, IFormFile passPortImageFile, IFormFile FlatContractFile, IFormFile WorkContractFile)
        {
            if (base.ModelState.IsValid)
            {
                if (empImageFile != null && empImageFile.Length != 0L)
                {
                    employees.empPicture = await SaveFileToDirectory(empImageFile, "empImages");
                }
                if (iqamaImageFile != null && iqamaImageFile.Length != 0L)
                {
                    employees.iqamaPic = await SaveFileToDirectory(iqamaImageFile, "iqamaImages");
                }
                if (passPortImageFile != null && passPortImageFile.Length != 0L)
                {
                    employees.passPortPic = await SaveFileToDirectory(passPortImageFile, "passportImages");
                }
                //by auf
                if (FlatContractFile != null && FlatContractFile.Length != 0L)
                {
                    employees.FlatContractFile = await SaveFileToDirectory(FlatContractFile, "FlatContractFiles");
                }
                if (WorkContractFile != null && WorkContractFile.Length != 0L)
                {
                    employees.WorkContractFile = await SaveFileToDirectory(WorkContractFile, "WorkContractFiles");
                }
                //end auf

                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    employees.IdCreated = _user.GetUserId(base.HttpContext.User);
                }
                employees.dtCreated = DateTime.Now;
                _context.Add(employees);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Employee. Please correct and try again.";
                }
                return RedirectToAction("Index");
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", employees.IdNationality);
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace", employees.IdWorkPlace);
            return View(employees);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Employees employees = await _context.TEmployees.SingleOrDefaultAsync((Employees m) => (int?)m.IdEmployee == id);
            if (employees == null)
            {
                return NotFound();
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", employees.IdNationality);
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace", employees.IdWorkPlace);
            return View(employees);
        }

        private async Task<string>  SaveFileToDirectory(IFormFile file,string folderName="EmployeeFiles")
        {
            var _fileDeafaultPath = ((PhysicalFileProvider)_fileProvider).Root;
            if (file != null && file.Length != 0L)
            {
                var empImagePath = Path.Combine(_fileDeafaultPath, folderName+"\\");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(new string[]
        {
            "IdEmployee,fullName,eMail,mobile,gender,IdNationality,dtDateOfBirth,iqamaPic,iqamaNumber,dtIqamaExpiryDate,dtpassPortExpiry,dtcontractExpiryDate,jobTitle,empPicture,Notes,idcreated,dtCreate,isDeleted,passportNumber,IdWorkPlace,dtJoiningDate,passPortPic,FlatContractFile,WorkContractFile,IsCompanykafala"
        })] Employees employees, IFormFile empImageFile, IFormFile iqamaImageFile, IFormFile passPortImageFile, IFormFile FlatContractFile, IFormFile WorkContractFile)
        {
         
            if (id != employees.IdEmployee)
            {
                return NotFound();
            }
            if (base.ModelState.IsValid)
            {
                try
                {
                    if (empImageFile != null && empImageFile.Length != 0L)
                    {
                        employees.empPicture = await SaveFileToDirectory(empImageFile, "empImages");
                    }
                    if (iqamaImageFile != null && iqamaImageFile.Length != 0L)
                    {
                        employees.iqamaPic =  await SaveFileToDirectory(iqamaImageFile, "iqamaImages");
                    }
                    if (passPortImageFile != null && passPortImageFile.Length != 0L)
                    {
                        employees.passPortPic = await SaveFileToDirectory(passPortImageFile, "passportImages");
                    }
                    //by auf
                    if (FlatContractFile != null && FlatContractFile.Length != 0L)
                    {
                        employees.FlatContractFile = await SaveFileToDirectory(FlatContractFile, "FlatContractFiles");
                    }
                    if (WorkContractFile != null && WorkContractFile.Length != 0L)
                    {
                        employees.WorkContractFile = await SaveFileToDirectory(WorkContractFile, "WorkContractFiles");
                    }
                    //end auf

                    if (_user.GetUserName(base.HttpContext.User) != null)
                    {
                        employees.IdModified = _user.GetUserId(base.HttpContext.User);
                    }
                    employees.dtModified = DateTime.Now;
                    _context.Update(employees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.IdEmployee))
                    {
                        return NotFound();
                    }
                    base.ViewData["AlertSaveErr"] = "There is an Error Savng the Employee. Please correct and try again.";
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                return RedirectToAction("Index");
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", employees.IdNationality);
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace", employees.IdWorkPlace);
            return View(employees);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            Employees employees = await _context.TEmployees.Include((Employees e) => e.Tnationality).SingleOrDefaultAsync((Employees m) => (int?)m.IdEmployee == id);
            if (employees == null)
            {
                return NotFound();
            }
            base.ViewData["IdNationality"] = new SelectList(_context.TNationalities, "IdCountry", "CountryName", employees.IdNationality);
            base.ViewData["IdWorkPlace"] = new SelectList(_context.TWorkplaces, "IdWorkPlace", "workPlace", employees.IdWorkPlace);
            return View(employees);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Employees employees = await _context.TEmployees.SingleOrDefaultAsync((Employees m) => m.IdEmployee == id);
            employees.isDeleted = 1;
            _context.Update(employees);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EmployeesExists(int id)
        {
            return _context.TEmployees.Any((Employees e) => e.IdEmployee == id);
        }

        public async Task<JsonResult> getIqamaExpiries()
        {
            DataTable tbl = new DataTable();
            using (DbCommand command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "getVisaExpiries";
                _context.Database.OpenConnection();
                DataTable dataTable = tbl;
                dataTable.Load(await command.ExecuteReaderAsync());
            }
            return Json(tbl.Rows);
        }
    }
}
