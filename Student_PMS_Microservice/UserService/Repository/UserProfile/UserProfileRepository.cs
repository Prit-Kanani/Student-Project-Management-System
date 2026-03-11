using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroup.Data;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Models;

namespace UserService.Repository.UserProfile;

public class UserProfileRepository(
    AppDbContext context
) : IUserProfileRepository
{
    public async Task<ListResult<UserProfileListDTO>> GetUserProfilePage()
    {
        var profiles = await context.UserProfile.AsNoTracking().ToListAsync();
        return ReflectionMapper.Map<ListResult<UserProfileListDTO>>(profiles);
    }

    public async Task<UserProfileViewDTO> GetUserProfileView(int userID)
    {
        var profile = await context.UserProfile
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserID == userID)
            ?? throw new NotFoundException("User profile not found");

        return ReflectionMapper.Map<UserProfileViewDTO>(profile);
    }

    public async Task<UserProfileUpdateDTO> GetUserProfilePK(int userID)
    {
        var profile = await context.UserProfile
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserID == userID)
            ?? throw new NotFoundException("User profile not found");

        return ReflectionMapper.Map<UserProfileUpdateDTO>(profile);
    }

    public async Task<OperationResultDTO> CreateUserProfile(UserProfileCreateDTO dto)
    {
        var userExists = await context.User.AnyAsync(x => x.UserID == dto.UserID);
        if (!userExists)
        {
            throw new BadRequestException("User does not exist for the supplied UserID");
        }

        var alreadyExists = await context.UserProfile.AnyAsync(x => x.UserID == dto.UserID);
        if (alreadyExists)
        {
            throw new BadRequestException("User profile already exists for the supplied UserID");
        }

        var profile = dto.Adapt<UserProfile>();
        await context.UserProfile.AddAsync(profile);
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = profile.UserProfileID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> UpdateUserProfile(UserProfileUpdateDTO dto)
    {
        var profile = await context.UserProfile.FirstOrDefaultAsync(x => x.UserProfileID == dto.UserProfileID)
            ?? throw new NotFoundException("User profile not found");

        var userExists = await context.User.AnyAsync(x => x.UserID == dto.UserID);
        if (!userExists)
        {
            throw new BadRequestException("User does not exist for the supplied UserID");
        }

        var duplicateUserLink = await context.UserProfile.AnyAsync(
            x => x.UserID == dto.UserID && x.UserProfileID != dto.UserProfileID);
        if (duplicateUserLink)
        {
            throw new BadRequestException("Another profile already exists for the supplied UserID");
        }

        dto.Adapt(profile);
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = profile.UserProfileID,
            RowsAffected = rows
        };
    }
}
