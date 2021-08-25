using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
	public class CompoundsUsers
	{
		[Key]
		public int IdCompoundUser
		{
			get;
			set;
		}

		public string IdUser
		{
			get;
			set;
		}

		[ForeignKey("IdUser")]
		public virtual ApplicationUser mUser
		{
			get;
			set;
		}

		public int IdCompound
		{
			get;
			set;
		}

		[ForeignKey("IdCompound")]
		public virtual Compounds mCompound
		{
			get;
			set;
		}
	}
}
