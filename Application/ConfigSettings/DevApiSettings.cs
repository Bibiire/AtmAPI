using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ConfigSettings
{
    public class DevApiSettings
    {
       
        public ProductKey ProductKeys { get; set; }
    }

    public class ProductKey
    {
        public string ClientId { get; set; }
        public string ProductId { get; set; }
        public string Password { get; set; }
        public string SubscriptionKey { get; set; }
        public string VectorKey { get; set; }
        
    }
}
