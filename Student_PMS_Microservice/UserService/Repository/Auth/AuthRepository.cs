using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;

namespace UserService.Repository.Auth;

public class AuthRepository(
    AppDbContext context
) : IAuthRepository
{
    private readonly AppDbContext _context = context;

    public async Task<LoginDTO?> Login(string email)
    {
        return await _context.User
            .Where(x => x.Email == email && x.IsActive)
            .Select(x => new LoginDTO
            {
                Email = x.Email,
                Password = x.Password
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserInfoDTO?> UserInfo(string email)
    {
        return await _context.User
            .Where(u => u.Email == email && u.IsActive)
            .Select(u => new UserInfoDTO
            {
                UserID = u.UserID,
                UserName = u.Name,
                Email = u.Email,
                RoleName = u.Role != null ? u.Role.RoleName : string.Empty
            })
            .FirstOrDefaultAsync();
    }
}
