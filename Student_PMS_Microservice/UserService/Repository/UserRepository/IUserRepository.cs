using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using UserService.DTOs;

namespace UserService.Repository.UserRepository;

public interface IUserRepository
{
    Task<ListResult<UserListDTO>>   GetUsersPage();
    Task<UserViewDTO>               GetUserView(int userID);
    Task<UserUpdateDTO>             GetUserPK(int userID);
    Task<OperationResultDTO>        CreateUser(UserCreateDTO dto);
    Task<OperationResultDTO>        UpdateUser(UserUpdateDTO dto);
    Task<OperationResultDTO>        DeactivateUser(int userID);
    Task<CreatedAndModifiedDTO>     CreatedAndModifiedBy(int CreatedByID, int ModifiedByID);
}
