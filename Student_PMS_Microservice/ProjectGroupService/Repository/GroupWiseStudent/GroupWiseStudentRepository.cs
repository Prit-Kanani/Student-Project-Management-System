using Comman.DTOs.CommanDTOs;
using Comman.Exceptions;
using Comman.Functions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.DTOs;
using ProjectGroupServices.Data;

namespace ProjectGroupService.Repository.GroupWiseStudent;

public class GroupWiseStudentRepository(
    AppDbContext context
) : IGroupWiseStudentRepository
{
    public async Task<ListResult<GroupWiseStudentListDTO>> GetGroupWiseStudentsPage(int skip, int take)
    {
        var groupWiseStudent = await context.GroupWiseStudent.AsNoTracking().Skip(skip).Take(take).ToListAsync();
        return ReflectionMapper.Map<ListResult<GroupWiseStudentListDTO>>(groupWiseStudent);
    }

    public async Task<GroupWiseStudentViewDTO> GetGroupWiseStudentView(int groupWiseStudentID)
    {
        var groupWiseStudent = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
            ?? throw new NotFoundException("Student group not found");

        return ReflectionMapper.Map<GroupWiseStudentViewDTO>(groupWiseStudent);
    }

    public async Task<GroupWiseStudentUpdateDTO> GetGroupWiseStudentPK(int groupWiseStudentID)
    {
        var groupWiseStudent = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
            ?? throw new NotFoundException("Student group not found");

        return ReflectionMapper.Map<GroupWiseStudentUpdateDTO>(groupWiseStudent);
    }

    public async Task<OperationResultDTO> CreateGroupWiseStudent(GroupWiseStudentCreateDTO dto)
    {
        var groupWiseStudent = dto.Adapt<Models.GroupWiseStudent>();
        groupWiseStudent.Created = DateTime.UtcNow;
        await context.GroupWiseStudent.AddAsync(groupWiseStudent);
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = groupWiseStudent.StudentWiseGroupID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> UpdateGroupWiseStudent(GroupWiseStudentUpdateDTO dto)
    {
        var groupWiseStudent = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == dto.StudentWiseGroupID)
            ?? throw new NotFoundException("Student group not found");

        dto.Adapt(groupWiseStudent);
        groupWiseStudent.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = groupWiseStudent.StudentWiseGroupID,
            RowsAffected = rows
        };
    }

    public async Task<OperationResultDTO> DeactivateGroupWiseStudent(int groupWiseStudentID)
    {
        var projectGroup = await context.GroupWiseStudent.FirstOrDefaultAsync(p => p.StudentWiseGroupID == groupWiseStudentID)
            ?? throw new NotFoundException("Student group not found");

        projectGroup.IsActive = false;
        projectGroup.Modified = DateTime.UtcNow;
        var rows = await context.SaveChangesAsync();

        return new OperationResultDTO
        {
            Id = projectGroup.StudentWiseGroupID,
            RowsAffected = rows
        };
    }
}
