using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Repository.GroupWiseStudent;

public interface IGroupWiseStudentRepository
{
    Task<ListResult<GroupWiseStudentListDTO>>   GetGroupWiseStudentsPage(int skip, int take);
    Task<GroupWiseStudentViewDTO>               GetGroupWiseStudentView(int groupWiseStudentID);
    Task<GroupWiseStudentUpdateDTO>             GetGroupWiseStudentPK(int groupWiseStudentID);
    Task<OperationResultDTO>                    CreateGroupWiseStudent(GroupWiseStudentCreateDTO dto);
    Task<OperationResultDTO>                    UpdateGroupWiseStudent(GroupWiseStudentUpdateDTO dto);
    Task<OperationResultDTO>                    DeactivateGroupWiseStudent(int groupWiseStudentID);
}
