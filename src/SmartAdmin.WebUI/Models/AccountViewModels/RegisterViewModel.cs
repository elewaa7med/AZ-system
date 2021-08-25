using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.AccountViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Full Name")]
		public string fullName
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Mobile Number")]
		public string mobileNo
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Login Name")]
		public string UserName
		{
			get;
			set;
		}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email
		{
			get;
			set;
		}

		[Display(Name = "Picture")]
		public string picName
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Position")]
		public int Idposition
		{
			get;
			set;
		}

		public virtual TPositions mposition
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword
		{
			get;
			set;
		}

		public virtual ICollection<TPositions> Jobpositions
		{
			get;
			set;
		}
	}
}
