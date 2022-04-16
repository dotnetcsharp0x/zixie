using System;
using System.ComponentModel.DataAnnotations;

namespace zixie.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(20,MinimumLength = 2, ErrorMessage ="Minimum length is 2 chars")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email template: smth@smth.smth")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z]).{8,20}$", ErrorMessage = "Password must be: <br>* 8-20 chars (a-z, A-Z)<br>* At least one number (0-9)<br>* At least one special character (!@#$%^&*)")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
