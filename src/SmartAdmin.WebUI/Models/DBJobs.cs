using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class DBJobs
	{
		[Key]
		public int IdJob
		{
			get;
			set;
		}

		public string JobName
		{
			get;
			set;
		}

		public decimal JobFrequency
		{
			get;
			set;
		}

		public DateTime dtLastPerformed
		{
			get;
			set;
		}
	}
}
