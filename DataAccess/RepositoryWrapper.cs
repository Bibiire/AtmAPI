using DataAccess.Implementations;
using DataAccess.Interfaces;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationDbContext _context;
        private IApplicationUserRepository _applicationUser;
        private IAccountDetailRepository _accountDetail;
        private ITransactionRepository _transaction;

        public RepositoryWrapper(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        

        

        public IApplicationUserRepository ApplicationUser
        {
            get
            {
                if (_applicationUser == null)
                {
                    _applicationUser = new ApplicationUserRepository(_context);
                }
                return _applicationUser;
            }
        }

        

        public IAccountDetailRepository AccountDetail
        {
            get
            {
                if (_accountDetail == null)
                {
                    _accountDetail = new AccountDetailRepository(_context);
                }
                return _accountDetail;
            }
        }

        public ITransactionRepository Transaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = new TrandsactionRepository(_context);
                }
                return _transaction;
            }
        }


    }
}
