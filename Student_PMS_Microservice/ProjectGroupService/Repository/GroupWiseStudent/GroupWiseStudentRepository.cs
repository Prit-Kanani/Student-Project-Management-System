using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.DTOs;
using ProjectGroupService.Exceptions;
using ProjectGroupServices.Data;
namespace ProjectGroupService.Repository.GroupWiseStudent;

public class GroupWiseStudentRepository(
    AppDbContext context
) : IGroupWiseStudentRepository
{
    #region GET GROUP WISE STUDENT PAGE
    public async Task<ListResult<GroupWiseStudentListDTO>> GetGroupWiseStudentsPage(int skip, int take)
    {
        var groupWiseStudent = await context.GroupWiseStudent.AsNoTracking().Skip(skip).Take(take).ToListAsync();
        var response = ReflectionMapper.Map<ListResult<GroupWiseStudentListDTO>>(groupWiseStudent);
        return response;
    }
    #endregion

    #region GET GROUP WISE STUDENT VIEW
    public async Task<GroupWiseStudentViewDTO> GetGroupWiseStudentView(int groupWiseStudentID)
    {
        var groupWiseStudent = context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
                                        ?? throw new NotFoundException("Studnet Group not found");
        var response = ReflectionMapper.Map<GroupWiseStudentViewDTO>(groupWiseStudent);
        return response;
    }
    #endregion

    #region GET GROUP WISE STUDENT PK
    public async Task<GroupWiseStudentUpdateDTO> GetGroupWiseStudentPK(int groupWiseStudentID)
    {
        var groupWiseStudent = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
                                            ?? throw new NotFoundException("Studnet Group not found");
        var response = ReflectionMapper.Map<GroupWiseStudentUpdateDTO>(groupWiseStudent);
        return response;
    }
    #endregion

    #region CREATE GROUP WISE STUDENT
    public async Task<OperationResultDTO> CreateGroupWiseStudent(GroupWiseStudentCreateDTO dto)
    {
        var groupWiseStudent = dto.Adapt<Models.GroupWiseStudent>();
        groupWiseStudent.Created = DateTime.UtcNow;
        await context.GroupWiseStudent.AddAsync(groupWiseStudent);
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = groupWiseStudent.StudentWiseGroupID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region UPDATE GROUP WISE STUDENT
    public async Task<OperationResultDTO> UpdateGroupWiseStudent(GroupWiseStudentUpdateDTO dto)
    {
        var groupWiseStudent = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == dto.StudentWiseGroupID)
                                              ?? throw new NotFoundException("Studnet Group not found");
        dto.Adapt(groupWiseStudent);
        groupWiseStudent.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO{ Id = groupWiseStudent.StudentWiseGroupID, RowsAffected = rows };
        return response;
    }
    #endregion

    #region DEACTIVATE GROUP WISE STUDENT
    public async Task<OperationResultDTO> DeactivateGroupWiseStudent(int groupWiseStudentID)
    {
        var projectGroup = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
                                           ?? throw new NotFoundException("Studnet Group not found");
        projectGroup.IsActive = false;
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();
        var response = new OperationResultDTO { Id = projectGroup.ProjectGroupID, RowsAffected = rows };
        return response;
    }
    #endregion
}