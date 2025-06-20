
using Frontfolio.API.Data;
using Frontfolio.API.Dtos.Auth;
using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

public class ProjectService
{

    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    //Get a project with a given ID for a 
    public async Task<ProjectDto> GetProjectById(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (project is null) throw new KeyNotFoundException($"Project with ID {id} doest not exist");

        return ProjectDto.MapFrom(project);
    }

    //Get all project for a User with a given ID
    public async Task<List<ProjectDto>> GetProject(int page, int pageSize,int UserId)
    {

        List<ProjectDto> projects = await _context.Projects
             .Where(p => p.UserId.Equals(UserId))
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .OrderByDescending(p => p.CreatedAt)
             .Select(p => new ProjectDto
             {
                 Id = p.Id,
                 Title = p.Title,
                 Summary = p.Summary,
                 Status = p.Status,
                 TechStack = p.TechStack,
                 ImageUrl = p.ImageUrl,
                 LiveUrl = p.LiveUrl,
                 GitHubUrl = p.GitHubUrl,
                 CreatedAt = p.CreatedAt,
                 UpdatedAt = p.UpdatedAt
             }).ToListAsync();
             
             
            
    }




}

