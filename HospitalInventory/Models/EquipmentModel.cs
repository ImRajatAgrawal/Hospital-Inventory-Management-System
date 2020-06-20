using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalInventory.Models
{
    public class EquipmentModel
    {
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "quantity is required")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "Equipment name is required")]
        [Display(Name = "Equipment Name")]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Equipment Category is required")]
        [Display(Name = "Equipment Category")]
        public int EquipmentCategoryId { get; set; }
    }
}