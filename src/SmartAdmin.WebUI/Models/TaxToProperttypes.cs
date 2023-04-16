using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Models
{
    public class TaxToProperttypes
    {
        [Key]
        public int Id { get; set; }
        public int TaxRateId { get; set; }
        public virtual TaxRate TaxRate { get; set; }
        public int PropertyTypesId { get; set; }
        public virtual PropertyTypes PropertyTypes { get; set; }
    }
}
