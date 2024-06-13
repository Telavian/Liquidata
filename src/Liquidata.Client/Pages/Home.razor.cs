using Liquidata.Client.Pages.Common;
using Liquidata.Common.Models;
using Liquidata.Common.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages;

public partial class HomeViewModel : ViewModelBase
{
    [Inject]
    private IProjectService _projectService { get; set; } = null!;

    private Func<Task>? _createProjectAsyncCommand;
    public Func<Task> CreateProjectAsyncCommand => _createProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleCreateProjectAsync, "Unable to create project");

    private Func<Task>? _loadProjectAsyncCommand;
    public Func<Task> LoadProjectAsyncCommand => _loadProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleLoadProjectAsync, "Unable to load project");

    private Func<Task>? _removeProjectAsyncCommand;
    public Func<Task> RemoveProjectAsyncCommand => _removeProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleRemoveProjectAsync, "Unable to remove project");

    public ProjectInfo[] AllProjects { get; set; } = Array.Empty<ProjectInfo>();
    public ProjectInfo? SelectedProject { get; set; }

    public const string NavigationPath = "/";

    protected override async Task OnInitializedAsync()
    {
        var projects = await _projectService.LoadAllProjectsAsync();
        AllProjects = projects ?? Array.Empty<ProjectInfo>();

        await RefreshAsync();
        await base.OnInitializedAsync();
    }

    private async Task HandleCreateProjectAsync()
    {
        await Task.Yield();
        await NavigateToAsync(CreateProjectViewModel.NavigationPath);
    }

    private async Task HandleLoadProjectAsync()
    {
        if (SelectedProject is null)
        {
            await ShowAlertAsync("No project selected");
            return;
        }

        await NavigateToAsync($"/{EditProjectViewModel.NavigationPath}?ProjectId={SelectedProject.ProjectId}");
    }

    private async Task HandleRemoveProjectAsync()
    {
        var project = SelectedProject;
        if (project is null)
        {
            return;
        }

        var isConfirm = await ConfirmActionAsync("Delete project", $"Remove project '{project.Name}'? This can not be undone.");

        if (isConfirm == true)
        {
            await _projectService.DeleteProjectAsync(project.ProjectId);
            AllProjects = AllProjects
                .Where(x => x.ProjectId != project.ProjectId)
                .ToArray();
        }
    }
}
