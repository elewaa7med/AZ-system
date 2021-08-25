using System;
using System.Collections.Generic;

namespace SmartAdmin.WebUI.Models.DTO
{
    public class RentContractPaymentDTO
    {
       
        public ICollection<RentContractPayment> RentContractPaymentList { get; set; }
        public ICollection<Invoices> InvoicesList{get;set;}
    }

    public class RentContractPayment
    {
        public int ID { get; set; }

        public int Amount { get; set; }

        public int RemainingAmount { get; set; }

        public int PaidAmount { get; set; }

        public bool Paid { get; set; }

        public DateTime DueDate { get; set; }

        public int UnitRentContractID { get; set; }

        public string Note { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string User { get; set; }

        public bool FullPay { get; set; }

        public string TenantName { get; set; }

        public string UnitNumber { get; set; }

        public string BuldingNumber { get; set; }
        public int pageid { get; set; }

        public decimal TotalPaymentValue { get; set; }

        public int PaymentMehtod { get; set; }

        public string InvoiceId { get; set; }

        public string checkVisaNumber { get; set; }

        public decimal DeservedAmount { get; set; }
    }
}
