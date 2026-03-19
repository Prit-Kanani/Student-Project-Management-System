using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.GroupWiseStudent;
using ProjectGroupService.Services.External;

namespace ProjectGroupService.Services.GroupWiseStudent;

public class GroupWiseStudentService(
    IGroupWiseStudentRepository repository,
    UserServiceClient userServiceClient,
    ILogger<GroupWiseStudentService> logger
) : IGroupWiseStudentService
{
    public async Task<ListResult<GroupWiseStudentListDTO>> GetGroupWiseStudentsPage(int skip, int take)
    {
        return await repository.GetGroupWiseStudentsPage(skip, take);
    }

    public async Task<GroupWiseStudentViewDTO> GetGroupWiseStudentView(int groupWiseStudentID)
    {
        var response = await repository.GetGroupWiseStudentView(groupWiseStudentID);

        try
        {
            var auditUsers = await userServiceClient.ResolveAuditUsers(
                response.CreatedByID,
                response.ModifiedByID,
                null
            );

            response.CreatedBy = auditUsers.CreatedBy;
            response.ModifiedBy = auditUsers.ModifiedBy;
        }
        catch (Exception ex) when (
            ex is ApiException ||
            ex is HttpRequestException ||
            ex is TaskCanceledException
        )
        {
            logger.LogWarning(
                ex,
                "Audit user resolution failed for group-wise student {GroupWiseStudentID}. Returning the core payload without audit names.",
                groupWiseStudentID
            );
        }

        return response;
    }

    public async Task<GroupWiseStudentUpdateDTO> GetGroupWiseStudentPK(int groupWiseStudentID)
    {
        return await repository.GetGroupWiseStudentPK(groupWiseStudentID);
    }

    public async Task<OperationResultDTO> CreateGroupWiseStudent(GroupWiseStudentCreateDTO dto)
    {
        return await repository.CreateGroupWiseStudent(dto);
    }

    public async Task<OperationResultDTO> UpdateGroupWiseStudent(GroupWiseStudentUpdateDTO dto)
    {
        return await repository.UpdateGroupWiseStudent(dto);
    }

    public async Task<OperationResultDTO> DeactivateGroupWiseStudent(int groupWiseStudentID)
    {
        return await repository.DeactivateGroupWiseStudent(groupWiseStudentID);
    }
}
