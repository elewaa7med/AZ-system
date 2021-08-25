using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartAdmin.WebUI.Models
{
    public class ViewBagFilter : IActionFilter
    {
        private static readonly string Enabled = "Enabled";
        private static readonly string Disabled = string.Empty;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller is Controller ? context.Controller as Controller : null;
            if (controller != null && (controller.ControllerContext.ActionDescriptor.ControllerName != "Account" || controller.ControllerContext.ActionDescriptor.ActionName.ToLower()=="register"))
            {
                // SmartAdmin Toggle Features
                controller.ViewBag.AppSidebar = Enabled;
                controller.ViewBag.AppHeader = Enabled;
                controller.ViewBag.AppLayoutShortcut = Enabled;
                controller.ViewBag.AppFooter = Enabled;
                controller.ViewBag.ShortcutMenu = Enabled;
                controller.ViewBag.ChatInterface = Enabled;
                controller.ViewBag.LayoutSettings = Enabled;

                // SmartAdmin Default Settings
                controller.ViewBag.App = "AZ - Admin";
                controller.ViewBag.AppName = "SmartAdmin WebApp";
                controller.ViewBag.AppFlavor = "Real Estate System";
                controller.ViewBag.AppFlavorSubscript = "System";
                controller.ViewBag.User = context.HttpContext.User.Identity.Name;
                controller.ViewBag.Email = "drlantern@gotbootstrap.com";
                controller.ViewBag.Twitter = "codexlantern";
                controller.ViewBag.Avatar = "avatar-admin.png";
                controller.ViewBag.Version = "4.0.2";
                controller.ViewBag.Bs4v = "4.3";
                controller.ViewBag.Logo = "logo.png";
                controller.ViewBag.LogoM = "logo.png";
                controller.ViewBag.Copyright = "2020 © AZ Realstate";
                controller.ViewBag.CopyrightInverse = "2020 © AZ Realstate";
                //controller.ViewBag.CopyrightInverse = "2019 © AZ Realstate by&nbsp;<a href='https = //www.gotbootstrap.com' class='text-white opacity-40 fw-500' title='gotbootstrap.com' target='_blank'>gotbootstrap.com</a>";
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
