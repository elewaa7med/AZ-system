using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace SmartAdmin.WebUI.Models
{
    public class MaintenanceModel
    {
        public int? UnitID { get; set; }
        public int? CompoundUnitID { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public string UserID { get; set; }
        public DateTime MaintenanceEndDate { get; set; }
        public virtual Units Unit { get; set; }
        public virtual CompoundUnits CompoundUnit { get; set; }
        public virtual ApplicationUser User { get; set; }
        public decimal Plumbing { get; set; }
        public string PlumbingDesc { get; set; }
        public decimal Electricity { get; set; }
        public string ElectricityDesc { get; set; }
        public decimal Paint { get; set; }
        public string PaintDesc { get; set; }
        public decimal Tiles { get; set; }
        public string TilesDesc { get; set; }
        public decimal Toilet { get; set; }
        public string ToiletDesc { get; set; }
        public decimal WaterHeater { get; set; }
        public string WaterHeaterDesc { get; set; }
        public decimal Kitchen { get; set; }
        public string KitchenDesc { get; set; }
        public decimal Conditioning { get; set; }
        public string ConditioningDesc { get; set; }
        public decimal Carpentry { get; set; }
        public string CarpentryDesc { get; set; }
        public decimal Waste { get; set; }
        public string WasteDesc { get; set; }
        public decimal Others { get; set; }
        public string OthersDesc { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
