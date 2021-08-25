using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.BldMetersViewModel
{
	public class BldWaterMeterVM
	{
		[Key]
		public int IdBuilding
		{
			get;
			set;
		}

		[Display(Name = "Water Meter Number")]
		public string MeterWaterNumber
		{
			get;
			set;
		}

		[Required]
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
