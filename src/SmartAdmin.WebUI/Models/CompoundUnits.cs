using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAdmin.WebUI.Enums;

namespace SmartAdmin.WebUI.Models
{
    public class CompoundUnits
    {
        [Key]
        public int IdUnit
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Building of The Unit")]
        public int IdBuilding
        {
            get;
            set;
        }

        [ForeignKey("IdBuilding")]
        [Display(Name = "Building Number")]
        public virtual CompoundBuildings mCompoundBuilding
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Unit Number")]
        public string UnitNumber
        {
            get;
            set;
        }

        [Display(Name = "Unit Condition (Good/Requires Work)")]
        public int? unitCondition
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Number of Rooms")]
        public int NUmberofRooms
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Number of Baths")]
        public int NoOfBaths
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Number of Living")]
        public int NuberofLivings
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Number of Majlis")]
        public int NumberofMajlis
        {
            get;
            set;
        }

        [Display(Name = "Unit Area")]
        public int? UnitArea
        {
            get;
            set;
        }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Rent Rate")]
        public int RentRate
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "For Families?")]
        public bool Forfamilies
        {
            get;
            set;
        }

        public bool isDeleted
        {
            get;
            set;
        }

        [Display(Name = "Electricity Meter Number")]
        public string ElectricityNo
        {
            get;
            set;
        }

        [Display(Name = "Electricity Sadat Number")]
        public string ElectricitySadadNo
        {
            get;
            set;
        }

        public string Notes
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Unit Type")]
        public int IdPropertyType
        {
            get;
            set;
        }

        [ForeignKey("IdPropertyType")]
        [Display(Name = "Property Type")]
        public virtual PropertyTypes mPropertyType
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Floor")]
        public int IdFloor
        {
            get;
            set;
        }

        [ForeignKey("IdFloor")]
        [Display(Name = "Floor")]
        public virtual BuildingFloors mFloor
        {
            get;
            set;
        }

        public string IdCreatedBy
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

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Create")]
        [DisplayFormat(DataFormatString = "{0 : dd\\\\MM\\\\yyyy - HH:mm}")]
        public DateTime dtCreated
        {
            get;
            set;
        }

        public string IdModifiedBy
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

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Modified")]
        [DisplayFormat(DataFormatString = "{0 : dd\\\\MM\\\\yyyy - HH:mm}")]
        public DateTime dtModified
        {
            get;
            set;
        }

        public virtual ICollection<CompoundUnitKeys> mCompoundUnitKeys
        {
            get;
            set;
        }

        [Display(Name = "Sales Representative")]
        public int? IdMandoob
        {
            get;
            set;
        }

        [ForeignKey("IdMandoob")]
        public virtual Mandoobs mMandoob
        {
            get;
            set;
        }

        [DataType(DataType.Date)]
        [Display(Name = "Date the key us taken")]
        [DisplayFormat(DataFormatString = "{0 : dd\\\\MM\\\\yyyy}")]
        public DateTime? dtTaken
        {
            get;
            set;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0 : dd\\\\MM\\\\yyyy}")]
        [Display(Name = "Data the key is Back")]
        public DateTime? dtBack
        {
            get;
            set;
        }

        [Display(Name = "The Key Is Available")]
        public bool? isTheKeyAvailable
        {
            get;
            set;
        }

        public int? IdUnitKey
        {
            get;
            set;
        }

        public bool? isRented
        {
            get;
            set;
        }
        public int? IdRentContract
        {
            get;
            set;
        }
        [ForeignKey("UnitRentContract")]
        public int? UnitRentContractID
        {
            get;
            set;
        }
        public virtual UnitRentContract UnitRentContract { get; set; }
        public UnitOwner UnitOwner { get; set; }
        [NotMapped]
        public string ElectricityMeterNumber { get; set; }
        [NotMapped]
        public string ElectricityPaymentNumber { get; set; }
        [NotMapped]
        public bool TransferToTenanat { get; set; }
        public string Description { get; set; }
        public string DescriptionAR { get; set; }

    }
}
