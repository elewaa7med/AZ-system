using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class MetersElectricityInfo
	{
		[Key]
		public int IdMeter
		{
			get;
			set;
		}

		[Display(Name = "Meter Number")]
		public string MeterNumber
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Unit Number")]
		public int IdUnit
		{
			get;
			set;
		}

		[ForeignKey("IdUnit")]
		public virtual Units mUnits
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
