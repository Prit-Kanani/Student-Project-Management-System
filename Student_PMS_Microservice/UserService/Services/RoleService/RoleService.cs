using Comman.DTOs.CommanDTOs;
using ProjectGroup.DTOs.RoleDTO;
using ProjectGroup.Exceptions;
using ProjectGroup.Repository.RoleRepository;

namespace ProjectGroup.Services.RoleService;

public class RoleService : IRoleService
{
    #region CONFIGURATION
    private readonly IRoleRepository _repository;
    //private readonly MicroserviceGateway _gateway;

    public RoleService(IRoleRepository repository /*, MicroserviceGateway gateway*/)
    {
        _repository = repository;
        //_gatway = gatway;
    }
    #endregion

    #region GET ROLES PAGE
    public async Task<ListResult<RoleListDTO>> GetRolesPage()
    {
        var result = await _repository.GetRolesPage();
        return result;
    }
    #endregion

    #region GET ROLE VIEW
    public async Task<RoleViewDTO> GetRoleView(int userID)
    {
        var result = await _repository.GetRoleView(userID);
        return result;
    }
    #endregion

    #region GET ROLE PK
    public async Task<RoleUpdateDTO> GetRolePK(int roleID)
    {
        var result = await _repository.GetRolePK(roleID);
        return result;
    }
    #endregion

    #region CREATE ROLE
    public async Task<OperationResultDTO> CreateRole(RoleCreateDTO dto)
    {
        var result = await _repository.CreateRole(dto);
        return result;
    }
    #endregion

    #region UPDATE ROLE
    public async Task<OperationResultDTO> UpdateRole(RoleUpdateDTO dto)
    {
        var response = await _repository.UpdateRole(dto);
        if(response != null) throw new ApiException("Role not found", 404);
        return response;
    }
    #endregion

    #region DEACTIVATE ROLE
    public async Task<OperationResultDTO> DeactivateRole(int roleID)
    {
        var response = await _repository.DeactivateRole(roleID);
        return response ?? throw new ApiException("Role not found", 404);
    }
    #endregion

    #region GET ROLE DROPDOWN
    public async Task<List<OptionDTO>> GetRoleDropdown()
    {
        var result = await _repository.GetRoleDropdown();
        return result;
    }
    #endregion
}
