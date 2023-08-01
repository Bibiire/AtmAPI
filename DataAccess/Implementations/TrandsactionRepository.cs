using DataAccess.Interfaces;
using Domain.Entities;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implementations
{
    public class TrandsactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TrandsactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
