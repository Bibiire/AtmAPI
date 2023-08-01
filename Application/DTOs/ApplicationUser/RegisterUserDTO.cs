using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ApplicationUser
{
    public class RegisterUserDTO
    {
       
        [Required]
        public string BVN { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string OtherName { get; set; }
        [Required]
        [StringLength(4)]
        public string Pin { get; set; }
        public string AccountNumber { get; set; }
        [Required]
        public string RoleCategory { get; set; }
        [Required,EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}