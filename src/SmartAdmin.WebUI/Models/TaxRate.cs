using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class TaxRate
    {
        [Key]
        public int Id { get; set; }
        public int Rate { get; set; }
        public DateTime StartApplingDate { get; set; }
        public DateTime? EndApplingDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TaxToProperttypes> TaxToProperttypes { get; set; }
    }
}
