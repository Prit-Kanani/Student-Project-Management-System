using Comman.DTOs.CommanDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services.UserProfile;

namespace UserService.Controllers;

[Route("api/UserService/[controller]")]
[ApiController]
[Authorize]
public class UserProfileController(
    IUserProfileService userProfileService,
    IValidator<UserProfileCreateDTO> createValidator,
    IValidator<UserProfileUpdateDTO> updateValidator
) : ControllerBase
{
    [HttpGet("Page")]
    [Produces<ListResult<UserProfileListDTO>>]
    public async Task<IActionResult> GetUserProfiles()
    {
        return Ok(await userProfileService.GetUserProfilePage());
    }

    [HttpGet("View/{userID:int}")]
    [Produces<UserProfileViewDTO>]
    public async Task<IActionResult> GetUserProfileView(int userID)
    {
        return Ok(await userProfileService.GetUserProfileView(userID));
    }

    [HttpGet("Pk/{userID:int}")]
    [Produces<UserProfileUpdateDTO>]
    public async Task<IActionResult> GetUserProfilePK(int userID)
    {
        return Ok(await userProfileService.GetUserProfilePK(userID));
    }

    [HttpPost("Insert")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateDTO dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);
        return Ok(await userProfileService.CreateUserProfile(dto));
    }

    [HttpPut("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateDTO dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);
        return Ok(await userProfileService.UpdateUserProfile(dto));
    }
}
