using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transaction
{
    public class IntraTransferRequestDTO
    {
        public string UserId { get; set; }
        public string TrandsctionType { get; set; }
        public double Amount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
