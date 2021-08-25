using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class Nationalities
	{
		[Key]
		public int IdCountry
		{
			get;
			set;
		}

		[Required]
		public string CountryNameCapitalLetters
		{
			get;
			set;
		}

		[Required]
		public string CountryName
		{
			get;
			set;
		}

		[Required]
		public string ISO
		{
			get;
			set;
		}

		[Required]
		public string ISO3
		{
			get;
			set;
		}

		[Required]
		public int NumCode
		{
			get;
			set;
		}

		[Required]
		public int PhoneCode
		{
			get;
			set;
		}

		public virtual ICollection<Tenants> mTenants
		{
			get;
			set;
		}
	}
}
