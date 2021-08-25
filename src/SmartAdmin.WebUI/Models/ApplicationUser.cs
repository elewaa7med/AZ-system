using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string fullName
        {
            get;
            set;
        }

        public string mobileNo
        {
            get;
            set;
        }

        public string picName
        {
            get;
            set;
        }

        public int? userDefaultBuildingCountry
        {
            get;
            set;
        }

        public int? userDefaultBuildingCity
        {
            get;
            set;
        }

        public int? userdefaultBuildingDistrict
        {
            get;
            set;
        }

        public int IdPosition
        {
            get;
            set;
        }

        public virtual DateTime? LastLoginDateTime
        {
            get;
            set;
        }

        public virtual DateTime? CurrentLoginDateTime
        {
            get;
            set;
        }

        public virtual DateTime? RegistrationDate
        {
            get;
            set;
        }

        [ForeignKey("IdPosition")]
        public virtual TPositions TPosition
        {
            get;
            set;
        }

        [ForeignKey("userDefaultBuildingCountry")]
        public virtual Countries TCountry
        {
            get;
            set;
        }

        [ForeignKey("userDefaultBuildingCity")]
        public virtual Cities TCity
        {
            get;
            set;
        }

        [ForeignKey("userdefaultBuildingDistrict")]
        public virtual Districts Districts
        {
            get;
            set;
        }

        public virtual ICollection<CompoundsUsers> mCompoundUsers
        {
            get;
            set;
        }
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();
        public virtual ICollection<UnitRentContractAllPaymentLogs> UnitRentContractAllPaymentLogs { get; set; }
    }
}
