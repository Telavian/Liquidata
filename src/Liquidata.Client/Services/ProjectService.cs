using Blazored.LocalStorage;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Common;
using Liquidata.Common.Models;

namespace Liquidata.Client.Services;

public class ProjectService(ILocalStorageService localStorage) : IProjectService
{
    private const string AllProjectsKey = "AllProjects";

    public async Task SaveProjectAsync(Project project)
    {
        var key = BuildProjectKey(project.ProjectId);
        await localStorage.SetItemAsync(key, project);

        var allProjects = (await LoadAllProjectsAsync())
            .ToList();
        allProjects = allProjects
            .Where(x => x.ProjectId != project.ProjectId)
            .Concat([new ProjectInfo { ProjectId = project.ProjectId, Name = project.Name }])
            .ToList();

        await SaveAllProjectsAsync(allProjects);
    }      

    public async Task DeleteProjectAsync(Guid projectId)
    {
        var key = BuildProjectKey(projectId);
        await localStorage.RemoveItemAsync(key);

        var allProjects = (await LoadAllProjectsAsync())
            .ToList();
        allProjects = allProjects
            .Where(x => x.ProjectId != projectId)
            .ToList();

        await SaveAllProjectsAsync(allProjects);
    }

    public async Task<Project?> LoadProjectAsync(Guid projectId)
    {
        var key = BuildProjectKey(projectId);
        var project = await localStorage.GetItemAsync<Project>(key);

        project!.RestoreParentReferences();
        return project;
    }

    public async Task<ProjectInfo[]> LoadAllProjectsAsync()
    {
        var key = AllProjectsKey;
        var results = await localStorage.GetItemAsync<ProjectInfo[]>(key);

        return results ?? Array.Empty<ProjectInfo>();
    }

    private async Task SaveAllProjectsAsync(IEnumerable<ProjectInfo> allProjects)
    {
        var key = AllProjectsKey;
        await localStorage.SetItemAsync(key, allProjects);
    }

    private string BuildProjectKey(Guid projectId)
    {
        return $"Project_{projectId}";
    }
}
