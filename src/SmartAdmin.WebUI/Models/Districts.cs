using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class Districts
	{
		[Key]
		public int IdDistrict
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "District Name")]
		public string DistrictName
		{
			get;
			set;
		}

		[Display(Name = "City")]
		public int IdCity
		{
			get;
			set;
		}

		[ForeignKey("IdCity")]
		public virtual Cities mCity
		{
			get;
			set;
		}

		public virtual ICollection<Buildings> mBuildings
		{
			get;
			set;
		}
	}
}
