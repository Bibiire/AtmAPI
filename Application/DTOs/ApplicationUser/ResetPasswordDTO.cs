using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ApplicationUser 
{
    public class ResetPasswordDTO
    {
        [Required, RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-,.]).{8,}$", ErrorMessage = "Password does not meet the valid requirement.")]
        public string Password { get; set; }
    }
}
