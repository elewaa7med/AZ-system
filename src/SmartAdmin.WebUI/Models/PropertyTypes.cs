using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class PropertyTypes
	{
		[Key]
		public int IdPropertyType
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Property Type")]
		public string PropertyTypeName
		{
			get;
			set;
		}

		public virtual ICollection<Units> mUnits
		{
			get;
			set;
		}
		public virtual ICollection<TaxToProperttypes> TaxToProperttypes { get; set; }

	}
}
