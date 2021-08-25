using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class Owners
	{
		[Key]
		public int IdOwner
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Owner Name")]
		public string OwnerName
		{
			get;
			set;
		}

		[Required]
		[Phone]
		[Display(Name = "Owner Mobile")]
		public string OwnerMobile
		{
			get;
			set;
		}

		[Display(Name = "Owner Phone")]
		[Phone]
		public string OwnerPhone
		{
			get;
			set;
		}

		[Display(Name = "owner Email")]
		[EmailAddress]
		public string Owneremail
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Address")]
		public string OwnerAddress
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Company Name")]
		public string CompanyName
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Contact Name")]
		public string ContactName
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Contact Phone")]
		public string ContactPhone
		{
			get;
			set;
		}

		[Display(Name = "Notes")]
		public string OwnerNnotes
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy - HH:mm}")]
		public DateTime dtCreated
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy - HH:mm}")]
		public DateTime dtModified
		{
			get;
			set;
		}

		[Display(Name = "Created By")]
		public string IdCreatedBy
		{
			get;
			set;
		}

		[Display(Name = "Modified By")]
		public string IdModifiedBy
		{
			get;
			set;
		}

		[ForeignKey("IdCreatedBy")]
		[Display(Name = "Created By")]
		public virtual ApplicationUser mUserCreated
		{
			get;
			set;
		}

		[ForeignKey("IdModifiedBy")]
		[Display(Name = "Modified By")]
		public virtual ApplicationUser mUserModified
		{
			get;
			set;
		}
	}
}
