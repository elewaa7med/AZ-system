using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models.CompoundsViewModels
{
	public class CompoundsVM
	{
		[Key]
		public int IdCompound
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "District")]
		public int IdDistrict
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Compound Name")]
		public string compoundName
		{
			get;
			set;
		}

		public string serviceWaterMeterNo
		{
			get;
			set;
		}

		public bool oneWaterMeterAllBld
		{
			get;
			set;
		}

		public string waterMeterAllBldNo
		{
			get;
			set;
		}

		public string Notes
		{
			get;
			set;
		}

		[Display(Name = "Created By")]
		public string IdCreated
		{
			get;
			set;
		}

		[Display(Name = "Modified By")]
		public string IdModified
		{
			get;
			set;
		}

		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH-mm}")]
		[Display(Name = "Date Created")]
		public DateTime dtCreated
		{
			get;
			set;
		}

		[Display(Name = "Date Modified")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH-mm}")]
		public DateTime dtModified
		{
			get;
			set;
		}

		[ForeignKey("IdDistrict")]
		public virtual Districts mDistrict
		{
			get;
			set;
		}

		[ForeignKey("IdCreated")]
		public virtual ApplicationUser mUserCreated
		{
			get;
			set;
		}

		[ForeignKey("IdModified")]
		public virtual ApplicationUser mUserModified
		{
			get;
			set;
		}

		public virtual ICollection<CompoundsUsers> mCompoundUsers
		{
			get;
			set;
		}

		public virtual ICollection<ApplicationUser> mInCompoundUsers
		{
			get;
			set;
		}

		public virtual ICollection<ApplicationUser> mOutCompoundUsers
		{
			get;
			set;
		}
		[NotMapped]
		public List<string> Users { get; set; } = new List<string>();

	}
}
