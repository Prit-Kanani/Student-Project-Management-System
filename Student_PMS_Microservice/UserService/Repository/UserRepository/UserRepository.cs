using Mapster;
using Comman.Functions;
using UserService.Data;
using UserService.Models;
using Comman.DTOs.CommanDTOs;
using Comman.MicroserviceDTO;
using UserService.DTOs.UserDTO;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repository.UserRepository;

public class UserRepository(
    AppDbContext context
) : IUserRepository
{

    #region CONFIGURATION
    private readonly HashPass hashPass = new();
    #endregion

    #region GET USERS PAGE
    public async Task<ListResult<UserListDTO>> GetUsersPage()
    {
        var users = await context.User.AsNoTracking().ToListAsync();
        var result = ReflectionMapper.Map<ListResult<UserListDTO>>(users);
        return result;
    }
    #endregion

    #region GET USERS VIEW
    public async Task<UserViewDTO> GetUserView(int userID)
    {
        var user = await context.User
            .Where(u => u.UserID == userID)
            .Select(u => new
            {
                u.Name,
                u.Email,
                u.Phone,
                u.IsActive,
                u.Created,
                u.Modified,
                u.Role.RoleName,
                CreatedBy = u.CreatedBy.Name,
                ModifiedBy = u.ModifiedBy.Name,
            })
            .FirstOrDefaultAsync();
        var result = ReflectionMapper.Map<UserViewDTO>(user);
        return result;
    }
    #endregion

    #region GET USERS PK
    public async Task<UserUpdateDTO> GetUserPK(int UserID)
    {
        var user = await context.User.FirstOrDefaultAsync(d => d.UserID == UserID)
                                        ?? throw new Exception("User does not exist!");
        var result = ReflectionMapper.Map<UserUpdateDTO>(user);
        return result;
    }
    #endregion

    #region CREATE USER
    public async Task<OperationResultDTO> CreateUser(UserCreateDTO dto)
    {
        var user = dto.Adapt<User>();
        user.Created = DateTime.UtcNow;
        user.Password = hashPass.HashPassword(dto.Password);
        await context.User.AddAsync(user);
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = user.UserID , RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE USER
    public async Task<OperationResultDTO> UpdateUser(UserUpdateDTO dto)
    {
        var user = await context.User.FirstOrDefaultAsync(d => d.UserID == dto.UserID)
                                        ?? throw new Exception("User does not exist!");
        dto.Adapt(user);
        user.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = user.UserID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region DEACTIVATE USER
    public async Task<OperationResultDTO> DeactivateUser(int UserID)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.UserID == UserID) ?? throw new Exception("User does not exist!");
        user.IsActive = false;
        user.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO { Id = user.UserID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region CREATED AND MODIFIED BY
    public async Task<CreatedAndModifiedDTO> CreatedAndModifiedBy(int CreatedByID, int ModifiedByID)
    {
        string CreatedBy = await context.User
            .Where(u => u.UserID == CreatedByID)
            .Select(u => u.Name )
            .FirstOrDefaultAsync() ?? throw new Exception("CreatedBy user does not exist!");

        string ModifiedBy = await context.User
            .Where(u => u.UserID == ModifiedByID)
            .Select(u =>u.Name )
            .FirstOrDefaultAsync() ?? "—";

        var response = ReflectionMapper.Map<CreatedAndModifiedDTO>(new { CreatedBy, ModifiedBy });

        return response;
    }
    #endregion
}