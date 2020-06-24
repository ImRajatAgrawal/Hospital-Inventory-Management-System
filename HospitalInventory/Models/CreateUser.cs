using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalInventory.Models
{
    public class CreateUser
    {
        [Display(Name = "Employee Name")]
        [Required(ErrorMessage = "Employee Name is Required")]
        public string employeeName { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name is Required")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\S*$", ErrorMessage = "White spaces are not allowed in password")]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Display(Name = "EmailID")]
        [Required(ErrorMessage = "EmailID is Required")]
        [DataType(DataType.EmailAddress)]
        public string emailId { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is Required")]
        public string empDesignation { get; set; }
    }

}