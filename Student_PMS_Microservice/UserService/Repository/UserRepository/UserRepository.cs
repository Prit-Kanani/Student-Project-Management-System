using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Comman.MicroserviceDTO;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Repository.UserRepository;

public class UserRepository(
    AppDbContext context
) : IUserRepository
{
    public async Task<ListResult<UserListDTO>> GetUsersPage()
    {
        var users = await context.User.AsNoTracking().ToListAsync();
        return ReflectionMapper.Map<ListResult<UserListDTO>>(users);
    }

    public async Task<UserViewDTO> GetUserView(int userID)
    {
        var user = await context.User
            .Where(u => u.UserID == userID)
            .Select(u => new
            {
                u.Name,
                u.Email,
                u.IsActive,
                u.Created,
                u.Modified,
                RoleName = u.Role != null ? u.Role.RoleName : null,
                CreatedBy = u.CreatedBy != null ? u.CreatedBy.Name : null,
                ModifiedBy = u.ModifiedBy != null ? u.ModifiedBy.Name : null,
            })
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("User does not exist!");

        return ReflectionMapper.Map<UserViewDTO>(user);
    }

    public async Task<UserUpdateDTO> GetUserPK(int userID)
    {
        var user = await context.User.FirstOrDefaultAsync(d => d.UserID == userID)
            ?? throw new NotFoundException("User does not exist!");

        return ReflectionMapper.Map<UserUpdateDTO>(user);
    }

    public async Task<OperationResultDTO> CreateUser(UserCreateDTO dto)
    {
        var emailExists = await context.User.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            throw new BadRequestException("A user with this email already exists");
        }

        if (dto.RoleID.HasValue)
        {
            var roleExists = await context.Role.AnyAsync(r => r.RoleID == dto.RoleID.Value);
            if (!roleExists)
            {
                throw new BadRequestException("Selected role does not exist");
            }
        }

        var user = dto.Adapt<User>();
        user.Created = DateTime.UtcNow;
        user.Password = HashPass.HashPassword(dto.Password);
        await context.User.AddAsync(user);
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = user.UserID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> UpdateUser(UserUpdateDTO dto)
    {
        var user = await context.User.FirstOrDefaultAsync(d => d.UserID == dto.UserID)
            ?? throw new NotFoundException("User does not exist!");

        dto.Adapt(user);
        user.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = user.UserID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> DeactivateUser(int userID)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.UserID == userID)
            ?? throw new NotFoundException("User does not exist!");

        user.IsActive = false;
        user.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = user.UserID,
            RowsAffected = rows
        };
    }

    public async Task<CreatedAndModifiedDTO> CreatedAndModifiedBy(int createdByID, int modifiedByID)
    {
        string createdBy = await context.User
            .Where(u => u.UserID == createdByID)
            .Select(u => u.Name)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("CreatedBy user does not exist!");

        string modifiedBy = await context.User
            .Where(u => u.UserID == modifiedByID)
            .Select(u => u.Name)
            .FirstOrDefaultAsync() ?? "—";

        return ReflectionMapper.Map<CreatedAndModifiedDTO>(new { CreatedBy = createdBy, ModifiedBy = modifiedBy });
    }

    public async Task<AuditUsersDTO> ResolveAuditUsers(int createdByID, int? modifiedByID, int? approvedByID)
    {
        var createdBy = await context.User
            .Where(u => u.UserID == createdByID)
            .Select(u => u.Name)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("CreatedBy user does not exist!");

        var modifiedBy = modifiedByID.HasValue
            ? await context.User.Where(u => u.UserID == modifiedByID.Value).Select(u => u.Name).FirstOrDefaultAsync() ?? "—"
            : "—";

        var approvedBy = approvedByID.HasValue
            ? await context.User.Where(u => u.UserID == approvedByID.Value).Select(u => u.Name).FirstOrDefaultAsync() ?? "—"
            : "—";

        return new AuditUsersDTO
        {
            CreatedBy = createdBy,
            ModifiedBy = modifiedBy,
            ApprovedBy = approvedBy
        };
    }
}
