using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string UserId { get; set; }
        public string TrandsctionType { get; set; }
        public double Amount { get; set; }
        
    }
}
