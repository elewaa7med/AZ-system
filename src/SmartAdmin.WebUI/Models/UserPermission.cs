using SmartAdmin.WebUI.Authorization;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAdmin.WebUI.Models
{
    public class UserPermission
    {
        public int ID { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Permission Permission { get; set; }
    }
}