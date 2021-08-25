using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models.ViewModels
{
    public class LegalListViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Tenent Name")]
        public string TenentName { get; set; }

        [Display(Name = "Tenent Nationality")]
        public string TenentNationality { get; set; }

        [Display(Name = "Bulding Number")]
        public string BuldingNumber { get; set; }

        [Display(Name = "Unit Number")]
        public string UnitNumber { get; set; }

        [Display(Name = "Contract Date")]
        public string ContractDate { get; set; }

        [Display(Name = "Due Date")]
        public string DueDate { get; set; }

        [Display(Name = "Delayed Rent")]
        public int DelayedRent { get; set; }

        [Display(Name = "Electricity Bill")]
        public int ElectricityBill { get; set; }

        [Display(Name = "Water Bill")]
        public int WaterBill { get; set; }

        [Display(Name = "Total Due Pay")]
        public int TotalDuePay => DelayedRent + ElectricityBill + WaterBill;

        [Display(Name = "Mandoob")]
        public string MandoobName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Request Submit Date")]
        public string RequestSubmitDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Request Raise Date")]
        public string RequestRaiseDate { get; set; }

        public string Note { get; set; }

        [Display(Name = "Is Bulding")]
        public bool IsBulding { get; internal set; }
        public int RentContractId { get; internal set; }
        public int NoteId { get; internal set; }
        public int? pageId { get; set; }
        public string DisplayType { get; set; }
        public int? compoundId { get; set; }
    }
}
