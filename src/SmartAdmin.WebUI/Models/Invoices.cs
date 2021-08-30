using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class Invoices
    {
        [Key]
        public int Id { get; set; }
        public long InvoiceId {get;set;}
        public decimal Payment { get; set; }
        public int PaymentMehtod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string checkVisaNumber { get; set; }
        public int ContractId { get; set; }
        public int? Status { get; set; }
        public virtual UnitRentContract unitRentContract { get; set; }

        public virtual ICollection<InvoiceRelatedPaymentDates> invoiceRelatedPaymentDates { get; set; }
    }
}
