using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAdmin.WebUI.Models
{
    public class Mandoobs
    {
        [Key]
        public int IdMandoob
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Sales Representative Name")]
        public string mandoobName
        {
            get;
            set;
        }

        [Required]
        [Display(Name = "Sales Representative Mobile")]
        public string mandoobMobile
        {
            get;
            set;
        }

        [Display(Name = "Remarks")]
        public string mandoobNotes
        {
            get;
            set;
        }

        public virtual ICollection<UnitKeys> mUnitKeys
        {
            get;
            set;
        }
    }
}
