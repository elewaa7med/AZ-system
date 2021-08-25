using System;

namespace SmartAdmin.WebUI.Models
{
    public class UnitRentContractNote
    {
        public int ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UnitRentContractID { get; set; }
        public virtual UnitRentContract UnitRentContract { get; set; }
        public string Note { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }
    }
}
