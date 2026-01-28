using Comman.DTOs.CommanDTOs;
using UserService.DTOs;

namespace UserService.Services.UserProfile;

public interface IUserProfileService
{
    Task<ListResult<UserProfileListDTO>>    GetUserProfilePage();
    Task<UserProfileViewDTO>                GetUserProfileView(int userID);
    Task<UserProfileUpdateDTO>              GetUserProfilePK(int userID);
    Task<OperationResultDTO>                CreateUserProfile(UserProfileCreateDTO dto);
    Task<OperationResultDTO>                UpdateUserProfile(UserProfileUpdateDTO dto);
}
