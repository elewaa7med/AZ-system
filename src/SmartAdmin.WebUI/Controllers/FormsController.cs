using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class FormsController : Controller
    {
        public IActionResult BasicInputs() => View();
        public IActionResult CheckboxRadio() => View();
        public IActionResult Elements() => View();
        public IActionResult InputGroups() => View();
        public IActionResult Samples() => View();
        public IActionResult Validation() => View();
    }
}
