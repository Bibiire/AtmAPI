using Application.DTOs;
using Application.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<BaseResponse> Deposit(DepositDTO request);
        Task<BaseResponse> Withdraw(DepositDTO request);
        Task<BaseResponse> GetTransactions();
        Task<BaseResponse> GetTransactions(string userId);
    }
}
