using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalInventory.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage ="Current Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage ="New Password is Required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "White spaces are not allowed in password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "White spaces are not allowed in password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match!!")]
        public string ConfirmPassword { get; set; }
    }
}