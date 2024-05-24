using Liquidata.Client.Pages.Common;
using Liquidata.Common;

namespace Liquidata.Client.Pages
{
    public partial class HomeViewModel : ViewModelBase
    {
        private Func<Task>? _createProjectAsyncCommand;
        public Func<Task> CreateProjectAsyncCommand => _createProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleCreateProjectAsync, "Unable to create project");

        private Func<Task>? _loadProjectAsyncCommand;
        public Func<Task> LoadProjectAsyncCommand => _loadProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleLoadProjectAsync, "Unable to load project");

        public Project[] AllProjects { get; set; } = Array.Empty<Project>();
        public Project? SelectedProject { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var projects = await LoadSettingAsync<Project[]>(Constants.Browser.AllProjectsKey);
            AllProjects = projects ?? Array.Empty<Project>();

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
