using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace SmartAdmin.WebUI.Models
{
    public class UnitRentContract
    {
        [Key]
        public int IdRentContract
        {
            get;
            set;
        }
        [Required]
        [Display(Name = "Contract No.")]
        public string contractNumber
        {
            get;
            set;
        }
        [Display(Name = "Unit")]
        public int? IdUnit
        {
            get;
            set;
        }

        [ForeignKey("IdUnit")]
        public virtual Units mUnit
        {
            get;
            set;
        }

        [Display(Name = "Unit")]
        public int? IdUnitCompound
        {
            get;
            set;
        }

        [ForeignKey("IdUnitCompound")]
        public virtual CompoundUnits mCompoundUnits
        {
            get;
            set;
        }

        [Display(Name = "Tenant")]
        public int IdTenant
        {
            get;
            set;
        }

        [ForeignKey("IdTenant")]
        public virtual Tenants mTenant
        {
            get;
            set;
        }

        [Display(Name = "Lease Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime dtLeaseStart
        {
            get;
            set;
        }

        [Display(Name = "Lease Period In Monthes")]
        public int? leasePeriodInMonthes
        {
            get;
            set;
        }

        [Display(Name = "Lease End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime dtLeaseEnd
        {
            get;
            set;
        }

        [Display(Name = "Yearly Rent")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int yearlyRent
        {
            get;
            set;
        }

        [Display(Name = "Yearly Rent")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [NotMapped]
        public int YearlyRentVm
        {
            get;
            set;
        }

        [Display(Name = "Paid Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int paidAmount
        {
            get;
            set;
        }

        [Display(Name = "Remaining Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int remainingAmount
        {
            get;
            set;
        }

        [Display(Name = "Last Contract Remaining Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int LastContractRemainingAmount
        {
            get;
            set;
        }

        [Display(Name = "Water Yearly Bill")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? WaterBillAmount
        {
            get;
            set;
        }

        [Display(Name = "Water Paid Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? WaterPaidAmount
        {
            get;
            set;
        }

        [Display(Name = "Water Remaining Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int? WaterRemainingAmount
        {
            get;
            set;
        }

        [Display(Name = "Insurance")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int insurance
        {
            get;
            set;
        }

        [Display(Name = "Payment Method")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Required]
        public int paymentMethod
        {
            get;
            set;
        }

        [Display(Name = "Unit Accounting Number")]
        public string unitAccountNumber
        {
            get;
            set;
        }

        [Display(Name = "Contract Image")]
        public string contractImage
        {
            get;
            set;
        }

        [Display(Name = "Water Bill Included")]
        public bool waterBillIncluded
        {
            get;
            set;
        }

        [Display(Name = "Electricity Bill Included")]
        public bool electricityBillIncluded
        {
            get;
            set;
        }

        [Display(Name = "Furnished Unit")]
        public bool furnishedUnit
        {
            get;
            set;
        }

        [Display(Name = "Attested Rent Contract")]
        public bool attestedRentContract
        {
            get;
            set;
        }

        [Display(Name = "Initial Elec. Meter Reading")]
        public int? initialElectricityMeterReading
        {
            get;
            set;
        }

        [Display(Name = "Initial Water Meter Reading")]
        public int? initialWaterMeterReading
        {
            get;
            set;
        }

        [Display(Name = "Departure Elec. Meter Reading")]
        public int? finalElectricityMeterReading
        {
            get;
            set;
        }

        [Display(Name = "Departure Water Meter Reading")]
        public int? finalWaterMeterReading
        {
            get;
            set;
        }

        [Display(Name = "Rent Commission")]
        public decimal rentCommission
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
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy - HH:mm}")]
        [Display(Name = "Date Created")]
        public DateTime dtCreated
        {
            get;
            set;
        }

        [Display(Name = "Modified By")]
        public string IdModified
        {
            get;
            set;
        }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy - HH:mm}")]
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

        [ForeignKey("IdMBuilding")]
        public int mMasterBuilding
        {
            get;
            set;
        }

        public int? IdCompound
        {
            get;
            set;
        }

        // by H1 

        public bool Archived
        {
            get;
            set;
        }

        // 


        [ForeignKey("IdCompound")]
        public virtual Compounds mCompound
        {
            get;
            set;
        }

        [NotMapped]
        public string lstchkBoxes
        {
            get;
            set;
        }

        [NotMapped]
        public List<int> UnitIncluded
        {
            get;
            set;
        }

        [NotMapped]
        public int IdBuilding
        {
            get;
            set;
        }
        [NotMapped]
        public string CurrentImageFile
        {
            get
            {
                return string.IsNullOrEmpty(contractImage) ? string.Empty : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\contracts\\", contractImage);
            }
            private set { }
        }

        public virtual ICollection<UnitRentContractPayment> UnitRentContractPayments { get; set; } = new HashSet<UnitRentContractPayment>();
        public virtual ICollection<UnitRentContractNote> UnitRentContractNotes { get; set; } = new HashSet<UnitRentContractNote>();
        [Display(Name = "Mandoob")]
        public string IdMandoob { get; set; }
        [ForeignKey("IdMandoob")]
        public virtual ApplicationUser Mandoob
        {
            get;
            set;
        }
        //by auf
        [Display(Name = "Added To Court")]
        public bool AddedtoCourt { get; set; }

        public int? LegalId { get; set; }

        public virtual Legal Legal { get; set; }
        public bool VerifiedFromGovernment { get; set; }
        public string NotVerifiedReason { get; set; }
        public string ElectricityNumber { get; set; }
        public string PaymentNumber { get; set; }
        [NotMapped]
        public string MeterStart { get; set; }
        [NotMapped]
        public bool TransferToTenanat { get; set; }
        public string Notes { get; set; }
        public string NotesAR { get; set; }
        public bool Renewed { get; set; }
        public bool Blocked { get; set; }
        public string BlockReason { get; set; }

        public ICollection<Invoices> invoices { get; set; }
        public virtual ICollection<UnitRentContractAllPaymentLogs> UnitRentContractAllPaymentLogs { get; set; }
        public virtual ICollection<UnitRentContractOtherPayment> UnitRentContractOtherPayment { get; set; }
    }
}