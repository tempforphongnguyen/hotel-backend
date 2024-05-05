
using B2Net.Models;
using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.Mail;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Auth;
using Infrastructure.ViewModel.Auth.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Domain.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHelperService<User> _helperService;
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;
        private readonly IUploadFileService _uploadFileService;
        private readonly INotificationService _notificationService;
        private readonly string DefaultGuestRoleId = "40dccaf1-fea6-4bb9-ad59-7ca2c4161c16";

        public AuthService(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, ISendMailService sendMailService, IUploadFileService uploadFileService, IHelperService<User> helperService, INotificationService notificationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _sendMailService = sendMailService;
            _uploadFileService = uploadFileService;
            _helperService = helperService;
            _notificationService = notificationService;
        }

        public async Task<string> AddRoleToUserAsync(AddRoleToUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
            if (user is null || role is null)
                return ResponseEnum.InvaildUserOrRole.ToString();

            if (await _userManager.IsInRoleAsync(user, role.Name))
                return ResponseEnum.UserVaildRole.ToString();

            IList<string> existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
    
                var result = await _userManager.AddToRoleAsync(user, role.Name);

            return result.Succeeded ? ResponseEnum.Success.ToString() : ResponseEnum.SomewhereWrong.ToString();
        }

        public async Task<IdentityResult> CreateRole(CreateRoleDTO model)
        {
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = model.RoleName });
            return result;
        }

        public async Task<AuthenticationResponse> Login(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) return new AuthenticationResponse();

            bool isValidUser = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!isValidUser) return new AuthenticationResponse();


            string accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.ExpRefreshToken = DateTime.UtcNow.AddDays(1);
            await _userManager.UpdateAsync(user);

            var response = new AuthenticationResponse
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                Success = true
            };

            return response;
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthDto { Message = ResponseEnum.EmailAlreadyExisted.ToString() };

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FirstName + " " + model.LastName,
                Avatar = "nan.png",
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Membership = MembershipEnum.Default.ToString(),
                Merit = 0,
                CreateDate = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthDto { Message = errors };
            }

            var role = await _roleManager.FindByIdAsync(DefaultGuestRoleId);

            var resultAddDefaultRole = await _userManager.AddToRoleAsync(user, role.Name);

            if (!resultAddDefaultRole.Succeeded)
            {
                var errorsRole = string.Empty;

                foreach (var error in resultAddDefaultRole.Errors)
                    errorsRole += $"{error.Description},";

                return new AuthDto { Message = errorsRole };
            }

            return new AuthDto
            {
                Email = user.Email,
                IsAuthenticated = true,
                Username = user.UserName
            };
        }

        public async Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var response = new AuthenticationResponse();
            var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);

            if (principal != null)
            {
                var email = principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email?.Value);

                if (user is null || user.RefreshToken != refreshTokenRequest.RefreshToken || user.ExpRefreshToken < DateTime.UtcNow)
                {
                    response.Success = false;
                    return response;
                }

                string newAccessToken = GenerateAccessToken(user);
                string refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.ExpRefreshToken = DateTime.UtcNow.AddDays(1);
                await _userManager.UpdateAsync(user);

                response.AccessToken = newAccessToken;
                response.RefreshToken = refreshToken;
                response.Success = true;

                return response;
            }

            return response;
        }

        public async Task<bool> RequestForgotPassword(string userMail)
        {
            var user = await _userManager.FindByNameAsync(userMail);

            if (user is null)
            {
                return false;
            }

            var rnd = new Random();
            var forgotPasswordCode = rnd.Next(10000, 99999);

            user.ForgotPasswordCode = forgotPasswordCode;
            await _userManager.UpdateAsync(user);

            var mailRequest = new MailRequest()
            {
                ToEmail = userMail,
                Subject = "Forgot password code",
                Body = "Hi " + userMail + " your forgot password code is: " + forgotPasswordCode
            };

            await _sendMailService.SendEmailWithoutAttachmentAsync(mailRequest);

            return true;
        }

        public async Task<bool> RequestForgotPassword(ForgotPasswordAccuracy accuracy)
        {
            var user = await _userManager.FindByNameAsync(accuracy.UserEmail);

            if (user is null)
            {
                return false;
            }

            if (user.ForgotPasswordCode == accuracy.ForgotPasswordCode && accuracy.ForgotPasswordCode >= 10000)
            {
                return true;
            }

            return false;
        }

        public async Task<ResponseEnum> SetupPasswordWhenForgot(ForgotPasswordConfirm confirm)
        {
            var user = await _userManager.FindByNameAsync(confirm.UserEmail);

            if (user is null)
            {
                return ResponseEnum.SomewhereWrong;
            }

            if (user.ForgotPasswordCode != confirm.ForgotPasswordCode || confirm.ForgotPasswordCode < 10000)
            {
                return ResponseEnum.IncorrectResetPwdCode;
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, confirm.Password);

            user.ForgotPasswordCode = 0;

            await _userManager.UpdateAsync(user);

            return ResponseEnum.Success ;
        }

        public async Task<string> UpdateAvatarUser(UserUploadImage userUpdateImage)
        {
            var user = await _userManager.FindByIdAsync(userUpdateImage.Id.ToString());
            if (user == null)
            {
                return string.Empty;
            }

            var fileName = await _uploadFileService.UploadFileAvatar(userUpdateImage.File);

            user.Avatar = fileName;

            await _userManager.UpdateAsync(user);

            return fileName;
        }

        public async Task<UserInfo> GetUserInfo(Guid id, string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roleName =  _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var roleInfo = await _roleManager.FindByNameAsync(roleName);
            if (user == null)
            {
                return null;
            }

            var userInfo = new UserInfo()
            {
                Id = user.Id,
                Email = user.Email,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roleInfo.Name,
                RoleId = roleInfo.Id,
                Membership = user.Membership,
                Merit = user.Merit,
                CreateDate = user.CreateDate
            };

            return userInfo;
        }

        public async Task<UserInfo> GetUserInfo(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var userInfo = new UserInfo()
            {
                Id = user.Id,
                Email = user.Email,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Membership = user.Membership,
                Merit = user.Merit
            };

            return userInfo;
        }

        public async Task<bool> UpdateUserInfo(UserInfo userInfo)
        {
            var user = await _userManager.FindByIdAsync(userInfo.Id.ToString());
            if (user == null)
            {
                return false;
            }
            
            user.DateOfBirth =  DateTime.Parse(userInfo.DateOfBirth.ToString(), null, DateTimeStyles.AdjustToUniversal);
            user.FullName = userInfo.FirstName + " " + userInfo.LastName;
            user.FirstName = userInfo.FirstName; 
            user.LastName = userInfo.LastName;
            user.PhoneNumber = userInfo.PhoneNumber;
            user.Avatar = userInfo.Avatar;

            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateAvatar(UserInfo userInfo)
        {
            var user = await _userManager.FindByIdAsync(userInfo.Id.ToString());
            if (user == null)
            {
                return false;
            }

            user.Avatar = userInfo.Avatar;

            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<string> ChangePassword(ChangePasswordForm changePasswordForm)
        {
            var user = await _userManager.FindByIdAsync(changePasswordForm.Id.ToString());
            if (user == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            bool isValidUser = await _userManager.CheckPasswordAsync(user, changePasswordForm.OldPassword);
            
            if(!isValidUser)
            {
                return ResponseEnum.IncorrectCurrentPassword.ToString();
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordForm.OldPassword, changePasswordForm.NewPassword);

            return result.Succeeded ? ResponseEnum.Success.ToString() : ResponseEnum.SomewhereWrong.ToString();
        }

        public async Task<List<UserInfo>> GetUsers(List<Guid> userIds)
        {
            var users = _userManager.Users.Where(s=> userIds.Contains(s.Id)).ToList();
            var userInfos = new List<UserInfo>();
            
            foreach (var user in users)
            {
                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                var userInfo = new UserInfo()
                {
                    Avatar = user.Avatar,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Id = user.Id,
                    Membership = user.Membership,
                    Merit = user.Merit,
                    PhoneNumber = user.PhoneNumber,
                    CreateDate = user.CreateDate,
                };

                userInfos.Add(userInfo);
            }

            return userInfos.ToList();
        }

        public async Task<IList<UserInfo>> GetListUser(PagingListPropModel pagingListPropModel)
        {
            var isVaildProp = false;
            PropertyInfo propertyInfo = null;

            if (!pagingListPropModel.SortBy.IsNullOrEmpty())
            {
                isVaildProp = _helperService.IsValidProperty(pagingListPropModel.SortBy);

                if (!isVaildProp)
                {
                    throw new ArgumentException("Invalid property name", nameof(pagingListPropModel.SortBy));
                }

                propertyInfo = _helperService.GetPropertyInfo(pagingListPropModel.SortBy);
            }

            //var userQuery = pagingListPropModel.SearchText.IsNullOrEmpty() ? _userManager.Users : _userManager.Users.Where(u => u.FirstName.Contains(pagingListPropModel.SearchText));
            var userQuery = pagingListPropModel.SearchText.IsNullOrEmpty() ? _userManager.Users.Where(u => u.Id != Guid.Parse("f2b70f74-c5d4-4f08-9cff-a3fd2f660754")) : _userManager.Users.Where(u => u.FirstName.Contains(pagingListPropModel.SearchText) && u.Id != Guid.Parse("f2b70f74-c5d4-4f08-9cff-a3fd2f660754"));

            userQuery = isVaildProp ? pagingListPropModel.SortDesc ? userQuery.OrderBy(pagingListPropModel.SortBy + " descending") : userQuery.OrderBy(pagingListPropModel.SortBy)
                                : userQuery;

            var users = await userQuery.Skip((pagingListPropModel.PageNumber - 1) * pagingListPropModel.PageSize).Take(pagingListPropModel.PageSize).ToListAsync();

           
            var userInfos = new List<UserInfo>();
            foreach(var user in users)
            {
                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                var userInfo = new UserInfo()
                {
                    Avatar = user.Avatar,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Id = user.Id,
                    Membership = user.Membership,
                    Merit = user.Merit,
                    PhoneNumber = user.PhoneNumber,
                    Role = role.IsNullOrEmpty() ? RoleEnum.Guest.ToString() : role,
                    CreateDate = user.CreateDate,
                };
                
                userInfos.Add(userInfo);
            }
            return userInfos;
        }

        public async Task<int> GetTotalUser(PagingListPropModel pagingListPropModel)
        {
            var total = 0;
            total = pagingListPropModel.SearchText.IsNullOrEmpty() ?await _userManager.Users.CountAsync() : await _userManager.Users.CountAsync(u => u.FirstName.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<string> UpdateUserMerit(Guid userId, decimal totalPaid)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            user.Merit += totalPaid;

            var memberShip = user.Membership;

            if (user.Membership != MembershipEnum.Gold.ToString())
            {
                if (user.Merit > 10000000 && user.Merit <= 20000000)
                {
                    user.Membership = MembershipEnum.Silver.ToString();
                }
                else if (user.Merit > 20000000)
                {
                    user.Membership = MembershipEnum.Gold.ToString();
                }
            }

            if(memberShip != user.Membership)
            {
                await _notificationService.AutoCreateWithType(CreateNotiTypeEnum.Membership, user.Membership, user.Id);
            }

            await _userManager.UpdateAsync(user);

            return ResponseEnum.Success.ToString();
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            var claims = new List<Claim>
            {
                new Claim(type:"Id", value: user.Id.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(type:"FirstName", value: user.FirstName),
                new Claim(type:"LastName", value: user.LastName),
                new Claim(ClaimTypes.Email, user.Email)
                
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["JWT:Audience"],
                Issuer = _configuration["JWT:Issuer"],
                //Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt16(_configuration["JWT:DurationInMinutes"])),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyDetail), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(keyDetail)
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
