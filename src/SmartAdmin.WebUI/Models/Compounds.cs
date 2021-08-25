using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class Compounds
	{
		[Key]
		public int IdCompound
		{
			get;
			set;
		}

		[Required]
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

		[Display(Name = "Service Water Meter")]
		public string serviceWaterMeterNo
		{
			get;
			set;
		}

		[Display(Name = "One Water Meter for all Buildings In the compound")]
		public bool oneWaterMeterAllBld
		{
			get;
			set;
		}

		[Display(Name = "Buildings Water meter No")]
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

		public string IdCreated
		{
			get;
			set;
		}

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

		[Display(Name = "District")]
		[ForeignKey("IdDistrict")]
		public virtual Districts mDistrict
		{
			get;
			set;
		}

		[Display(Name = "Created By")]
		[ForeignKey("IdCreated")]
		public virtual ApplicationUser mUserCreated
		{
			get;
			set;
		}

		[Display(Name = "Modified By")]
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
	}
}
