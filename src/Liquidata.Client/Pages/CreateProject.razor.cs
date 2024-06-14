using Liquidata.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Services.Interfaces;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages;

public partial class CreateProjectViewModel : ViewModelBase
{
    [Inject] private IProjectService _projectService { get; set; } = null!;

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
        await _projectService.SaveProjectAsync(newProject);
        
        Console.WriteLine($"Project created");
        return newProject.ProjectId;
    }
}
