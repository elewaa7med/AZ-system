using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Extensions;
using SmartAdmin.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Controllers
{
    [AllowAnonymous]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Invoice(string v)
        {
            int.TryParse(DecryptString(v, ConstantValue.EncriptionKey), out int InvoiceId);
            if (InvoiceId == 0)
            {
                return NotFound();
            }
            Invoices Model = null;
            var Invoice = _context.Invoices.Include(x => x.invoiceRelatedPaymentDates).ThenInclude(x => x.unitRentContractPayment).ThenInclude(x => x.UnitRentContract);

            if (Invoice.ThenInclude(x => x.mUnit).FirstOrDefault(x => x.Id == InvoiceId).invoiceRelatedPaymentDates.FirstOrDefault().unitRentContractPayment.UnitRentContract.mUnit != null)
            {
                Model = Invoice.ThenInclude(x => x.mUnit).ThenInclude(x => x.mBuilding).FirstOrDefault(x => x.Id == InvoiceId);
                Model.invoiceRelatedPaymentDates.FirstOrDefault().unitRentContractPayment.UnitRentContract.mTenant = _context.TTenants.Include(x => x.mCompany).FirstOrDefault(x => x.IdTenant == Model.invoiceRelatedPaymentDates.FirstOrDefault().unitRentContractPayment.UnitRentContract.IdTenant);
            }
            else
            {
                Model = Invoice
                                .ThenInclude(x => x.mCompoundUnits)
                                .ThenInclude(x => x.mCompoundBuilding)
                                .ThenInclude(x => x.mCompound)
                                .Include(x => x.invoiceRelatedPaymentDates)
                                .ThenInclude(x => x.unitRentContractPayment)
                                .ThenInclude(x => x.UnitRentContract)
                                .ThenInclude(x => x.mCompoundUnits)
                                .ThenInclude(x => x.mCompoundBuilding)
                                .ThenInclude(x => x.mCompoundUnits)
                                .Include(x => x.invoiceRelatedPaymentDates)
                                .ThenInclude(x => x.unitRentContractPayment)
                                .ThenInclude(x => x.UnitRentContract)
                                .ThenInclude(x => x.mTenant)
                                .ThenInclude(x => x.mCompany)
                                .FirstOrDefault(x => x.Id == InvoiceId);
            }
            ViewBag.Url = "\\QRs\\QR" + Model.Id + ".png";
            return View("Invoice_2", Model);
        }

        public IActionResult OtherInvoice(string v)
        {
            int.TryParse(DecryptString(v, ConstantValue.EncriptionKey), out int InvoiceId);
            if (InvoiceId == 0)
            {
                return NotFound();
            }
            Invoices Model = null;
            var OtherInvoice = _context.Invoices.Include(x => x.UnitRentContractOtherPayment).ThenInclude(x => x.UnitRentContract);
            if (OtherInvoice.ThenInclude(x => x.mUnit).FirstOrDefault(x => x.UnitRentContractOtherPayment.ID == InvoiceId).UnitRentContractOtherPayment.UnitRentContract.mUnit != null)
            {
                Model = OtherInvoice.ThenInclude(x => x.mUnit).ThenInclude(x => x.mBuilding).FirstOrDefault(x => x.UnitRentContractOtherPayment.ID == InvoiceId);
                Model.UnitRentContractOtherPayment.UnitRentContract.mTenant = _context.TTenants.Include(x => x.mCompany).FirstOrDefault(x => x.IdTenant == Model.UnitRentContractOtherPayment.UnitRentContract.IdTenant);
            }
            else
            {
                Model = OtherInvoice.Include(x => x.UnitRentContractOtherPayment)
                    .ThenInclude(x => x.UnitRentContract)
                    .ThenInclude(x => x.mCompoundUnits)
                    .ThenInclude(x => x.mCompoundBuilding)
                    .ThenInclude(x => x.mCompound)
                    .Include(x => x.UnitRentContractOtherPayment)
                    .ThenInclude(x => x.UnitRentContract)
                    .ThenInclude(x => x.mCompoundUnits)
                    .ThenInclude(x => x.mCompoundBuilding)
                    .ThenInclude(x => x.mCompoundUnits)
                    .Include(x => x.UnitRentContractOtherPayment)
                    .ThenInclude(x => x.UnitRentContract)
                    .ThenInclude(x => x.mTenant)
                    .ThenInclude(x => x.mCompany)
                    .FirstOrDefault(x => x.UnitRentContractOtherPayment.ID == InvoiceId);

            }
            ViewBag.Url = "\\QRs\\QR" + InvoiceId + ".png";
            return View("OtherInvoice_2", Model);

        }

        [NonAction]
        public static string DecryptString(string cipherText, string keyString)
        {
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(keyString, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
