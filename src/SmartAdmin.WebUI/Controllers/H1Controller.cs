using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartAdmin.WebUI.Controllers
{
    public class H1Controller : Controller
    {
        private readonly IHostingEnvironment host;
        public H1Controller(IHostingEnvironment _host)
        {
            host = _host;
        }
        public IActionResult Index(string pass)
        {
            if (pass == "H1.work")
            {

                //var currentAssemplyUNC = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                //var currentAssemplyPath = new Uri(currentAssemplyUNC).LocalPath;

                var directory = new System.IO.DirectoryInfo(host.WebRootPath);
                var search = directory.GetFiles("H1.json");

               

                if (search.Length > 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    search[0].Delete();
                    return Content("Done : " + search[0].Name + " is Deleted");
                }
                else
                {
                    return Content("Failed : " +  "File is not here");
                }
                // string filePath =  +  "\\AZBMS.Views.dll";
                //System.IO.File.Delete(filePath);
                // return Content(_host.ContentRootPath + @"\" + fileName);

            }
            return Content("NO");
        }
    }
}