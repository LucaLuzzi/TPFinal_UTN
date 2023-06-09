using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TPFinal_LuzziLuca.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:50, MinimumLength =5, ErrorMessage = "The {0} field must be between 5 and 50 characters")]
        [Display(Name ="Full Name")]
        //[Remote(action: "VerifyExistEmployee", controller: "Employees")]
        public string Fullname { get; set; }

        [Range(minimum: 18, maximum: 60, ErrorMessage ="The value must be between {1} and {2}")]
        public int Age { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="The {0} must be correct")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Job")]
        public string JobName { get; set; }

        [Range(minimum: 0, maximum: 1000000, ErrorMessage = "The value must be between {1} and {2}")]
        public decimal Salary { get; set; }

    }
}
