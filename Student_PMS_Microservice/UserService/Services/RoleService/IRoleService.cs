using Comman.DTOs.CommanDTOs;
using UserService.DTOs.RoleDTO;

namespace UserService.Services.RoleService;

public interface IRoleService
{
    Task<ListResult<RoleListDTO>>   GetRolesPage();
    Task<RoleViewDTO>               GetRoleView(int roleID);
    Task<RoleUpdateDTO>             GetRolePK(int roleID);
    Task<OperationResultDTO>        CreateRole(RoleCreateDTO dto);
    Task<OperationResultDTO>        UpdateRole(RoleUpdateDTO dto);
    Task<OperationResultDTO>        DeactivateRole(int roleID);
    Task<List<OptionDTO>>           GetRoleDropdown();
}
