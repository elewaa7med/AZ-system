using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
	public class BuildingFloors
	{
		[Key]
		public int IdBuildingFloor
		{
			get;
			set;
		}

		[Required]
		public string PropertyFloorName
		{
			get;
			set;
		}

		public virtual ICollection<Units> mUnits
		{
			get;
			set;
		}
	}
}
