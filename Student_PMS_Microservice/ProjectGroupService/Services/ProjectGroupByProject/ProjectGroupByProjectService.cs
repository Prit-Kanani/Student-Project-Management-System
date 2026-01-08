using Comman.DTOs.CommanDTOs;
using ProjectGroupService.DTOs;
using ProjectGroupService.Repository.ProjectGroupByProject;

namespace ProjectGroupService.Services.ProjectGroupByProject;

public class ProjectGroupByProjectService(
        IProjectGroupByProjectRepository repository
)   : IProjectGroupByProjectService
{
    #region GET PROJECT GROUP BY PROJECT PAGE
    public async Task<ListResult<ProjectGroupByProjectListDTO>> GetProjectGroupByProjectsPage()
    {
        var response = await repository.GetProjectGroupByProjectsPage();
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT VIEW
    public async Task<ProjectGroupByProjectViewDTO> GetProjectGroupByProjectView(int projectGroupByProjectID)
    {
        var response = await repository.GetProjectGroupByProjectView(projectGroupByProjectID);
        return response;
    }
    #endregion

    #region GET PROJECT GROUP BY PROJECT PK
    public async Task<ProjectGroupByProjectUpdateDTO> GetProjectGroupByProjectPK(int projectGroupByProjectID)
    {
        var response = await repository.GetProjectGroupByProjectPK(projectGroupByProjectID);
        return response;
    }
    #endregion

    #region CREATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> CreateProjectGroupByProject(ProjectGroupByProjectCreateDTO dto)
    {
        var response = await repository.CreateProjectGroupByProject(dto);
        return response;
    }
    #endregion

    #region UPDATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> UpdateProjectGroupByProject(ProjectGroupByProjectUpdateDTO dto)
    {
        var response = await repository.UpdateProjectGroupByProject(dto);
        return response;
    }
    #endregion

    #region DEACTIVATE PROJECT GROUP BY PROJECT
    public async Task<OperationResultDTO> DeactivateProjectGroupByProject(int projectGroupByProjectID)
    {
        var response = await repository.DeactivateProjectGroupByProject(projectGroupByProjectID);
        return response;
    }
    #endregion

    #region BULK CREATE PROJECT GROUP BY PROJECT
    public async Task BulkInsertAsync(List<BulkProjectGroupByProjectCreateDTO> projectGroupByProjects)
    {
        await repository.BulkInsertAsync(projectGroupByProjects);
    }
    #endregion
}
