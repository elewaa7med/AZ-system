using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.AccountViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email
		{
			get;
			set;
		}
	}
}
