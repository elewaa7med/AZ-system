using Microsoft.AspNetCore.Mvc.Rendering;
using SmartAdmin.WebUI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SmartAdmin.WebUI
{
    public class Enums
    {
        public enum UnitOwner
        {
            [Display(Name = "AZ employee", Order = 4)]
            AZ = 1,
            [Display(Name = "X Saad employee", Order = 2)]
            xSaad = 2,
            [Display(Name = "AZ", Order = 1)]
            SaadGroup = 3,
            [Display(Name = "New Saad employee", Order = 3)]
            newSaad = 4
        }

        public enum PaymentMehtod
        {
            [Display(Name = "Cash", Order = 1)]
            Cash = 1,
            [Display(Name = "Network", Order = 2)]
            Network = 2,
            [Display(Name = "Check", Order = 3)]
            Check =3,
            [Display(Name = "Visa-Master", Order = 4)]
            Visa_Master = 4,
            [Display(Name = "Bank transfer", Order = 5)]
            Bank_transfer = 5
        }

        public enum MonyType
        {
            [Display(Name ="Income",Order =1)]
            Income=0,
            [Display(Name = "expense ", Order = 2)]
            expense = 1
        }

        public static List<SelectListItem> GetEnumAsSelectList<E>() where E : Enum
        {
            return Enum.GetValues(typeof(E)).Cast<E>().OrderBy(e => e.GetDisplayOrder()).Select(e => new SelectListItem
            {
                Text = e.GetDisplayName(),
                Value = ((int)(object)e).ToString()
            }).ToList();
        }

        public static List<SelectListItem> GetEnumAsSelectListWithSelectedValue<E>(int selected) where E : Enum
        {
            return Enum.GetValues(typeof(E)).Cast<E>().OrderBy(e => e.GetDisplayOrder()).Select(e => new SelectListItem
            {
                Text = e.GetDisplayName(),
                Value = ((int)(object)e).ToString(),
                Selected = ((int)(object)e) == selected ? true : false
            }).ToList();
        }
    }
}
