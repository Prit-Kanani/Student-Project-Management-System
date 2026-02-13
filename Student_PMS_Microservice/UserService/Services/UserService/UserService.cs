using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using ProjectGroup.Services.UserService;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Repository.UserRepository;

namespace UserService.Services.UserService;

public class UsersService : IUserService
{
    #region CONFIGURATION
    private readonly IUserRepository _repository;
    //private readonly MicroserviceGateway _gateway;

    public UsersService(IUserRepository repository /*, MicroserviceGateway gateway*/) 
        => _repository = repository;//_gatway = gatway;
    #endregion

    #region GET USERS PAGE
    public async Task<ListResult<UserListDTO>> GetUsersPage()
    {
        var result = await _repository.GetUsersPage();
        return result;
    }
    #endregion

    #region GET USER VIEW
    public async Task<UserViewDTO> GetUserView(int userID)
    {
        var result = await _repository.GetUserView(userID);
        return result;
    }
    #endregion

    #region GET USER PK
    public async Task<UserUpdateDTO> GetUserPK(int userID)
    {
        var result = await _repository.GetUserPK(userID);
        return result;
    }
    #endregion

    #region CREATE USER
    public async Task<OperationResultDTO> CreateUser(UserCreateDTO dto)
    {
        var result = await _repository.CreateUser(dto);
        return result;
    }
    #endregion

    #region UPDATE USER
    public async Task<OperationResultDTO> UpdateUser(UserUpdateDTO dto)
    {
        var result = await _repository.UpdateUser(dto);
        if(result != null) throw new ApiException("User not found", 404);
        return result;
    }
    #endregion

    #region DEACTIVATE USER
    public async Task<OperationResultDTO> DeactivateUser(int userID)
    {
        var result = await _repository.DeactivateUser(userID);
        return result == null ? throw new ApiException("User not found", 404) : result;
    }
    #endregion

    #region CREATE AND MODIFIED USER
    public async Task<CreatedAndModifiedDTO> CreatedAndModifiedBy(int CreatedByID, int ModifiedByID)
    {
        var result = await _repository.CreatedAndModifiedBy(CreatedByID, ModifiedByID);
        return result;
    }
    #endregion
}
