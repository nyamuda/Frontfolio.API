namespace Frontfolio.API.Services.Abstractions
{
    public interface IProjectService
    {
        Task<ProjectDto> GetAsync(int projectId, int tokenUserId);

        Task<PageInfo<ProjectDto>> GetAllAsync(int page, int pageSize, int userId, ProjectSortOption sortOption, ProjectFilterOption filterOption);

        Task<ProjectDto> CreateAsync(int userId, AddProjectDto projectDto);

        Task UpdateAsync(int projectId, int tokenUserId, UpdateProjectDto updateProjectDto);

        Task DeleteAsync(int projectId, int tokenUserId);





    }
}
