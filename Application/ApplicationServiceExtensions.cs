using Application.Services.Implementations;
using Application.Services.Implementations.Auth;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<ITransactionService, TransactionService>();
            

            return services;
        }
    }
}
