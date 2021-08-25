using System;

namespace SmartAdmin.WebUI.Models.DTO
{
    public class RentContractNoteDTO
    {
        public int ID { get; set; }
        public string Note { get; set; }
        public string User { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ContractID { get; set; }
        public int pageid { get; set; }
    }
}
