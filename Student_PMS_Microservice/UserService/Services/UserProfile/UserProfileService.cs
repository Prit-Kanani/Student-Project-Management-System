using Comman.DTOs.CommanDTOs;
using UserService.DTOs;
using UserService.Repository.UserProfile;

namespace UserService.Services.UserProfile;

public class UserProfileService(
    IUserProfileRepository repository
) : IUserProfileService
{
    public async Task<ListResult<UserProfileListDTO>> GetUserProfilePage()
    {
        return await repository.GetUserProfilePage();
    }

    public async Task<UserProfileViewDTO> GetUserProfileView(int userID)
    {
        return await repository.GetUserProfileView(userID);
    }

    public async Task<UserProfileUpdateDTO> GetUserProfilePK(int userID)
    {
        return await repository.GetUserProfilePK(userID);
    }

    public async Task<OperationResultDTO> CreateUserProfile(UserProfileCreateDTO dto)
    {
        return await repository.CreateUserProfile(dto);
    }

    public async Task<OperationResultDTO> UpdateUserProfile(UserProfileUpdateDTO dto)
    {
        return await repository.UpdateUserProfile(dto);
    }
}
