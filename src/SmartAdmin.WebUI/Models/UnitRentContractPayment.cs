using System;
using System.Collections.Generic;

namespace SmartAdmin.WebUI.Models
{
    public class UnitRentContractPayment
    {
        public int ID { get; set; }
        public int Amount { get; set; }
        public int RemainingAmount { get { return Amount - PaidAmount >= 0 ? Amount - PaidAmount : 0; } private set { } }
        public int PaidAmount { get; set; }
        public bool Paid { get; set; }
        public DateTime DueDate { get; set; }
        public int UnitRentContractID { get; set; }
        public virtual UnitRentContract UnitRentContract { get; set; }
        public string Note { get; set; }
        public DateTime? PaymentDate { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }
        public virtual ICollection<UnitRentContractPaymentLog> UnitRentContractPaymentLogs { get; set; } = new HashSet<UnitRentContractPaymentLog>();
        public virtual ICollection<InvoiceRelatedPaymentDates> InvoiceRelatedPaymentDates { get; set; }
    }
    public class UnitRentContractPaymentLog
    {
        public int ID { get; set; }
        public int PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int UnitRentContractPaymentID { get; set; }
        public virtual UnitRentContractPayment UnitRentContractPayment { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }
    }

    public class UnitRentContractAllPaymentLogs
    {
        public int ID { get; set; }
        public int AllPaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int UnitRentContractID { get; set; }
        public virtual UnitRentContract UnitRentContract { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Invoices Invoice {get;set;}
        public int InvoiceID {get;set; }
        public string UserID { get; set; }
        public string Action { get; set; }
    }
}
