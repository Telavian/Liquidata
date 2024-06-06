using Liquidata.Common;
using Liquidata.Common.Models;

namespace Liquidata.Common.Services.Interfaces;

public interface IProjectService
{
    Task SaveProjectAsync(Project project);
    Task DeleteProjectAsync(Guid projectId);
    Task<Project?> LoadProjectAsync(Guid projectId);
    Task<ProjectInfo[]> LoadAllProjectsAsync();
}
