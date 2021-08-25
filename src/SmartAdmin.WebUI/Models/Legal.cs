using Microsoft.AspNetCore.ResponseCompression;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{

    //contracts which moved to court
    public class Legal: BaseEntity
    {
        
        public int IdRentContract { get; set; }
        
        [Required]
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Request Submit Date")]
        public DateTime RequestSubmitDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Request Raise Date")]
        public DateTime? RequestRaiseDate { get; set; }

        [Display(Name = "Electricity Bill")]
        public int  ElectricityBill { get; set; }

        [Display(Name = "Walter Bill")]
        public int WalterBill { get; set; }

        //relations

        [ForeignKey("IdRentContract")]
        public virtual UnitRentContract UnitRentContract { get; set; }
    }
}
