using Comman.DTOs.CommanDTOs;
using UserService.DTOs;

namespace UserService.Repository.UserProfile;

public class UserProfileRepository(

) : IUserProfileRepository
{
    public async Task<ListResult<UserProfileListDTO>> GetUserProfilePage()
    {
        var response = new ListResult<UserProfileListDTO>();
        return response;
    }
    public async Task<UserProfileViewDTO> GetUserProfileView(int userID)
    {
        var response = new UserProfileViewDTO();
        return response;
    }
    public async Task<UserProfileUpdateDTO> GetUserProfilePK(int userID)
    {
        var response = new UserProfileUpdateDTO();
        return response;
    }
    public async Task<OperationResultDTO> CreateUserProfile(UserProfileCreateDTO dto)
    {
        var response = new OperationResultDTO();
        return response;
    }
    public async Task<OperationResultDTO> UpdateUserProfile(UserProfileUpdateDTO dto)
    {
        var response = new OperationResultDTO();
        return response;
    }
}
