using Application.DTOs.ApplicationUser;
using Application.DTOs;
using Application.Services.Interfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AtmAPI.Controllers.V1._01
{
    [ApiController]
    [Route("v{version:apiVersion}/api/[controller]")]
    [ApiVersion("1.01")]
    public class UserAuthController : ControllerBase
    {
        private readonly ILogger<UserAuthController> _logger;
        private readonly IUserAuthService _userAuthService;
        public UserAuthController(ILogger<UserAuthController> logger, IUserAuthService userAuthService)
        {
            _logger = logger;
            _userAuthService = userAuthService;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var response = await _userAuthService.RegisterUser(registerUser);
            if (response.Code == "00") return Ok(response);
            return BadRequest(response);

        }

        /// <summary>
        /// Signin User
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse<LoginResponseDTO>))]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var loginResponse = await _userAuthService.Login(login);
            if (loginResponse.Code is "00") return Ok(loginResponse);
            if (loginResponse.Code is "25") return NotFound(loginResponse);
            return BadRequest(loginResponse);
        }

        
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="email"></param>
        /// <param name="emailToken"></param>
        /// <returns></returns>
        [HttpPatch("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO viewModel, [Required, EmailAddress] string email)
        {
            _logger.LogInformation($"Reset Password Request \n");
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var loginResponse = await _userAuthService.ResetPassword(email, viewModel);
            if (loginResponse.Code is "00") return Ok(loginResponse);
            if (loginResponse.Code is "25") return NotFound(loginResponse);
            return BadRequest(loginResponse);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(404, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse<LoginResponseDTO>))]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO refreshToken)
        {
            _logger.LogInformation($"Refresh Token Request \n");
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            var loginResponse = await _userAuthService.RefreshToken(refreshToken);
            if (loginResponse.Code is "00") return Ok(loginResponse);
            if (loginResponse.Code is "25") return NotFound(loginResponse);
            return BadRequest(loginResponse);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("[action]")]
        [Authorize]
        [ProducesResponseType(400, Type = typeof(BaseResponse))]
        [ProducesResponseType(200, Type = typeof(BaseResponse))]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            _logger.LogInformation($"Change Password Request \n");
            if (!ModelState.IsValid) return BadRequest(ResponseHelper.BuildResponse("30", ModelState));
            string userId = User.FindFirst("UserId")?.Value;
            var response = await _userAuthService.ChangePassword(userId, model);
            if (response.Code is "00") return Ok(response);
            return BadRequest(response);
        }
    }
}
