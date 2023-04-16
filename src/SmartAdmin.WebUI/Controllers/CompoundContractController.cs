using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    public class CompoundContractController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _user;
        private readonly IFileProvider _fileProvider;


        public CompoundContractController(ApplicationDbContext context, UserManager<ApplicationUser> user,
            IAuthorizationService authorizationService, IFileProvider fileProvider)
        {
            _context = context;
            _user = user;
            _authorizationService = authorizationService;
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            return View(_context.compoundContracts.ToList());
        }

        public IActionResult Details(int Id)
        {
            return View(_context.compoundContracts.FirstOrDefault(x=>x.Id == Id));
        }

        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompoundContracts model,IFormFile contractImageFile)
        {
            if (base.ModelState.IsValid)
            {
                if (contractImageFile != null && contractImageFile.Length != 0L)
                {
                    model.contractImage = await SaveFileToDirectory(contractImageFile);
                }
                if (_user.GetUserName(base.HttpContext.User) != null)
                {
                    model.IdCreated = _user.GetUserId(base.HttpContext.User);
                }
                model.dtCreated = DateTime.Now;
            }
            _context.Add(model);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                base.ViewData["AlertSaveErr"] = "There is an Error Savng the Compound Contract. Please correct and try again.";
                return View();
            }
            return RedirectToAction("Index");
        }

       

        //public IActionResult Edit()
        //{
        //    return View();

        //}

        //[HttpPost]
        //public IActionResult Edit(CompoundContracts model)
        //{
        //    return View();

        //}

        public IActionResult Delete(int Id)
        {
            return View(_context.compoundContracts.FirstOrDefault(x => x.Id == Id));
        }

        [HttpPost]
        public IActionResult Delete(CompoundContracts model)
        {
            var compoundContracts = _context.compoundContracts.FirstOrDefault(x => x.Id == model.Id);
            if (compoundContracts != null)
            {
                try
                {
                    _context.Remove(compoundContracts);
                    _context.SaveChanges();
                }
                catch
                {
                    base.ViewData["AlertSaveErr"] = "There is an Error Deleteing  Compound Contract. Please correct and try again.";
                    return View();
                }
            }
            return RedirectToAction("Index");
        }



        private async Task<string> SaveFileToDirectory(IFormFile file, string folderName = "Compoundcontracts")
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

    }
}
