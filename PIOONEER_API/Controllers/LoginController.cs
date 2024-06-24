using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using PIOONEER_Service.Interface;
using System.Net;
using Tools;

namespace PIOONEER_API.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ILoginService _loginService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public LoginController(ILoginService loginService,IEmailService emailService,IUserService userService)
        {
            _loginService = loginService;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous] // Allow anonymous access to the login endpoint
        public async Task<IActionResult> Login([FromBody] PIOONEER_Model.DTO.LoginRequest loginRequest)
        {
            
            var result = await _loginService.AuthorizeUser(loginRequest);
            if (result.Token != null)
            {
                return CustomResult("Login successful.", new { result.Token, LoginResponse = result.LoginResponse });
            }
            else
            {
                return CustomResult("Invalid email or password.", HttpStatusCode.Unauthorized);
            }
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] PIOONEER_Model.DTO.ForgotPasswordRequest request)
        {
            var user = _userService.GetUserByEmail(request.Email);

            if (user == null)
            {
                return CustomResult("Email not found.", HttpStatusCode.NotFound);
            }

            await _emailService.SendOtpEmailAsync(request.Email);
            return CustomResult("OTP sent to your email.");
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] PIOONEER_Model.DTO.ResetPasswordRequest request)
        {
            var response = await _userService.ResetPasswordAsync(request);

            if (response.Message == "Password reset successfully.")
            {
                return CustomResult(response.Message);
            }
            else
            {
                return CustomResult(response.Message, HttpStatusCode.BadRequest);
            }
        }
    }
}
