using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace SmartAdmin.WebUI.Models
{
    public class CompoundContracts
    {
        [Key]
        public int Id
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

        [Display(Name = "Lease End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime dtLeaseEnd
        {
            get;
            set;
        }

        [Required]
        public string CompoundName { get; set; }

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

        [NotMapped]
        public string CurrentContractFile
        {
            get
            {
                return string.IsNullOrEmpty(contractImage) ? string.Empty :  "assets\\Compoundcontracts\\" + contractImage;
            }
            private set { }
        }
        [Display(Name = "Contract Image")]
        public string contractImage
        {
            get;
            set;
        }
        public string Notes { get; set; }
    }
}