using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AccountDetail : BaseEntity
    {
        [Required]
        [StringLength(10)]
        public string AccountNumber { get; set; }
        [Required]
        public string Pin { get; set; }
        public double CurrentBalance { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
