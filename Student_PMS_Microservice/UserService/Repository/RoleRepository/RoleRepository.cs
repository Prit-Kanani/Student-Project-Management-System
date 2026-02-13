using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Models;

namespace UserService.Repository.RoleRepository;

public class RoleRepository(
    AppDbContext context
) : IRoleRepository
{
    #region CONFIGURATION
    private readonly AppDbContext _context = context;
    #endregion

    #region GET ROLE PAGE
    public async Task<ListResult<RoleListDTO>> GetRolesPage()
    {
        var roles = await _context.Role.AsNoTracking().ToListAsync();
        var result = ReflectionMapper.Map<ListResult<RoleListDTO>>(roles);
        return result;
    }
    #endregion

    #region GET ROLE PK
    public async Task<RoleUpdateDTO> GetRolePK(int roleID)
    {
        var role = await _context.Role
            .Where(r => r.RoleID == roleID)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Role not found");

        var result = ReflectionMapper.Map<RoleUpdateDTO>(role);

        return result;
    }
    #endregion

    #region GET ROLE VIEW
    public async Task<RoleViewDTO> GetRoleView(int roleID)
    {
        var role = await _context.Role
            .Where(r => r.RoleID == roleID)
            .Include(r => r.CreatedBy)
            .Select(r => new
            {
                r.RoleName,
                r.Description,
                CreatedBy = r.CreatedBy.Name,
                ModifiedBy = r.ModifiedBy.Name ?? "—",
                r.IsActive,
                r.Created,
                r.Modified
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Role not found");
        ;

        var result = ReflectionMapper.Map<RoleViewDTO>(role);

        return result;
    }
    #endregion

    #region CREATE ROLE
    public async Task<OperationResultDTO> CreateRole(RoleCreateDTO dto)
    {
        var role = dto.Adapt<Role>();

        role.Created = DateTime.UtcNow;

        _context.Role.Add(role);
        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
        return response;
    }
    #endregion

    #region DEACTIVATE ROLE
    public async Task<OperationResultDTO> DeactivateRole(int roleID)
    {
        var role = await _context.Role.FindAsync(roleID) ?? throw new NotFoundException("Role not found");
        role.IsActive = false;
        role.Modified = DateTime.UtcNow;

        var rows = await _context.SaveChangesAsync();
        var response = new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
        return response;
    }
    #endregion

    #region UPDATE ROLE
    public async Task<OperationResultDTO> UpdateRole(RoleUpdateDTO dto)
    {
        var role = await _context.Role.FindAsync(dto.RoleID) ?? throw new NotFoundException("Role not found");
        dto.Adapt(role);

        role.ModifiedByID = 1;
        role.Modified = DateTime.UtcNow;

        var rows = await _context.SaveChangesAsync();

        var response = new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
        return response;
    }
    #endregion

    #region GET ROLE DROPDOWN
    public async Task<List<OptionDTO>> GetRoleDropdown()
    {
        var roles = await _context.Role.Where(r => r.IsActive).ToListAsync();
        var response = ReflectionMapper.Map<List<OptionDTO>>(roles);
        return response;
    }
    #endregion
}
