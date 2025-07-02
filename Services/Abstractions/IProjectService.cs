namespace Frontfolio.API.Services.Abstractions
{
    public interface IProjectService
    {
        Task<ProjectDto> Get(int id);

        Task<List<ProjectDto>> Get();

        Task<ProjectDto> Create(AddProjectDto projectDto);

        Task Delete(int id);

        Task Update(int id, UpdateProjectDto projectDto);



    }
}
