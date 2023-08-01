using Application.DTOs;
using Application.DTOs.ApplicationUser;
using Application.Services.Interfaces;
using DataAccess;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.InfraInterfaces;
using System.Web;
using Microsoft.Extensions.Options;
using Application.ConfigSettings;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Services.Implementations.Auth
{
    public class UserAuthService : IUserAuthService
    {
        private readonly ILogger<UserAuthService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEncryptionService _encryptionService;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly AppEndpointSettings _appEndpointSettings;

        public UserAuthService(ILogger<UserAuthService> logger , UserManager<ApplicationUser> userManager, IRepositoryWrapper repositoryWrapper, IWebHostEnvironment environment,ITokenService tokenService, SignInManager<ApplicationUser> signinManager,IOptions<AppEndpointSettings> appEndpointSettings, IEncryptionService encryptionService)
        {
            _logger = logger;
            _userManager = userManager;
            _repositoryWrapper = repositoryWrapper;
            _environment = environment;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _signinManager = signinManager;
            _appEndpointSettings = appEndpointSettings.Value;
        }

        public async Task<BaseResponse> RegisterUser(RegisterUserDTO registerUser)
        {
            
            var adminEmail = await _userManager.FindByEmailAsync(registerUser.Email);
            if (adminEmail !=  null)
            {
                return BaseResponse.Failure("26", "Duplicate Record Found - Email already exist");
            }
            
            
            
            

            var user = new ApplicationUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                OtherName = registerUser.OtherName,
                RoleCategory = registerUser.RoleCategory,
                BVN = registerUser.BVN,
            };
            var accountDetails = new AccountDetail
            {
                AccountNumber = registerUser.AccountNumber,
                CurrentBalance = 0.00,
                Pin = _encryptionService.SHA512(registerUser.Pin),
                ApplicationUserId = user.Id
            };
            _repositoryWrapper.AccountDetail.Create(accountDetails);
            var password = registerUser.Password;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, new List<string> { registerUser.RoleCategory});
                user.RoleCategory = registerUser.RoleCategory;
                await _userManager.UpdateAsync(user);
                await _repositoryWrapper.Save();
                return BaseResponse.Success();
            }
            string error = result.Errors.FirstOrDefault().Description;
            _logger.LogInformation($"Register user terminated [Reason : Creating user failed | Error : {error}]\n");
            return BaseResponse.Failure("30",error);
        }
        public async Task<BaseResponse> Login(LoginDTO loginDTO)
        {
            var user = await _repositoryWrapper.ApplicationUser.GetUserDetailsWithEmail(loginDTO.Email);
            if(user is null)
            {
                _logger.LogInformation($"Login Terminated [Reason : Email not found | Email :  {loginDTO.Email}]");
                return BaseResponse.Failure("12", "Login failed - Email or password do not match for an existing user");
            }
            
            if (user.IsDeactivated)
            {
                _logger.LogInformation($"Login Terminated [Reason : Account is not active | Email :  {loginDTO.Email}]");
                return BaseResponse.Failure("12", "Login failed - Your account has been deactiavted. Please contact your admin for more details");
            }
            var passwordCheck = await _signinManager.PasswordSignInAsync(user, loginDTO.Password,false,true);
            if (passwordCheck.IsLockedOut)
            {
                _logger.LogInformation($"Login terminated [Reason : USer is locked out | Email : {loginDTO.Email}] \n");
                return BaseResponse.Failure("12", "Login failed - Your account is locked please try again with correct email and/or password in 60 minutes");
            }
            if (!passwordCheck.Succeeded)
            {
                _logger.LogInformation($"Login terminated [Reason : Password Check Failed | Email : {loginDTO.Email}] \n");
                return BaseResponse.Failure("12", "Login failed - Email or password do not match for an existing user");
            }          
            await _userManager.ResetAccessFailedCountAsync(user);
            var authResponse = await _tokenService.GetAuthenticationResultForUserAsync(user);
            return authResponse;
        }

        public async Task<BaseResponse> UserLogin(UserLoginDTO loginDTO)
        {
            var user = await _repositoryWrapper.ApplicationUser.GetUserDetailsWithEmail(loginDTO.Email);
            if (user is null)
            {
                _logger.LogInformation($"Login Terminated [Reason : Email not found | Email :  {loginDTO.Email}]");
                return BaseResponse.Failure("12", "Login failed - Email or password do not match for an existing user");
            }

            if (user.IsDeactivated)
            {
                _logger.LogInformation($"Login Terminated [Reason : Account is not active | Email :  {loginDTO.Email}]");
                return BaseResponse.Failure("12", "Login failed - Your account has been deactiavted. Please contact your admin for more details");
            }
            if (user.Account.Pin != _encryptionService.SHA512(loginDTO.Pin))
            {
                _logger.LogInformation($"Login terminated [Reason : Incorrect Pin | Email : {loginDTO.Email}] \n");
                return BaseResponse.Failure("12", "Login failed - Incorrect Pin");
            }
            await _userManager.ResetAccessFailedCountAsync(user);
            var authResponse = await _tokenService.GetAuthenticationResultForUserAsync(user);
            return authResponse;
        }
        public async Task<BaseResponse> RefreshToken(RefreshTokenDTO refreshToken)
        {
            _logger.LogInformation($"Refresh Token Request processing \n");
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken.Token);
            var userId = principal.Identity.Name;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogInformation($"Refresh Token terminated [Reason : User not found | Email : {user.Email}] \n");
                return BaseResponse.Failure("25", "User could not found");
            }
            if (user.RefreshToken != refreshToken.RefreshToken)
            {
                _logger.LogInformation($"Login terminated [Reason : Invalid Refresh Token | Email : {user.Email}] \n");
                return BaseResponse.Failure("12", "Invalid refresh token");
            }
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                _logger.LogInformation($"Login terminated [Reason : Refresh Token has expired | Email : {user.Email}] \n");
                return BaseResponse.Failure("12", "Invalid refresh token - token has expired");
            }

            var authResponse = await _tokenService.GetAuthenticationResultForUserAsync(user);
            return authResponse;
        }
        public async Task<BaseResponse> ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            _logger.LogInformation($"Forgot Password Processing [Email : {forgotPassword.Email}] \n");
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var email = user.UserName;

                var passwordResetLink = $"{_appEndpointSettings.FrontendBaseUrl}{_appEndpointSettings.ResetPassword}?email={HttpUtility.UrlEncode(email)}" +
                 $"&emailToken={HttpUtility.UrlEncode(token)}";

                
                return  BaseResponse.Success("A password reset link would be sent to the email address if it exist");
            }
            _logger.LogInformation($"Forgot Password Terminated [ Reason :Email Does not exist | Email : {forgotPassword.Email}] \n");
            return BaseResponse.Failure( "00", "A password reset link would be sent to the email address if it exist");
        }
        public async Task<BaseResponse> ResetPassword(string email, ResetPasswordDTO resetPasswordModel)
        {
            _logger.LogInformation($"Reset Password Processing [Email : {email} ] \n");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                _logger.LogInformation($"ResetPassword terminated [Reason : User not found | Email : {email}] \n");
                return BaseResponse.Failure("25", "User record not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var userPassword = await _userManager.ResetPasswordAsync(user, token, resetPasswordModel.Password);
            if (userPassword.Succeeded)
            {
                user.EmailConfirmed = true;
                user.LockoutEnd = null;
                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.UpdateAsync(user);

                
                return BaseResponse.Success("Password was changed successfully");
            }
            else if (!userPassword.Succeeded && userPassword.Errors.Any(x => x.Code == "InvalidToken"))
            {
                _logger.LogInformation($"Reset Password terminated [ Reason :Invalid Token | Email : {email}] \n");
                return BaseResponse.Failure("12", "Invalid Token");

            }
            return BaseResponse.Failure("12", userPassword.Errors.FirstOrDefault().Description);
        }
        public async Task<BaseResponse> ChangePassword(string userId, ChangePasswordDTO changePassword)
        {
            _logger.LogInformation($"Change Password [UserID :{userId}]\n");

            if (changePassword.NewPassword == changePassword.CurrentPassword)
            {
                return BaseResponse.Failure("30", "Current password cannot be the same with new password" );
            }

            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.UpdateAsync(user);

                return BaseResponse.Success("Password was changed successfully" );
            }
            return BaseResponse.Failure("12",result.Errors.FirstOrDefault().Description);
        }

    }
}
