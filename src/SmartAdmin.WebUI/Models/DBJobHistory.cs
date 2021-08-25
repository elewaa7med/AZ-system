using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class DBJobHistory
	{
		[Key]
		public int IdDBJobHistory
		{
			get;
			set;
		}

		public bool statusFlag
		{
			get;
			set;
		}

		public DateTime dtLastExecuted
		{
			get;
			set;
		}
	}
}
