using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartAdmin.WebUI.Controllers;
using SmartAdmin.WebUI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _services;

        //Inject IServiceProvider
        public Worker(IServiceProvider services, ILogger<Worker> logger)
        {
            _logger = logger;
            _services = services;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using (var scope = _services.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await DoWorkAsync(ctx, stoppingToken);
                    await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
                }
            }
        }

        private async Task DoWorkAsync(ApplicationDbContext ctx, CancellationToken stoppingToken)
        {
            var dueValues = GetListUsersNotPayedDue(ctx);
            foreach (var dueValue in dueValues)
            {
                string WhatsAppMessage = buildMessageBody(dueValue);
                HttpResponseMessage ReturnValue = new HttpResponseMessage();
                string WhatsAppNumber = ValidateWhatsAppNumber(dueValue.TenantWhatsapp);
                if (WhatsAppNumber != null)
                    ReturnValue = await SendWhatsappAction(WhatsAppNumber, WhatsAppMessage);
            }
        }

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
                          + ValueAR + "\n";
            if (dueValue.masterBuilding != 2)
            {
                WhatsAppMessage += "نأمل منكم المبادرة بالسداد عن طريق التحويل الى رقم الآيبان SA8860000000216635326018\n" 
                    +"\n شركة ألف زين للتطوير العقاري\n\n";
            }
            else
            {
                WhatsAppMessage += "نأمل منكم المبادرة بالسداد عن طريق التحويل الى رقم الآيبان SA1160100021695019015001\n"
                    + "\n شركة صروح الشرقية للاستثمار والتنمية\n\n";

            }
            WhatsAppMessage += "Dear customer,\n"
                          + ValueEn + "\n";
            if (dueValue.masterBuilding != 2)
            {
                WhatsAppMessage += "Amount due can be transferred to the IBAN SA8860000000216635326018\n" 
                    +"\n AZ Real Estate Development Company";
            }
            else
            {
                WhatsAppMessage += "Amount due can be transferred to the IBAN SA1160100021695019015001\n"
                    + "\n Eastern Sorooh Inves&Development Co";

            }


            return WhatsAppMessage;
        }

        public List<DueValue> GetListUsersNotPayedDue(ApplicationDbContext _context)
        {
            var currentTime = DateTime.Now;
            var next60DaysTime = currentTime.AddMonths(1);

            var due60 = _context.TUnitRentContract.Include(x => x.mUnit).Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c =>
                                                               (c.mMasterBuilding == 1 || c.mMasterBuilding == 2 || c.mMasterBuilding == 3)
                                                               && !c.Archived
                                                               && !string.IsNullOrEmpty(c.mTenant.tenantEmail)
                                                               && c.remainingAmount > 0
                                                               && c.UnitRentContractPayments.Where(p => !p.Paid).MinOrDefault(p => p.DueDate) != null
                                                              //c.IdRentContract == 12225
                                                              )
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractNumber = t.contractNumber,
                                                      UnitNumber = t.mUnit.UnitNumber ?? t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantWhatsapp = t.mTenant.Whatsapp,
                                                      RemainingDates = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next60DaysTime).OrderBy(e => e.DueDate).Select(e => new DueDatesWithValues { DueDate = e.DueDate, RemainingAmount = e.RemainingAmount }).ToList(),
                                                      Building = t.mUnit.mBuilding.BuildingName ?? t.mCompound.compoundName,
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      masterBuilding = t.mMasterBuilding
                                                  }).Where(x => x.RemainingDates.Count > 0).ToList();

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
            }
            return null;
        }
        public string ValidateWhatsAppNumber(string WhatsAppNumber)
        {
            if (WhatsAppNumber != null)
            {
                if (WhatsAppNumber.StartsWith("+966") || WhatsAppNumber.StartsWith("966"))
                    return WhatsAppNumber;
                if (WhatsAppNumber.StartsWith("00966") || WhatsAppNumber.Length < 8)
                    return null;
                if (WhatsAppNumber.StartsWith("05"))
                {
                    var result = WhatsAppNumber.Substring(1);
                    return "966" + result[1];
                }

                if (WhatsAppNumber.StartsWith("5"))
                {
                    return "966" + WhatsAppNumber;
                }
            }
            return null;
        }

    }
}
