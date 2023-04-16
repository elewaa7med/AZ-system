using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class InvoiceRelatedPaymentDates
    {
        public int Id { get; set; }

        public decimal Amount { get; set; } // For Each Payment
        public int InvoiceId { get; set; }
        public Invoices invoice { get; set; }
        public bool PaymentState { get; set; }
        public int UnitRentContractPaymentId { get; set; }
        public int? Status { get; set; }
        public int? TaxRateId { get; set; }
        public virtual TaxRate TaxRate{get;set;}
        public decimal TaxAmount { get; set; }
        public UnitRentContractPayment unitRentContractPayment { get; set; }
    }
}
