using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class ApplicationRole : IdentityRole
	{
		[Display(Name = "Description")]
		public string Description
		{
			get;
			set;
		}

		[Display(Name = "Date Created")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy - HH-mm}")]
		public DateTime dtCreated
		{
			get;
			set;
		}

		public ApplicationRole()
		{
		}

		public ApplicationRole(string roleName)
			: base(roleName)
		{
		}

		public ApplicationRole(string roleName, string description, DateTime dtCreated)
			: base(roleName)
		{
			Description = description;
			this.dtCreated = dtCreated;
		}
	}
}
