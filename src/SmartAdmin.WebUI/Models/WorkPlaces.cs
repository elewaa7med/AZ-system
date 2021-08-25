using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class WorkPlaces
	{
		[Key]
		public int IdWorkPlace
		{
			get;
			set;
		}

		[Required]
		[StringLength(250)]
		[Display(Name = "Work Place")]
		public string workPlace
		{
			get;
			set;
		}

		[StringLength(250)]
		[Display(Name = "Notes")]
		public string notes
		{
			get;
			set;
		}
	}
}
