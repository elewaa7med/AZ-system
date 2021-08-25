using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class TPositions
	{
		[Key]
		public int IdPosition
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Position Name-Arabic")]
		public string positionName_a
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Position Name-English")]
		public string positionName_E
		{
			get;
			set;
		}

		public virtual ICollection<ApplicationUser> appUsers
		{
			get;
			set;
		}
	}
}
