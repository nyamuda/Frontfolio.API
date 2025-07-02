namespace Frontfolio.API.Services.Abstractions
{
    public interface IProjectService
    {
        Task<ProjectDto> Get(int id,int tokenUserId);

        Task<List<ProjectDto>> Get(int page, int pageSize, int userId, ProjectSortOption? sortOption);

        Task<ProjectDto> Create(AddProjectDto projectDto);

        Task Delete(int id,int tokenUserId);

        Task Update(int id,int tokenUserId, UpdateProjectDto projectDto);



    }
}
