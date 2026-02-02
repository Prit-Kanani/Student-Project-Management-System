using Comman.Functions;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;

namespace UserService.Repository.Auth;

public class AuthRepository(
    AppDbContext Context    
) : IAuthRepository
{
    #region CONFIGURATION
    private readonly AppDbContext _context = Context;
    #endregion CONFIGURATION

    #region Login
    public async Task<LoginDTO> Login(string Email)
    {
        var login = await _context.User.FirstOrDefaultAsync(x => x.Email == Email);
        var response = ReflectionMapper.Map<LoginDTO>(login);
        return response;
    }
    #endregion

    #region Get User Info Using Email
    public async Task<UserInfoDTO> UserInfo(string Email)
    {
        var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == Email);
        var response = ReflectionMapper.Map<UserInfoDTO>(user);
        return response;
    }
}
