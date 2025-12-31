using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.GroupWiseStudent;

namespace ProjectGroupService.Services.GroupWiseStudent;

public class GroupWiseStudentService(
        IGroupWiseStudentRepository repository
)   : IGroupWiseStudentService
{
    #region GET GROUP WISE STUDENT PAGE
    public async Task<ListResult<GroupWiseStudentListDTO>> GetGroupWiseStudentsPage(int skip,int take)
    {
        var response = await repository.GetGroupWiseStudentsPage(skip, take);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    public async Task<GroupWiseStudentViewDTO> GetGroupWiseStudentView(int groupWiseStudentID)
    {
        var response = await repository.GetGroupWiseStudentView(groupWiseStudentID);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    public async Task<GroupWiseStudentUpdateDTO> GetGroupWiseStudentPK(int groupWiseStudentID)
    {
        var response = await repository.GetGroupWiseStudentPK(groupWiseStudentID);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> CreateGroupWiseStudent(GroupWiseStudentCreateDTO dto)
    {
        var response = await repository.CreateGroupWiseStudent(dto);
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> UpdateGroupWiseStudent(GroupWiseStudentUpdateDTO dto)
    {
        var response = await repository.UpdateGroupWiseStudent(dto);
        return response;
    }
    #endregion

    #region DEACTIVATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> DeactivateGroupWiseStudent(int groupWiseStudentID)
    {
        var response = await repository.DeactivateGroupWiseStudent(groupWiseStudentID);
        return response;
    }
    #endregion
}
