using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Chat() => View();
        public IActionResult Confirmation() => View();
        public IActionResult Contacts() => View();
        public IActionResult Error() => View();
        public IActionResult Error404() => View();
        public IActionResult ErrorAnnounced() => View();
        public IActionResult Forget() => View();
        public IActionResult ForumDiscussion() => View();
        public IActionResult ForumList() => View();
        public IActionResult ForumThreads() => View();
        public IActionResult InboxGeneral() => View();
        public IActionResult InboxRead() => View();
        public IActionResult InboxWrite() => View();
        public IActionResult Invoice() => View();
        public IActionResult Locked() => View();
        public IActionResult Login() => View();
        public IActionResult LoginAlt() => View();
        public IActionResult Profile() => View();
        public IActionResult Register() => View();
        public IActionResult Search() => View();
        public IActionResult Contract_Building() => View();
        public IActionResult Receipt() => View();
        public IActionResult Electric_mile() => View();
        public IActionResult Invoice_2() => View();
        public IActionResult petty_Cash_Request() => View();



    }
}
