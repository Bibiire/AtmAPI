using Application.DTOs;
using Application.DTOs.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IUserAuthService
    {
        Task<BaseResponse> ChangePassword(string userId, ChangePasswordDTO changePassword);
        Task<BaseResponse> Login(LoginDTO loginDTO);
        Task<BaseResponse> UserLogin(UserLoginDTO loginDTO);
        Task<BaseResponse> RefreshToken(RefreshTokenDTO refreshToken);
        Task<BaseResponse> RegisterUser(RegisterUserDTO registerUser);
        Task<BaseResponse> ResetPassword(string email, ResetPasswordDTO resetPasswordModel);
    }
}
