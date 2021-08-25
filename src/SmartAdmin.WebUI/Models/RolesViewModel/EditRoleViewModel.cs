using System.Collections.Generic;

namespace SmartAdmin.WebUI.Models.RolesViewModel
{
	public class EditRoleViewModel
	{
		public ApplicationRole mRole
		{
			get;
			set;
		}

		public IEnumerable<ApplicationUser> mInRoleUsers
		{
			get;
			set;
		}

		public IEnumerable<ApplicationUser> mOutRoleUsers
		{
			get;
			set;
		}
		public IEnumerable<string> Users { get; set; } = new List<string>();
	}
}
