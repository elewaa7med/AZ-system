using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.WebUI
{
	public static class UrlHelperExtensions
	{
		public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action("ConfirmEmail", "Account", new
			{
				userId,
				code
			}, scheme);
		}

		public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
		{
			return urlHelper.Action("ResetPassword", "Account", new
			{
				userId,
				code
			}, scheme);
		}
	}
}
