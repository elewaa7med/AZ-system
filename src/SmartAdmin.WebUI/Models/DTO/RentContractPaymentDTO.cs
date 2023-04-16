using System;
using System.Collections.Generic;

namespace SmartAdmin.WebUI.Models.DTO
{
    public class RentContractPaymentDTO
    {
       
        public ICollection<RentContractPayment> RentContractPaymentList { get; set; }
        public ICollection<Invoices> InvoicesList{get;set;}
        public ICollection<RentContractOtherPayment> RentContractOtherPaymentList { get; set; }
    }

    public class RentContractOtherPayment
    {
        public int ID { get; set; }

        public int Commession { get; set; }
        public int Insurence { get; set; }
        public int OtherPayment { get; set; }
        public bool ApplyTax { get; set; }
        public int PaidAmount { get; set; }

        public string UnitNumber { get; set; }
        public string BuldingNumber { get; set; }
        public string Note { get; set; }

        public int UnitRentContractID { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string User { get; set; }

        public string TenantName { get; set; }
        public string InvoiceId { get; set; }
        public int pageid { get; set; }

        public int PaymentMehtod { get; set; }
        public string checkVisaNumber { get; set; }

        public int MonyType { get; set; }
        public long InvoiceIdNmber { get; set; }
        public string OtherPaymentText { get; set; }
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

        public int PropertyTypeId { get; set; }
    }
}
