using Comman.Functions;
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
        var login = await _context.User.FirstOrDefaultAsync(x => x.Email == email && x.IsActive);
        return login is null ? null : ReflectionMapper.Map<LoginDTO>(login);
    }

    public async Task<UserInfoDTO?> UserInfo(string email)
    {
        var user = await _context.User
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        return user is null ? null : ReflectionMapper.Map<UserInfoDTO>(user);
    }
}
