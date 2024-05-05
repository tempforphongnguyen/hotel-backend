using B2Net.Models;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Auth;
using Infrastructure.ViewModel.Auth.User;
using Microsoft.AspNetCore.Identity;

namespace Domain.IService
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto model);
        Task<string> AddRoleToUserAsync(AddRoleToUserDto model);
        Task<IdentityResult> CreateRole(CreateRoleDTO model);
        Task<AuthenticationResponse> Login(LoginDTO model);
        Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
        Task<bool> RequestForgotPassword(string userMail);
        Task<bool> RequestForgotPassword(ForgotPasswordAccuracy accuracy);
        Task<ResponseEnum> SetupPasswordWhenForgot(ForgotPasswordConfirm confirm);
        Task<UserInfo> GetUserInfo(Guid id, string role);
        Task<UserInfo> GetUserInfo(Guid id);
        Task<bool> UpdateUserInfo(UserInfo userInfo);
        Task<string> UpdateAvatarUser(UserUploadImage userUpdateImage);
        Task<string> ChangePassword(ChangePasswordForm changePasswordForm);
        Task<IList<UserInfo>> GetListUser(PagingListPropModel pagingListPropModel);
        Task<int> GetTotalUser(PagingListPropModel pagingListPropModel);
        Task<string> UpdateUserMerit(Guid userId, decimal totalPaid);
        Task<List<UserInfo>> GetUsers(List<Guid> userIds);
    }
}
