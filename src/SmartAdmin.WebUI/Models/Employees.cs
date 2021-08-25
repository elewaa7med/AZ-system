using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class Employees
	{
		[Key]
		public int IdEmployee
		{
			get;
			set;
		}

		[Required]
		[StringLength(250)]
		[Display(Name = "Full Name")]
		public string fullName
		{
			get;
			set;
		}

		[StringLength(50)]
		[EmailAddress]
		[Display(Name = "EMail")]
		public string eMail
		{
			get;
			set;
		}

		[StringLength(50)]
		[Phone]
		[Display(Name = "Mobile")]
		public string mobile
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Gender")]
		public int gender
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Nationality")]
		public int IdNationality
		{
			get;
			set;
		}

		[ForeignKey("IdNationality")]
		[Display(Name = "Nationality")]
		public virtual Nationalities Tnationality
		{
			get;
			set;
		}

		[DataType(DataType.Date)]
		[Display(Name = "Birth Date")]
		public DateTime? dtDateOfBirth
		{
			get;
			set;
		}

		[Display(Name = "Iqama Image")]
		public string iqamaPic
		{
			get;
			set;
		}

		[Required]
		[StringLength(50)]
		[Display(Name = "Iqama Number")]
		public string iqamaNumber
		{
			get;
			set;
		}

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Iqama Expiry date")]
		public DateTime dtIqamaExpiryDate
		{
			get;
			set;
		}

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Passport Expiry date")]
		public DateTime dtpassPortExpiry
		{
			get;
			set;
		}

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Contract Expiry Date")]
		public DateTime dtcontractExpiryDate
		{
			get;
			set;
		}

		[DataType(DataType.Date)]
		[Display(Name = "Joining Date")]
		public DateTime dtJoiningDate
		{
			get;
			set;
		}

		[StringLength(250)]
		[Display(Name = "Job Title")]
		public string jobTitle
		{
			get;
			set;
		}

		[StringLength(250)]
		[Display(Name = "Employee Picture")]
		public string empPicture
		{
			get;
			set;
		}

		[StringLength(250)]
		[Display(Name = "Passport Picture")]
		public string passPortPic
		{
			get;
			set;
		}

		[StringLength(512)]
		[Display(Name = "Notes")]
		public string Notes
		{
			get;
			set;
		}

		public int isDeleted
		{
			get;
			set;
		}

		[StringLength(50)]
		[Display(Name = "Passport Number")]
		public string passportNumber
		{
			get;
			set;
		}

		[Display(Name = "Work Place")]
		public int IdWorkPlace
		{
			get;
			set;
		}
		
		[Display(Name = "Company Kafala")]
		public bool IsCompanykafala
		{
			get;
			set;
		}

		[ForeignKey("IdWorkPlace")]
		public virtual WorkPlaces mWorkPlace
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

		[DataType(DataType.DateTime)]
		[Display(Name = "Date Created")]
		public DateTime dtCreated
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[Display(Name = "Modified By")]
		public string IdModified
		{
			get;
			set;
		}

		[DataType(DataType.DateTime)]
		[Display(Name = "Date Modified")]
		public DateTime dtModified
		{
			get;
			set;
		}

		[ForeignKey("IdCreated")]
		public virtual ApplicationUser mCreatedBy
		{
			get;
			set;
		}

		[ForeignKey("IdModified")]
		public virtual ApplicationUser mModifiedBy
		{
			get;
			set;
		}

		[StringLength(250)]
		[Display(Name = "Flat Contract File")] 
		public string FlatContractFile { get;  set; }

		[StringLength(250)]
		[Display(Name = "Work Contract File")] 
		public string WorkContractFile { get;  set; }
    }
}
