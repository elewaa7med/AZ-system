using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
    public class ElectricityMeter
    {
        public int ID { get; set; }
        public string ElectricityMeterNumber { get; set; }
        public string PaymentNumber { get; set; }
        public string StartOfMeter { get; set; }
        public bool TransferTheAccountToTenant { get; set; }
        public int? UnitID { get; set; }
        public int? CompoundUnitID { get; set; }
        public virtual Units Unit { get; set; }
        public virtual CompoundUnits CompoundUnit { get; set; }
        public string Note { get; set; }
        [NotMapped]
        public string Representative { get; set; }
        [NotMapped]
        public int BuildingID { get; set; }
    }
}