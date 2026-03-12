using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Repository.RoleRepository;

public class RoleRepository(
    AppDbContext context
) : IRoleRepository
{
    private readonly AppDbContext _context = context;

    public async Task<ListResult<RoleListDTO>> GetRolesPage()
    {
        var roles = await _context.Role.AsNoTracking().ToListAsync();
        return ReflectionMapper.Map<ListResult<RoleListDTO>>(roles);
    }

    public async Task<RoleUpdateDTO> GetRolePK(int roleID)
    {
        var role = await _context.Role
            .Where(r => r.RoleID == roleID)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Role not found");

        return ReflectionMapper.Map<RoleUpdateDTO>(role);
    }

    public async Task<RoleViewDTO> GetRoleView(int roleID)
    {
        var role = await _context.Role
            .Where(r => r.RoleID == roleID)
            .Include(r => r.CreatedBy)
            .Select(r => new
            {
                r.RoleName,
                r.Description,
                CreatedBy = r.CreatedBy != null ? r.CreatedBy.Name : string.Empty,
                ModifiedBy = r.ModifiedBy != null ? r.ModifiedBy.Name : string.Empty,
                r.IsActive,
                r.Created,
                r.Modified
            })
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Role not found");

        return ReflectionMapper.Map<RoleViewDTO>(role);
    }

    public async Task<OperationResultDTO> CreateRole(RoleCreateDTO dto)
    {
        var role = dto.Adapt<Role>();
        role.Created = DateTime.UtcNow;

        _context.Role.Add(role);
        var rows = await _context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> DeactivateRole(int roleID)
    {
        var role = await _context.Role.FindAsync(roleID)
            ?? throw new NotFoundException("Role not found");

        role.IsActive = false;
        role.Modified = DateTime.UtcNow;

        var rows = await _context.SaveChangesAsync();
        return new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> UpdateRole(RoleUpdateDTO dto)
    {
        var role = await _context.Role.FindAsync(dto.RoleID)
            ?? throw new NotFoundException("Role not found");

        dto.Adapt(role);
        role.ModifiedByID = 1;
        role.Modified = DateTime.UtcNow;

        var rows = await _context.SaveChangesAsync();
        return new OperationResultDTO
        {
            Id = role.RoleID,
            RowsAffected = rows
        };
    }

    public async Task<List<OptionDTO>> GetRoleDropdown()
    {
        var roles = await _context.Role.Where(r => r.IsActive).ToListAsync();
        return ReflectionMapper.Map<List<OptionDTO>>(roles);
    }
}
