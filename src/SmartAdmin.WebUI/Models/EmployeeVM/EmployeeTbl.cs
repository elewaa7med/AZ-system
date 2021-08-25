using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.EmployeeVM
{
	public class EmployeeTbl
	{
		public int IdEmployee
		{
			get;
			set;
		}

		[Display(Name = "Expiry Date")]
		public string dtIqamaExpiryDate
		{
			get;
			set;
		}

		[Display(Name = "Full Name")]
		public string fullName
		{
			get;
			set;
		}

		[Display(Name = "Iqama Number")]
		public string iqamaNumber
		{
			get;
			set;
		}

		[Display(Name = "Mobile")]
		public string mobile
		{
			get;
			set;
		}

		[Display(Name = "Nationality")]
		public string CountryName
		{
			get;
			set;
		}

		[Display(Name = "Days")]
		public int remaining_days
		{
			get;
			set;
		}

		[Display(Name = "Job Title")]
		public string jobTitle
		{
			get;
			set;
		}

		[Display(Name = "Passport Number")]
		public string passportNumber
		{
			get;
			set;
		}

		[Display(Name = "Work Place")]
		public string workplace
		{
			get;
			set;
		}

		[Display(Name = "Company Kafala")]
		public bool IsCompanykafala
		{
			get;
			set;
		}
	}
}
