using Application.InfraInterfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrasturctureService(this IServiceCollection services)
        {
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IHttpConnection, HttpConnection>();

            return services;
        }
    }
}
