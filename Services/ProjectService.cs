
using Frontfolio.API.Data;
using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

public class ProjectService
{

    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    //Get a project with a given ID
    public async Task<ProjectDto> GetProjectById(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (project is null) throw new KeyNotFoundException($"Project with ID {id} doest not exist");

        return ProjectDto.MapFrom(project);
    }

    p




}

