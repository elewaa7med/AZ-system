using SmartAdmin.WebUI.Controllers;

namespace SmartAdmin.WebUI.Models.DTO
{
    public class CompoundDTO
    {
        public string CompoundName { get; set; }
        public int CompoundID { get; set; }
        public DueViewModel DueValues { get; set; }
        public string RepresentitveId { get; set; }
    }
}
