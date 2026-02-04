using Comman.Functions;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;

namespace UserService.Repository.Auth;

public class AuthRepository(
    AppDbContext Context,
    IConfiguration configuration
) : IAuthRepository
{
    #region CONFIGURATION
    private readonly AppDbContext _context = Context;
    #endregion CONFIGURATION

    #region LOGIN
    public async Task<LoginDTO> Login(string Email)
    {
        var login = await _context.User.FirstOrDefaultAsync(x => x.Email == Email && x.IsActive == true);
        var response = ReflectionMapper.Map<LoginDTO>(login);
        return response;
    }
    #endregion

    #region GET USER INFO USING EMAIL
    public async Task<UserInfoDTO> UserInfo(string Email)
    {
        var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == Email && u.IsActive == true);
        var response = ReflectionMapper.Map<UserInfoDTO>(user);
        return response;
    }
    #endregion
}
