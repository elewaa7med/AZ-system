using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class CompoundBuildings
	{
		[Key]
		public int IdBuilding
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Compound")]
		public int IdCompound
		{
			get;
			set;
		}

		[ForeignKey("IdCompound")]
		[Display(Name = "Compound")]
		public virtual Compounds mCompound
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Building Number")]
		public string BuildingNumber
		{
			get;
			set;
		}

		[Display(Name = "Address")]
		public string BuildingAddress
		{
			get;
			set;
		}

		[DisplayFormat(DataFormatString = "{0:N0}")]
		[Display(Name = "Building Value")]
		public int BuildingValue
		{
			get;
			set;
		}

		[DisplayFormat(DataFormatString = "{0:N0}")]
		[Display(Name = "Yearly Income")]
		public int BuildingYearlyIncome
		{
			get;
			set;
		}

		[DisplayFormat(DataFormatString = "{0:N0}")]
		[Display(Name = "Building Area")]
		public int? BuildingArea
		{
			get;
			set;
		}

		[Display(Name = "Building GPS Link")]
		public string GPSLink
		{
			get;
			set;
		}

		[Display(Name = "Building GPS Lat.")]
		public float? GPSLatitude
		{
			get;
			set;
		}

		[Display(Name = "Building GPS Long.")]
		public float? GPSLongitude
		{
			get;
			set;
		}

		[DataType(DataType.ImageUrl)]
		[Display(Name = "Building Image")]
		public string BuildingPicture
		{
			get;
			set;
		}

		[Display(Name = "Water Meter Number")]
		public string MeterWaterNumber
		{
			get;
			set;
		}

		[Display(Name = "Year of Build")]
		public int BuildingYear
		{
			get;
			set;
		}

		[Display(Name = "Remarks")]
		public string BuildingNotes
		{
			get;
			set;
		}

		[Display(Name = "Building For Families?")]
		public bool ForFamilies
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Building Owner")]
		public int IDOwner
		{
			get;
			set;
		}

		[ForeignKey("IDOwner")]
		[Display(Name = "Owner")]
		public virtual Owners mOwner
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[Display(Name = "Date Created")]
		[DisplayFormat(DataFormatString = "{0:dd\\\\MM\\\\yyyy - HH:mm}")]
		public DateTime dtCreated
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[Display(Name = "Date Modified")]
		[DisplayFormat(DataFormatString = "{0:dd\\\\MM\\\\yyyy - HH:mm}")]
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

		public virtual ICollection<CompoundUnits> mCompoundUnits
		{
			get;
			set;
		}
	}
}
