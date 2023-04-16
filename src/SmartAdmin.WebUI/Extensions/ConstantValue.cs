using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Extensions
{
    public class ConstantValue
    {
        public static string EncriptionKey { get; set; } = "Df43TSDDDDDCD5931069B522E695D4F2";
        public static string BaseUrl { get; set; } = "http://systems.az-ltd.com";
        //public static string BaseUrl { get; set; } = "http://localhost:44363";
        public static string globalInvoicePath { get; set; } = "/Invoice/Invoice?v=";
        public static string globalOtherInvoicePath { get; set; } = "/Invoice/OtherInvoice?v=";
    }
}
