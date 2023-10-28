using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdmin.WebUI.Data;
using SmartAdmin.WebUI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860v
//ToString("#,##0") => 2,200

namespace SmartAdmin.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailOfDueDatesController : ControllerBase
    {
        private readonly IEmailSender emailSender;
        private readonly ApplicationDbContext _context;
        public SendEmailOfDueDatesController(IEmailSender email, ApplicationDbContext context)
        {
            emailSender = email;
            _context = context;
        }
        // GET: api/<SendEmailOfDueDatesController>
        [HttpGet]
        public async Task<string> GetAsync()

        {

            var dueValues = GetListUsersNotPayedDue();
            try
            {
                foreach (var item in dueValues)
                {
                    string body = $@"<div> Dear {item.TenantName},</div>" +
                        $@"<div>We just remmenber you about unit rentets </div> 
                           <table  border='1' style='border-collapse:collapse'>
                                    <tr>
                                            <th>
                                                  UnitNumber  
                                            </th>
                                            <th>
                                                  Building  
                                            </th>
                                            <th>
                                                  Due Date  
                                            </th>
                                            <th>
                                                  Remaining Amount  
                                            </th>
                                    </tr>";
                    foreach (var date in item.RemainingDates)
                    {
                        body += $@"
                                    <tr>
                                            <td>
                                                  {item.UnitNumber} 
                                            </td>
                                            <td>
                                                 {item.Building} 
                                            </td>
                                            <td>
                                                 {date.DueDate.ToShortDateString()} 
                                            </td>
                                            <td>
                                                {date.RemainingAmount.ToString("C", CultureInfo.CreateSpecificCulture("ar-SA"))}  
                                            </td>
                                    </tr>
                                   
                                ";

                        try
                        {
                            var client = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, "https://go-wloop.net/api/v1/message/send");
                            request.Headers.Add("Accept", "application/json");
                            request.Headers.Add("AUTHORIZATION", "Bearer 6f8ad056d8d39e466b789264d0960ba4_Qazbzeb0kJEPhOWpsCHDjVmtja5B0iqY8IRfFWVn");
                            var content = new MultipartFormDataContent();
                            string WhatsApp = item.TenantWhatsapp;
                            content.Add(new StringContent(WhatsApp), "phone");
                            content.Add(new StringContent("عزيزنا المستاجر :" +
                                "برجاء سرعة سداد الايجار للوحده رقم: " + item.UnitNumber + " قى المبنى رقم: " + item.Building + " للعقد رقم " + item.ContractNumber + " بتاريخ " + date.DueDate + " بقيمة: " + date.RemainingAmount), "body");
                            request.Content = content;
                            if (!string.IsNullOrEmpty(WhatsApp))
                            {
                                var response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();
                            }
                        }
                        catch (Exception ex)
                        {
                            return ex.Message.ToString();
                            throw;
                        }

                    }
                    body += $@" <tr>
                                    <th colspan = '3' >
                                        Total
                                    </th >
 

                                     <th>
                                        {item.Value.ToString("C", CultureInfo.CreateSpecificCulture("ar-SA"))}
                                     </th >
                                </tr >
                            </table>";

                    body += $"<div> total days {item.TotalDays} </div>";
                    body += $"<h2>Thanks</h2>";
                    //TODO: remove my mail 
                    emailSender.SendEmailHtmlBodyAsync(
                      email: item.TenantEmail,
                     subject: "Your Unit Due Date",
                     message: body);
                    ////TODO: remove break no jsut for test 
                    //break;
                }


                return "Done";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
                throw;
            }

        }

        private List<DueValue> GetListUsersNotPayedDue()
        {
            var resutl = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments).Where(e => e.contractNumber.Contains("380"));
            var currentTime = DateTime.Now;
            var next30DaysTime = currentTime.AddMonths(1);

            var due60 = _context.TUnitRentContract.Include(t => t.UnitRentContractPayments)
                                                  .Include(t => t.mTenant)
                                                  .Where(c =>
                                                               !c.Archived
                                                               && !string.IsNullOrEmpty(c.mTenant.tenantEmail)
                                                               && c.remainingAmount > 0
                                                               && c.UnitRentContractPayments.Where(p => !p.Paid).MinOrDefault(p => p.DueDate) != null
                                                              )
                                                  .Select(t => new DueValue
                                                  {
                                                      ContractID = t.IdRentContract,
                                                      ContractNumber = t.contractNumber,
                                                      UnitNumber = t.mUnit.UnitNumber ?? t.mCompoundUnits.UnitNumber ?? string.Empty,
                                                      TenantName = t.mTenant.tenantName,
                                                      TenantEmail = t.mTenant.tenantEmail,
                                                      TenantWhatsapp = t.mTenant.Whatsapp,
                                                      Mobile = t.mTenant.tenantMobile,
                                                      AnnualRent = t.yearlyRent,
                                                      RemainingRents = t.UnitRentContractPayments.Count(p => !p.Paid),
                                                      Value = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next30DaysTime).Sum(p => p.RemainingAmount),
                                                      CollectionDate = t.UnitRentContractPayments.Where(p => !p.Paid).Min(p => p.DueDate).ToShortDateString(),
                                                      RemainingDates = t.UnitRentContractPayments.Where(p => !p.Paid && p.DueDate < next30DaysTime).OrderBy(e => e.DueDate).Select(e => new DueDatesWithValues { DueDate = e.DueDate, RemainingAmount = e.RemainingAmount }).ToList(),
                                                      ExpiryDate = t.dtLeaseEnd.ToShortDateString(),
                                                      TotalDays = t.UnitRentContractPayments.Where(p => !p.Paid).Min(d => d.DueDate).Subtract(currentTime).Days,
                                                      Building = t.mUnit.mBuilding.BuildingName ?? t.mCompound.compoundName,
                                                      Mandoob = t.Mandoob.fullName,
                                                      MandoobPhone = t.Mandoob.PhoneNumber,
                                                      Note = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.Note).FirstOrDefault() ?? string.Empty,
                                                      NoteID = t.UnitRentContractNotes.OrderByDescending(e => e.CreatedOn).Select(e => e.ID).FirstOrDefault()

                                                  }).OrderBy(t => t.TotalDays).ToList();

            return due60;
        }
    }
}
