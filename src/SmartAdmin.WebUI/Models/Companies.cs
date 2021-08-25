using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class Companies
	{
		[Key]
		public int IdCompany
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Company Name")]
		public string companyName
		{
			get;
			set;
		}

		[Display(Name = "Company Address")]
		public string companyAddress
		{
			get;
			set;
		}

		[Display(Name = "Company Phone")]
		public string compayPhone
		{
			get;
			set;
		}

		public virtual ICollection<Units> mUnits
		{
			get;
			set;
		}
	}
}
