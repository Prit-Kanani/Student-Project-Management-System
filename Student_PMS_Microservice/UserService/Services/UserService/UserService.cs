using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.MicroserviceDTO;
using ProjectGroup.Services.UserService;
using UserService.DTOs;
using UserService.Repository.UserRepository;

namespace UserService.Services.UserService;

public class UsersService(
    IUserRepository repository
) : IUserService
{
    private readonly IUserRepository _repository = repository;

    public async Task<ListResult<UserListDTO>> GetUsersPage()
    {
        return await _repository.GetUsersPage();
    }

    public async Task<UserViewDTO> GetUserView(int userID)
    {
        return await _repository.GetUserView(userID);
    }

    public async Task<UserUpdateDTO> GetUserPK(int userID)
    {
        return await _repository.GetUserPK(userID);
    }

    public async Task<OperationResultDTO> CreateUser(UserCreateDTO dto)
    {
        return await _repository.CreateUser(dto);
    }

    public async Task<OperationResultDTO> UpdateUser(UserUpdateDTO dto)
    {
        return await _repository.UpdateUser(dto);
    }

    public async Task<OperationResultDTO> DeactivateUser(int userID)
    {
        var result = await _repository.DeactivateUser(userID);
        return result == null ? throw new NotFoundException("User not found") : result;
    }

    public async Task<CreatedAndModifiedDTO> CreatedAndModifiedBy(int createdByID, int modifiedByID)
    {
        return await _repository.CreatedAndModifiedBy(createdByID, modifiedByID);
    }

    public async Task<AuditUsersDTO> ResolveAuditUsers(int createdByID, int? modifiedByID, int? approvedByID)
    {
        return await _repository.ResolveAuditUsers(createdByID, modifiedByID, approvedByID);
    }
}
