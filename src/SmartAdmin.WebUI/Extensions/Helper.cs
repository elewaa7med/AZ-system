using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Controllers;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Extensions
{
    public class Helper
    {
        public string buildMessageBody(DueValue dueValue)
        {
            string ValueAR = null;
            string ValueEn = null;
            foreach (var date in dueValue.RemainingDates)
            {
                ValueAR += "\nنود تذكيركم بسداد مبلغ الايجار المستحق بتاريخ " + date.DueDate.ToShortDateString() + " بمبلغ " + date.RemainingAmount.ToString("N0", CultureInfo.InvariantCulture) + " ريال.";
                ValueEn += "\nKind reminder for the rent due on " + date.DueDate.ToShortDateString() + " for the amount of " + date.RemainingAmount.ToString("N0", CultureInfo.InvariantCulture) + " Riyals.";
            }
            string WhatsAppMessage =
                        "عميلنا العزيز،\n"
                        + ValueAR + "\n"
                        + "نأمل منكم المبادرة بالسداد عن طريق التحويل الى رقم الآيبان SA8860000000216635326018\n";
            if (dueValue.masterBuilding != 2)
            {
                WhatsAppMessage += "\n شركة ألف زين للتطوير العقاري\n\n";
            }
            else
            {
                WhatsAppMessage += "\n شركة صروح للتطوير العقاري\n\n";

            }
            WhatsAppMessage += "Dear customer,\n"
                          + ValueEn + "\n"
                          + "Amount due can be transferred to the IBAN SA8860000000216635326018\n";
            if (dueValue.masterBuilding != 2)
            {
                WhatsAppMessage += "\n AZ Real Estate Development Company";
            }
            else
            {
                WhatsAppMessage += "\n Srooh Real Estate Development Company";

            }

            return WhatsAppMessage;
        }
        public DueValue GetListUsersNotPayedDue(ApplicationDbContext _context, int unitId, int buildingId)
        {
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);

            var due60 = _context.TUnitRentContract.Include(x => x.mUnit).Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c =>
                                                               !c.Archived
                                                               && !string.IsNullOrEmpty(c.mTenant.tenantEmail)
                                                               && c.remainingAmount > 0
                                                               && c.UnitRentContractPayments.Where(p => !p.Paid).MinOrDefault(p => p.DueDate) != null
                                                               && c.IdUnit == unitId
                                                               && c.mUnit.IdBuilding == buildingId
                                                              )
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractNumber = t.contractNumber,
                                                      UnitNumber = t.mUnit.UnitNumber ?? t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantWhatsapp = t.mTenant.Whatsapp,
                                                      RemainingDates = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next30DaysTime).OrderBy(e => e.DueDate).Select(e => new DueDatesWithValues { DueDate = e.DueDate, RemainingAmount = e.RemainingAmount }).ToList(),
                                                      Building = t.mUnit.mBuilding.BuildingName ?? t.mCompound.compoundName,
                                                      ExpiryDate = t.dtLeaseStart.ToShortDateString()
                                                  }).OrderBy(x => x.ExpiryDate).LastOrDefault();

            return due60;
        }
        public async Task<HttpResponseMessage> SendWhatsappAction(string WhatsappNumber, string TextMessage)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://go-wloop.net/api/v1/message/send");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("AUTHORIZATION", "Bearer 6f8ad056d8d39e466b789264d0960ba4_CW1icMg9V8QUkd6U0RCNnDVQkqUpYNoYFGlOt2lP");
                var content = new MultipartFormDataContent();
                string WhatsApp = WhatsappNumber;
                content.Add(new StringContent(WhatsApp), "phone");
                content.Add(new StringContent(TextMessage), "body");
                request.Content = content;

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }
        public string ValidateWhatsAppNumber(string WhatsAppNumber)
        {
            if (WhatsAppNumber.StartsWith("+966") || WhatsAppNumber.StartsWith("966") || WhatsAppNumber.StartsWith("00966"))
                return WhatsAppNumber;

            if (WhatsAppNumber.StartsWith("05"))
            {
                var result = WhatsAppNumber.Substring(1);
                return "966" + result[1];
            }

            if (WhatsAppNumber.StartsWith("5"))
            {
                var result = WhatsAppNumber.Substring(1);
                return "966" + WhatsAppNumber;
            }
            return null;
        }

    }
}
