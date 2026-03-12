using Comman.DTOs.CommanDTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectGroupService.DTOs;
using ProjectGroupService.Services.GroupWiseStudent;

namespace ProjectGroupService.Controllers;

[Route("api/ProjectGroupService/[controller]")]
[ApiController]
public class GroupWiseStudentController(
    IGroupWiseStudentService groupWiseStudentService
) : ControllerBase
{
    [HttpGet]
    [Route("Page")]
    [Produces<ListResult<GroupWiseStudentListDTO>>]
    public async Task<IActionResult> GetGroupWiseStudentsPage(int skip = 0, int take = 10)
    {
        var response = await groupWiseStudentService.GetGroupWiseStudentsPage(skip, take);
        return Ok(response);
    }

    [HttpGet]
    [Route("{groupWiseStudentId:long}")]
    [Produces<GroupWiseStudentUpdateDTO>]
    public async Task<IActionResult> GetGroupWiseStudentByPK(int groupWiseStudentId)
    {
        var response = await groupWiseStudentService.GetGroupWiseStudentPK(groupWiseStudentId);
        return Ok(response);
    }

    [HttpGet]
    [Route("View/{groupWiseStudentId:int}")]
    [Produces<GroupWiseStudentViewDTO>]
    public async Task<IActionResult> GetGroupWiseStudentView(int groupWiseStudentId)
    {
        var response = await groupWiseStudentService.GetGroupWiseStudentView(groupWiseStudentId);
        return Ok(response);
    }

    [HttpPost]
    [Route("Create")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> CreateGroupWiseStudent([FromBody] GroupWiseStudentCreateDTO dto)
    {
        var response = await groupWiseStudentService.CreateGroupWiseStudent(dto);
        return Ok(response);
    }

    [HttpPost]
    [Route("Update")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> UpdateGroupWiseStudent([FromBody] GroupWiseStudentUpdateDTO dto)
    {
        var response = await groupWiseStudentService.UpdateGroupWiseStudent(dto);
        return Ok(response);
    }

    [HttpDelete]
    [Route("Deactivate/{groupWiseStudentId:int}")]
    [Produces<OperationResultDTO>]
    public async Task<IActionResult> DeactivateGroupWiseStudent(int groupWiseStudentId)
    {
        var response = await groupWiseStudentService.DeactivateGroupWiseStudent(groupWiseStudentId);
        return Ok(response);
    }
}
