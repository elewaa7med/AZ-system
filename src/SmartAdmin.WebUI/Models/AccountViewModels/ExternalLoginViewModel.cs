using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models.AccountViewModels
{
	public class ExternalLoginViewModel
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
