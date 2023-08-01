using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transaction
{
    public class DepositDTO
    {
        public string UserId { get; set; }
        public double Amount { get; set; }
    }
}
