using Application.DTOs;
using Application.DTOs.ApplicationUser;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<BaseResponse<LoginResponseDTO>> GetAuthenticationResultForUserAsync(ApplicationUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
