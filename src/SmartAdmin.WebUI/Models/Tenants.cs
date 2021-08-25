using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
    public class Tenants
    {
        [Key]
        public int IdTenant
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Tenant Name")]
        public string tenantName
        {
            get;
            set;
        }
        [Display(Name = "Tenant Name")]
        public string TenantNameAR
        {
            get;
            set;
        }

        [EmailAddress]
        [Display(Name = "Email")]
        public string tenantEmail
        {
            get;
            set;
        }

        [Required]
        [Phone]
        [Display(Name = "Mobile")]
        public string tenantMobile
        {
            get;
            set;
        }

        [Display(Name = "Emergency Phone")]
        [Phone]
        public string emergencyPhone
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Family Count")]
        public int tenantFamilycount
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
        public virtual Nationalities mNationality
        {
            get;
            set;
        }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Iqama Picture")]
        public string IqamaPicture
        {
            get;
            set;
        }

        [Display(Name = "Iqama Number")]
        public string IqamaNo
        {
            get;
            set;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy}")]
        [Display(Name = "Iqama Expiry Date")]
        public DateTime dtIqamaExpiration
        {
            get;
            set;
        }

        [Display(Name = "Iqama Expiry Hijri date")]
        public string IqamaExpirationHijri
        {
            get;
            set;
        }

        [Display(Name = "Work Notes")]
        public string workNotes
        {
            get;
            set;
        }

        [Display(Name = "Company")]
        public int IdCompany
        {
            get;
            set;
        }

        [ForeignKey("IdCompany")]
        [Display(Name = "Company")]
        public virtual Companies mCompany
        {
            get;
            set;
        }

        [Display(Name = "Tenant Notes")]
        public string tenantsNotes
        {
            get;
            set;
        }

        public bool isMarried
        {
            get;
            set;
        }

        [Display(Name = "Id-HR Letter Notes")]
        public string IdLetterPicture
        {
            get;
            set;
        }

        public string TenantCompany
        {
            get;
            set;
        }

        public string IdCreatedBy
        {
            get;
            set;
        }


        public string IdModifiedBy
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
        [DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy - HH:mm}")]
        public DateTime dtCreated
        {
            get;
            set;
        }

        [Display(Name = "Modified By")]
        [ForeignKey("IdModifiedBy")]
        public virtual ApplicationUser mUserModified
        {
            get;
            set;
        }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd\\\\MM\\\\yyyy - HH:mm}")]
        public DateTime dtModified
        {
            get;
            set;
        }

        [NotMapped]
        public bool HasContract { get; set; }
    }
}
