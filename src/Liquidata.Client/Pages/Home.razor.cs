using Liquidata.Client.Models;
using Liquidata.Client.Pages.Common;
using Liquidata.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages
{
    public partial class HomeViewModel : ViewModelBase
    {
        [Inject]
        private IProjectService _projectService { get; set; } = null!;

        private Func<Task>? _createProjectAsyncCommand;
        public Func<Task> CreateProjectAsyncCommand => _createProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleCreateProjectAsync, "Unable to create project");

        private Func<Task>? _loadProjectAsyncCommand;
        public Func<Task> LoadProjectAsyncCommand => _loadProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleLoadProjectAsync, "Unable to load project");

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
    }
}
