using Comman.DTOs.CommanDTOs;
using ProjectGroup.DTOs.RoleDTO;

namespace ProjectGroup.Repository.RoleRepository;

public interface IRoleRepository
{
    Task<ListResult<RoleListDTO>>   GetRolesPage();
    Task<RoleViewDTO>               GetRoleView(int roleID);
    Task<RoleUpdateDTO>             GetRolePK(int roleID);
    Task<OperationResultDTO>        CreateRole(RoleCreateDTO dto);
    Task<OperationResultDTO>        UpdateRole(RoleUpdateDTO dto);
    Task<OperationResultDTO>        DeactivateRole(int roleID);
    Task<List<OptionDTO>>           GetRoleDropdown();
}
