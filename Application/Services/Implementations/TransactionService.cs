using Application.DTOs;
using Application.DTOs.Transaction;
using Application.Services.Implementations.Auth;
using Application.Services.Interfaces;
using DataAccess;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<UserAuthService> _logger;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public TransactionService(ILogger<UserAuthService> logger, IRepositoryWrapper repositoryWrapper)
        {
            _logger = logger;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<BaseResponse> Deposit(DepositDTO request)
        {
            var user = await _repositoryWrapper.ApplicationUser.Find(u => u.Id == request.UserId);
            if (user == null)
            {
                return BaseResponse.Failure("99", "User does not exist");
            }
            
            var account = await _repositoryWrapper.AccountDetail.Find(a => a.ApplicationUserId == request.UserId);
            var newbalance = account.CurrentBalance + request.Amount;
            _repositoryWrapper.Transaction.Create(new Transaction
            {
                Amount = request.Amount,
                UserId = request.UserId,
                TrandsctionType = "Deposit"
            });
            account.CurrentBalance = newbalance;
            _repositoryWrapper.AccountDetail.Update(account);
            await _repositoryWrapper.Save();
            return BaseResponse.Success();

        }

        public Task<BaseResponse> GetTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> GetTransactions(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> Withdraw(DepositDTO request)
        {
            var user = await _repositoryWrapper.ApplicationUser.Find(u => u.Id == request.UserId);
            if (user == null)
            {
                return BaseResponse.Failure("99", "User does not exist");
            }

            var account = await _repositoryWrapper.AccountDetail.Find(a => a.ApplicationUserId == request.UserId);
            if (account.CurrentBalance < request.Amount)
            {
                return BaseResponse.Failure("99", "Insufficient fund");
            }
            var newbalance = account.CurrentBalance + request.Amount;
            _repositoryWrapper.Transaction.Create(new Transaction
            {
                Amount = request.Amount,
                UserId = request.UserId,
                TrandsctionType = "Deposit"
            });
            account.CurrentBalance = newbalance;
            _repositoryWrapper.AccountDetail.Update(account);
            await _repositoryWrapper.Save();
            return BaseResponse.Success();
        }
    }
}
