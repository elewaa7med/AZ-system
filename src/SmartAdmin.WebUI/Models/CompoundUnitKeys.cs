using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class CompoundUnitKeys
	{
		[Key]
		public int IdUnitKeys
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Unit Number")]
		public int IdUnit
		{
			get;
			set;
		}

		[ForeignKey("IdUnit")]
		[Display(Name = "Unit Number")]
		public virtual CompoundUnits mCompoundUnit
		{
			get;
			set;
		}

		[Required]
		[Display(Name = "Sales Representative")]
		public int IdMandoob
		{
			get;
			set;
		}

		[ForeignKey("IdMandoob")]
		[Display(Name = "Sales Representative")]
		public virtual Mandoobs mMandoob
		{
			get;
			set;
		}

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy}")]
		[Display(Name = "Date the key is taken")]
		public DateTime? dtTaken
		{
			get;
			set;
		}

		[DataType(DataType.Date)]
		[Display(Name = "Date the key is returned")]
		[DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy}")]
		public DateTime? dtBack
		{
			get;
			set;
		}

		[Display(Name = "Key Is Available.")]
		public bool isTheKeyAvailable
		{
			get;
			set;
		}
	}
}
