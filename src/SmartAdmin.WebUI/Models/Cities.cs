using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class Cities
	{
		[Key]
		public int IdCity
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "City Name")]
		public string CityName
		{
			get;
			set;
		}

		[Display(Name = "Country")]
		public int IdCountry
		{
			get;
			set;
		}

		[ForeignKey("IdCountry")]
		[Display(Name = "Country")]
		public virtual Countries mBuildingCountry
		{
			get;
			set;
		}

		public virtual ICollection<Districts> mDistricts
		{
			get;
			set;
		}
	}
}
