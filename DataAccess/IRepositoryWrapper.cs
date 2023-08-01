using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRepositoryWrapper
    {
       
        IAccountDetailRepository AccountDetail { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ITransactionRepository Transaction { get; }
        

        Task<int> Save();
    }
}
