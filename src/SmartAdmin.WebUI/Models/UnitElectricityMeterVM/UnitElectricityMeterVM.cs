using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.UnitElectricityMeterVM
{
    public class UnitElectricityMeterVM
    {
        [Key]
        public int IdUnit
        {
            get;
            set;
        }

        [Display(Name = "Unit Number")]
        public string UnitNumber
        {
            get;
            set;
        }

        public int IdBuilding
        {
            get;
            set;
        }

        [Display(Name = "Electricity Meter Number")]
        public string ElectricityMeterNumber
        {
            get;
            set;
        }

        [Display(Name = "Building")]
        public string BuildingInfo
        {
            get;
            set;
        }

        public string districtName
        {
            get;
            set;
        }
    }
}
