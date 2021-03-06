//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HospitalInventory.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.Patients = new HashSet<Patient>();
        }

        public int EmployeeId { get; set; }
        public string employeeName { get; set; }
        [DisplayName("UserName")]
        [Required(ErrorMessage = "This Field is Required")]
        public string userName { get; set; }
        [RegularExpression(@"^\S*$", ErrorMessage = "White spaces are not allowed in password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This Field is Required")]
        public string password { get; set; }
        public string emailAddress { get; set; }
        public int designationId { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
        public string resetPasswordCode { get; set; }
        public virtual EmployeeDesignation EmployeeDesignation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Patient> Patients { get; set; }
    }
}

