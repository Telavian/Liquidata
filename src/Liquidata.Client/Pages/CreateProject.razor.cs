using Liquidata.Client.Pages.Common;
using Liquidata.Common;
using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages;

public partial class CreateProjectViewModel : ViewModelBase
{
    public const string NavigationPath = "CreateProject";

    public string? Name { get; set; }
    public string? Url { get; set; }
    public bool LoadImages { get; set; } = true;
    public bool RotateIpAddresses { get; set; } = false;
    public int Concurrency { get; set; } = 3;

    private Func<Task>? _createProjectAsyncCommand;
    public Func<Task> CreateProjectAsyncCommand => _createProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleCreateProjectAsync, "Unable to create project");

    private async Task HandleCreateProjectAsync()
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
        {
            validationErrors.Add("Name is required");
        }

        if (!Uri.TryCreate(Url, UriKind.Absolute, out _))
        {
            validationErrors.Add("Url is required");
        }

        if (validationErrors.Any())
        {
            var errors = validationErrors
                .Select(x => $"<br>{x}");
            var errorText = $"Validation errors: {string.Join("", errors)}";

            await ShowAlertAsync(errorText, true);
            return;
        }

        var projectId = await SaveProjectAsync();
        await NavigateToAsync($"/{EditProjectViewModel.NavigationPath}?projectId={projectId}");
    }

    private async Task<Guid> SaveProjectAsync()
    {
        var newProject = new Project
        {
            Name = Name!,
            Url = Url!,
        };

        newProject.AllTemplates.Add(new Template
        {
            Name = Template.MainTemplateName,
            Url = Url ?? ""
        });

        Console.WriteLine($"Saving project '{Name}'");
        var projectKey = Constants.Browser.ProjectKey(newProject.ProjectId);
        await SaveSettingAsync(projectKey, newProject);

        Console.WriteLine($"Updating all projects");
        var allProjects = await LoadSettingAsync(Constants.Browser.AllProjectsKey, new List<Project>());
        allProjects!.Add(newProject);
        await SaveSettingAsync(Constants.Browser.AllProjectsKey, allProjects);
        
        Console.WriteLine($"Project created");
        return newProject.ProjectId;
    }
}
