using Domain.Service;
using Infrastructure.ViewModel.Auth;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.IService;
using System.ComponentModel;
using Infrastructure.ViewModel.Auth.User;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Enum;
using Microsoft.Extensions.Logging;

namespace HotelApi.Controller
{
    //User: admin - Password: @dmiN123 email: admin@gmail.com
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
            {
                response.IsSuccess = false;
                response.ErrorMessage = result.Message;
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(result);
        }


        [HttpPost]
        [Route("AddRoleToUser")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserDto model)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.AddRoleToUserAsync(model);

            if (result != ResponseEnum.Success.ToString())
            {
                response.ErrorMessage = result;
                response.IsSuccess = false;
                return BadRequest(response) ;
            }

            return Ok(result);
        }



        [HttpPost]
        [Route("CreateRole")]
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _authService.CreateRole(model);
            if (!response.Succeeded) return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.Login(model);
            if (!result.Success) return Unauthorized();

            var response = new MainResponse
            {
                Content = new AuthenticationResponse
                {
                    RefreshToken = result.RefreshToken,
                    AccessToken = result.AccessToken,
                    Success = true
                },
                IsSuccess = true,
                ErrorMessage = ""
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var response = new MainResponse();

            if (refreshTokenRequest is null)
            {
                response.ErrorMessage = ResponseEnum.InvalidRequest.ToString();
                response.IsSuccess = false;

                return BadRequest(response);
            }

            var refreshToken = await _authService.RefreshToken(refreshTokenRequest);

            if (!refreshToken.Success)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Content = new AuthenticationResponse
            {
                RefreshToken = refreshToken.RefreshToken,
                AccessToken = refreshToken.AccessToken,
                Success = true
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> RequestForgotPassword(string userMail)
        {
            var result = await _authService.RequestForgotPassword(userMail);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (!result)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.EmailNotFound.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("CheckForgotPasswordCode")]
        public async Task<IActionResult> CheckForgotPasswordCode(ForgotPasswordAccuracy accuracy)
        {
            var result = await _authService.RequestForgotPassword(accuracy);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (!result)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("SetupPasswordWhenForgot")]
        public async Task<IActionResult> SetupPasswordWhenForgot(ForgotPasswordConfirm passwordConfirm)
        {
            var result = await _authService.SetupPasswordWhenForgot(passwordConfirm);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (result != ResponseEnum.Success)
            {
                response.IsSuccess = false;
                response.ErrorMessage = result.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(Guid id)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var role = User.Claims.FirstOrDefault(s => s.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            if (role.IsNullOrEmpty())
            {
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                response.IsSuccess = false;
                return BadRequest(response);
            }

            if (role == RoleEnum.Guest.ToString())
            {
                var userRequestId = User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;
                if(userRequestId.IsNullOrEmpty() || userRequestId != id.ToString())
                {
                    response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
            }

            var result = await _authService.GetUserInfo(id, role);

            if (result == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [HttpPost("EditUserInfo")]
        public async Task<IActionResult> EditUserInfo(UserInfo userInfo)
        {
            var result = await _authService.UpdateUserInfo(userInfo);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (result == false)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordForm changePasswordForm)
        {
            var response = new MainResponse();

            var userRequestId = User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;

            if (userRequestId.IsNullOrEmpty() || userRequestId != changePasswordForm.Id.ToString())
            {
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                response.IsSuccess = false;
                return BadRequest(response);
            }

            var result = await _authService.ChangePassword(changePasswordForm);

            
            if (result != ResponseEnum.Success.ToString())
            {
                response.IsSuccess = false;
                response.ErrorMessage = result;
                return BadRequest(response);
            }

            response.Content = result;
            response.IsSuccess = true;

            return Ok(response);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] UserUploadImage userInfo)
        {
            var result = await _authService.UpdateAvatarUser(userInfo);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (result.IsNullOrEmpty())
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingListWithPropAndFilter(PagingListPropModel pagingListPropModel)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var users = await _authService.GetListUser(pagingListPropModel);

            var total = await _authService.GetTotalUser(pagingListPropModel);

            response.Content = new { data = users, total };

            return Ok(response);
        }

        #region Do not use regularly

        #endregion
    }
}
