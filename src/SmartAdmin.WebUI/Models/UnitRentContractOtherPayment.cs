using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class UnitRentContractOtherPayment
    {
        public int ID { get; set; }
        public int Commession { get; set; }
        public int Insurence { get; set; }
        public int OtherPayment { get; set; }
        public int PaidAmount { get; set; }
        public bool ApplyTax { get; set; }
        public string Note { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int MonyType { get; set; }
        public int UnitRentContractID { get; set; }
        public virtual UnitRentContract UnitRentContract { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }
        public string OtherPaymentText { get; set; }
    }
}
