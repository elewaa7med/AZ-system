using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class MeterWaterInfo
	{
		[Key]
		public int IdMeter
		{
			get;
			set;
		}

		[Display(Name = "Water Meter Number")]
		public string MeterNumber
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Building")]
		public int IdBuilding
		{
			get;
			set;
		}

		[ForeignKey("IdBuilding")]
		[Display(Name = "Building")]
		public virtual Buildings mBuilding
		{
			get;
			set;
		}

		[Display(Name = "Notes")]
		public string MeterNotes
		{
			get;
			set;
		}
	}
}
