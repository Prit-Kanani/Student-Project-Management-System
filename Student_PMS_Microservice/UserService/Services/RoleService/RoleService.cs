using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using ProjectGroup.Services.RoleService;
using UserService.DTOs;
using UserService.Repository.RoleRepository;

namespace UserService.Services.RoleService;

public class RoleService(
    IRoleRepository repository
) : IRoleService
{
    public async Task<ListResult<RoleListDTO>> GetRolesPage()
    {
        return await repository.GetRolesPage();
    }

    public async Task<RoleViewDTO> GetRoleView(int userID)
    {
        return await repository.GetRoleView(userID);
    }

    public async Task<RoleUpdateDTO> GetRolePK(int roleID)
    {
        return await repository.GetRolePK(roleID);
    }

    public async Task<OperationResultDTO> CreateRole(RoleCreateDTO dto)
    {
        return await repository.CreateRole(dto);
    }

    public async Task<OperationResultDTO> UpdateRole(RoleUpdateDTO dto)
    {
        return await repository.UpdateRole(dto);
    }

    public async Task<OperationResultDTO> DeactivateRole(int roleID)
    {
        return await repository.DeactivateRole(roleID)
            ?? throw new NotFoundException("Role not found");
    }

    public async Task<List<OptionDTO>> GetRoleDropdown()
    {
        return await repository.GetRoleDropdown();
    }
}
