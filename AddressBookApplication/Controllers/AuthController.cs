using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace AddressBookApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserBL _userService;

        public AuthController(IUserBL userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// This method is used to Register the User
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserEntity>> Register([FromBody] UserEntity user)
        {
            ResponseModel<UserEntity> response = new ResponseModel<UserEntity>();

            await _userService.Register(user);
            response.Success = true;
            response.Message = "User Registered Successfully";
            response.Data = user;
            return Ok(response);
        }

        /// <summary>
        /// This method is used to Login a User
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDTO userDTO)
        {
            ResponseModel<UserEntity> response = new ResponseModel<UserEntity>();
            var token = await _userService.Login(userDTO);
            if (token == null)
            {
                response.Success = false;
                response.Message = "Invalid credentials";
                response.Data = null;
                return Unauthorized(response);
            }

            response.Success = true;
            response.Message = "User Logged In Successfully";
            response.Data = token;
            return Ok(new { Token = token });
        }

        /// <summary>
        /// This method is used to Trigger Forget Password
        /// </summary>
        /// <param name="forgotPasswordDTO"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            var result = await _userService.GenerateResetTokenAsync(forgotPasswordDTO.Email);
            if (result == null)
            {
                response.Success = false;
                response.Message = "User Not Found!";
                response.Data = null;

                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Password reset link has been sent to your email.";
            response.Data = result;

            return Ok(response);
        }

        /// <summary>
        /// This method is used to Reset the user's password
        /// </summary>
        /// <param name="resetPasswordDTO"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            ResponseModel<bool> response = new ResponseModel<bool>();
            var result = await _userService.VerifyResetTokenAsync(resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
            if (!result)
            {
                response.Success = false;
                response.Message = "Invalid or Expired Token.";
                response.Data = result;
            }

            response.Success = true;
            response.Message = "Password has been reset successfully.";
            response.Data = result;

            return Ok(response);
        }
    }
}
