using Liquidata.Client.Pages.Dialogs;
using Liquidata.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Models;
using Liquidata.Client.Services.Interfaces;
using BlazorComponentBus;
using Liquidata.Client.Messages;
using System.Text.Json.Serialization;
using BlazorFileSaver;
using Liquidata.Common.Services;
using Liquidata.UI.Common.Pages.Common;

namespace Liquidata.Client.Pages;

public partial class EditProjectViewModel : ViewModelBase, IDisposable
{
    private bool _isBrowserInitialized;
    private static Func<XPathSelection, Task> _processSelectedItemAction = async selection => await Task.Yield();
    
    [Inject] private ComponentBus _bus { get; set; } = null!;
    [Inject] private IJSRuntime? _jsRuntime { get; set; } = null!;
    [Inject] private IBlazorFileSaver _blazorFileSaver { get; set; } = null!;
    [Inject] private IClientBrowserService _browserService { get; set; } = null!;
    [Inject] private IXPathProcessorService _xPathProcessorService { get; set; } = null!;
    [Inject] private IProjectService _projectService { get; set; } = null!;

    public const string NavigationPath = "EditProject";

    [Parameter]
    [SupplyParameterFromQuery]
    public Guid ProjectId { get; set; }

    public string RelativeSelectionParent { get; set; } = "";

    public Project? CurrentProject { get; set; } = new Project();
    public BrowserMode BrowserMode { get; set; } = BrowserMode.Select;
    public string? ActiveUrl { get; set; } = "";

    private Template _selectedTemplate = new Template();
    public Template SelectedTemplate 
    {
        get => _selectedTemplate;
        set => UpdateProperty(ref _selectedTemplate, value,
            v => HandleNewTemplateSelectedAsync(v));
    }    

    private ActionBase? _selectedAction;
    public ActionBase? SelectedAction
    {
        get => _selectedAction;
        set => UpdateProperty(ref _selectedAction, value,
            v => ProcessSelectedActionChangedAsync());
    }

    public IReadOnlyCollection<ActionBase> TemplateActionItems { get; set; } = [];

    private Func<Task>? _addTemplateAsyncCommand;
    public Func<Task> AddTemplateAsyncCommand => _addTemplateAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleAddTemplateAsync, "Unable to add template");

    private Func<ActionBase, Task>? _addChildActionAsyncCommand;
    public Func<ActionBase, Task> AddChildActionAsyncCommand => _addChildActionAsyncCommand ??= CreateEventCallbackAsyncCommand<ActionBase>(HandleAddChildActionAsync, "Unable to add child action");

    private Func<ActionBase, Task>? _removeActionAsyncCommand;
    public Func<ActionBase, Task> RemoveActionAsyncCommand => _removeActionAsyncCommand ??= CreateEventCallbackAsyncCommand<ActionBase>(HandleRemoveActionAsync, "Unable to remove action");

    private Func<Task>? _toggleBrowserModeAsyncCommand;
    public Func<Task> ToggleBrowserModeAsyncCommand => _toggleBrowserModeAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleToggleBrowserModeAsync, "Unable to toggle browser mode");

    private Func<Task>? _browserLoadedAsyncCommand;
    public Func<Task> BrowserLoadedAsyncCommand => _browserLoadedAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleBrowserLoadedAsync, "Unable to process browser loaded");

    private Func<Task>? _displayProjectSettingsAsyncCommand;
    public Func<Task> DisplayProjectSettingsAsyncCommand => _displayProjectSettingsAsyncCommand ??= CreateEventCallbackAsyncCommand(DisplayProjectSettingsAsync, "Unable to display project settings");

    private Func<ActionBase, Task>? _clearSelectionAsyncCommand;
    public Func<ActionBase, Task> ClearSelectionAsyncCommand => _clearSelectionAsyncCommand ??= CreateEventCallbackAsyncCommand<ActionBase>(HandleClearSelectionAsync, "Unable to clear selection");

    private Func<string, Task>? _loadTemplateUrlAsyncCommand;
    public Func<string, Task> LoadTemplateUrlAsyncCommand => _loadTemplateUrlAsyncCommand ??= CreateEventCallbackAsyncCommand<string>(HandleLoadTemplateUrlAsync, "Unable to load template url");

    private Func<ActionBase, Task>? _showActionOptionsAsyncCommand;
    public Func<ActionBase, Task> ShowActionOptionsAsyncCommand => _showActionOptionsAsyncCommand ??= CreateEventCallbackAsyncCommand<ActionBase>(HandleShowActionOptionsAsync, "Unable to show action options");

    private Func<Task>? _navigateHomeAsyncCommand;
    public Func<Task> NavigateHomeAsyncCommand => _navigateHomeAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleNavigateHomeAsync, "Unable to navigate home");

    private Func<Task>? _saveProjectAsyncCommand;
    public Func<Task> SaveProjectAsyncCommand => _saveProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleSaveProjectAsync, "Unable to save project");

    private Func<Task>? _saveAsProjectAsyncCommand;
    public Func<Task> SaveAsProjectAsyncCommand => _saveAsProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleSaveAsProjectAsync, "Unable to save as project");

    private Func<Task>? _exportProjectAsyncCommand;
    public Func<Task> ExportProjectAsyncCommand => _exportProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleExportProjectAsync, "Unable to export project");

    private Func<Task>? _runProjectAsyncCommand;
    public Func<Task> RunProjectAsyncCommand => _runProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleRunProjectAsync, "Unable to run project");

    private Func<Task>? _deleteProjectAsyncCommand;
    public Func<Task> DeleteProjectAsyncCommand => _deleteProjectAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleDeleteProjectAsync, "Unable to delete project");

    [JSInvokable]
    public static async Task ProcessSelectedItemAsync(string xpathSelection)
    {
        await Task.Yield();
        var selection = xpathSelection.FromJson<XPathSelection>()!;

        await _processSelectedItemAction(selection);
    }

    public void Dispose()
    {
        _bus.UnSubscribe<ActionUpdatedMessage>(HandleActionUpdatedMessage);
    }

    protected override async Task OnInitializedAsync()
    {
        CurrentProject = await _projectService.LoadProjectAsync(ProjectId);
        
        if (CurrentProject is null)
        {
            await ShowAlertAsync("Unable to load project");
            return;
        }

        Console.WriteLine($"Displaying project '{CurrentProject.Name}'");
        SelectedTemplate = CurrentProject.AllTemplates
            .First(x => x.Name == Template.MainTemplateName);

        Console.WriteLine($"Setting active template '{SelectedTemplate?.Name}'");
        _processSelectedItemAction = HandleItemSelectedAsync;
        
        ActiveUrl = string.IsNullOrWhiteSpace(SelectedTemplate?.Url) 
            ? CurrentProject?.Url 
            : SelectedTemplate?.Url;

        _bus.Subscribe<ActionUpdatedMessage>(HandleActionUpdatedMessage);
        await base.OnInitializedAsync();
    }

    protected Color BuildBrowserModeColor()
    {
        if (BrowserMode == BrowserMode.Browse)
        {
            return Color.Success;
        }
        
        if (BrowserMode == BrowserMode.Select)
        {
            return Color.Primary;
        }

        throw new Exception($"Unknown browser mode: '{BrowserMode}'");
    }

    private async Task HandleAddTemplateAsync()
    {
        var (success, templateResult) = await ShowDialogAsync<AddTemplateDialog, Template>("Add Template");

        if (!success || templateResult is null)
        {
            return;
        }

        CurrentProject!.AllTemplates.Add(templateResult);
        SelectedTemplate = templateResult;
    }

    private async Task HandleAddChildActionAsync(ActionBase action)
    {
        await Task.Yield();
        var (success, actionType) = await ShowDialogAsync<AddActionDialog, ActionType>("Add Action");
        
        if (!success)
        {
            return;
        }

        var newAction = action.AllowChildren 
            ? action.AddChildAction(CurrentProject!, actionType) 
            : action.AddSiblingAction(CurrentProject!, actionType);

        await PerformActionInitializationAsync(newAction);
        await _bus.Publish(new ActionUpdatedMessage { ActionId = action?.ActionId ?? Guid.Empty });
    }

    private async Task HandleRemoveActionAsync(ActionBase action)
    {
        await Task.Yield();

        var isConfirmed = await ConfirmActionAsync("Remove action?", "Remove action and all descendants?");

        if (isConfirmed == true)
        {
            await RemoveActionAsync(action);
        }

        await _bus.Publish(new ActionUpdatedMessage { ActionId = action?.ActionId ?? Guid.Empty });
    }

    private async Task HandleToggleBrowserModeAsync()
    {
        await Task.Yield();

        if (BrowserMode == BrowserMode.Browse)
        {
            BrowserMode = BrowserMode.Select;
        }
        else if (BrowserMode == BrowserMode.Select)
        {
            BrowserMode = BrowserMode.Browse;
        }

        await UpdateBrowserSelectionModeAsync();
    }

    private async Task RemoveActionAsync(ActionBase action)
    {
        if (action.ActionType == ActionType.Template)
        {
            if (action.Name == Template.MainTemplateName)
            {
                await ShowAlertAsync("Unable remove main template");
                return;
            }

            CurrentProject!.AllTemplates.Remove((action as Template)!);
            SelectedTemplate = CurrentProject.AllTemplates.First();
            return;
        }

        var isSelected = SelectedAction == action ||
            action.TraverseTree()
                .Where(x => x.ActionId == action.ActionId)
                .Any();

        action.Parent!.RemoveChild(action);

        if (isSelected)
        {
            SelectedAction = null;
        }
    }

    private async Task DisplayProjectSettingsAsync()
    {
        await Task.Yield();
        var parameters = new DialogParameters
        {
            { nameof(ProjectSettingsDialog.CurrentProject), CurrentProject }
        };

        await ShowDialogAsync<ProjectSettingsDialog>("Project settings", parameters);
    }

    private async Task HandleBrowserLoadedAsync()
    {
        Console.WriteLine("Initializing browser");

        // https://stackoverflow.com/questions/35432749/disable-web-security-in-chrome-48
        // CSP Unblock
        // Cors Unblock        
        var hasAccess = await _browserService.CheckForDocumentAccessAsync();        

        if (!hasAccess)
        {
            _isBrowserInitialized = true;
            await ShowAlertAsync(Constants.WebSecurityErrorMessage, true);
            return;
        }

        _isBrowserInitialized = true;
        Console.WriteLine("Executing browser initialization");        
        await _browserService.InitializeBrowserAsync();
        await _browserService.WaitForBrowserReadyAsync(TimeSpan.FromSeconds(10));

        await UpdateBrowserSelectionModeAsync();
        await ExecuteProjectAsync(CurrentProject!);
    }

    private async Task HandleItemSelectedAsync(XPathSelection selection)
    {
        await Task.Yield();

        if (selection.IsShiftKey)
        {
            await ShowSelectionDetailsAsync(selection.XPath);
            return;
        }

        var currentAction = SelectedAction;
        if (currentAction is null || currentAction is not SelectionActionBase selectionAction)
        {
            return;
        }
        
        var shouldContinue = true;
        if (selectionAction.ActionType == ActionType.RelativeSelect)
        {
            shouldContinue = await ProcessRelativeSelectedItemAsync(selection);
        }        

        if (shouldContinue)
        {
            await MergeSelectedItemAsync(selectionAction, selection.XPath);
        }        

        await HighlightSelectionsAsync();
        await RefreshAsync();
    }

    private async Task<bool> ProcessRelativeSelectedItemAsync(XPathSelection selection)
    {
        if (string.IsNullOrWhiteSpace(RelativeSelectionParent))
        {
            RelativeSelectionParent = selection.XPath;
            return false;
        }
                
        selection.XPath = await _xPathProcessorService.DetermineRelativeXPathAsync(RelativeSelectionParent, selection.XPath);
        RelativeSelectionParent = "";        

        return true;
    }

    private async Task ShowSelectionDetailsAsync(string xpath)
    {
        var info = await _browserService.GetSelectionInfoAsync(xpath);

        var parameters = new DialogParameters
        {
            { nameof(SelectionInfoDialog.Info), info }
        };
        await ShowDialogAsync<SelectionInfoDialog>("Selection Info", parameters);
    }

    private async Task<bool> MergeSelectedItemAsync(SelectionActionBase selection, string xpath)
    {
        if (string.IsNullOrWhiteSpace(selection.XPath))
        {
            selection.XPath = xpath;
            return true;
        }
                
        var (success, operation) = await ShowDialogAsync<SelectionOperationDialog, SelectionOperation>("Selection Operation");

        if (!success)
        {
            return false;
        }

        selection!.XPath = await _xPathProcessorService.ProcessXPathOperationAsync(selection.XPath, xpath, operation);
        return true;
    }

    private async Task ProcessSelectedActionChangedAsync()
    {
        await Task.Yield();
        RelativeSelectionParent = "";

        await UpdateBrowserSelectionModeAsync();
        await HighlightSelectionsAsync();
    }

    private async Task UpdateBrowserSelectionModeAsync()
    {
        await Task.Yield();
        var effectiveMode = BrowserMode == BrowserMode.Select && (SelectedAction?.IsSelectionAction() ?? false)
            ? BrowserMode.Select
            : BrowserMode.Browse;

        await _browserService
            .UpdateBrowserSelectionModeAsync(SelectedAction, effectiveMode);
    }

    private async Task HighlightSelectionsAsync()
    {
        var allSelections = (SelectedAction?.GetAllSelectionAncestors() ?? [])
            .Reverse();

        await _browserService.ClearCurrentSelectionsAsync();
        var lastSelection = "";

        foreach (var selection in allSelections)
        {
            var selectionXPath = selection.XPath;            

            if (selection.ActionType == ActionType.Select)
            {
                await _browserService.HighlightSelectionsAsync([selectionXPath]);
            }
            else if (selection.ActionType == ActionType.RelativeSelect)
            {
                selectionXPath = _xPathProcessorService.MakeRelativeXPathQuery(lastSelection, selectionXPath);
                await _browserService.HighlightRelativeSelectionsAsync([selectionXPath]);
            }

            lastSelection = selectionXPath;
        }

        await _browserService.HighlightRelativeSelectionParentAsync(RelativeSelectionParent);
    }

    private async Task HandleClearSelectionAsync(ActionBase action)
    {
        await Task.Yield();
        if (!action.IsSelectionAction())
        {
            return;
        }

        var isConfirmed = await ConfirmActionAsync("Clear", "Clear selection? This can not be undone.");

        if (isConfirmed != true)
        {
            return;
        }

        var selectionAction = (SelectionActionBase)action;
        selectionAction.XPath = "";

        await HighlightSelectionsAsync();
    }

    private async Task HandleNewTemplateSelectedAsync(Template template)
    {
        TemplateActionItems = [template];

        if (!string.IsNullOrWhiteSpace(template.Url))
        {
            await HandleLoadTemplateUrlAsync(template.Url);
        }
    }

    private async Task HandleLoadTemplateUrlAsync(string url)
    {
        await Task.Yield();

        if (ActiveUrl == url)
        {
            return;
        }

        var isConfirm = await ConfirmActionAsync("Load url", "Load template url?");
        
        if (isConfirm == true)
        {
            ActiveUrl = url;
        }        
    }

    private async Task HandleShowActionOptionsAsync(ActionBase action)
    {
        await Task.Yield();
        
        var parameters = new DialogParameters
        {
            { nameof(ActionOptionsDialogViewModel.Action), action },
            { nameof(ActionOptionsDialogViewModel.Parent), this }
        };

        await ShowDialogAsync<ActionOptionsDialog>("Options", parameters);
    }

    private async Task HandleNavigateHomeAsync()
    {
        var isConfirm = await ConfirmActionAsync("Home", "Navigate home? All unsaved changes will be lost");

        if (isConfirm == true)
        {
            await NavigateToAsync($"{HomeViewModel.NavigationPath}");
        }
    }

    private async Task HandleSaveProjectAsync()
    {
        if (CurrentProject is null)
        {
            return;
        }

        await _projectService.SaveProjectAsync(CurrentProject);
        await ShowSnackbarMessageAsync("Project saved");
    }

    private async Task HandleSaveAsProjectAsync()
    {
        if (CurrentProject is null)
        {
            return;
        }

        var (success, saveAsResult) = await ShowDialogAsync<SaveAsProjectSettingsDialog, string>("Save project as");

        if (!success || string.IsNullOrWhiteSpace(saveAsResult))
        {
            return;
        }

        CurrentProject = CurrentProject.Clone(saveAsResult);

        await _projectService.SaveProjectAsync(CurrentProject);
        await ShowSnackbarMessageAsync($"Project saved to '{saveAsResult}'");
    }

    private async Task HandleExportProjectAsync()
    {
        if (CurrentProject is null)
        {
            return;
        }

        var json = CurrentProject.ToJson();
        await _blazorFileSaver.SaveAs($"{CurrentProject.Name}.json", json, "application/json");
    }

    private async Task HandleRunProjectAsync()
    {
        await Task.Yield();

        if (CurrentProject is null)
        {
            return;
        }

        var isConfirm = await ConfirmActionAsync("Run", "Save current changes and run complete project?");

        if (isConfirm == true)
        {
            await SaveProjectAsyncCommand();
            var url = $"/RunProject?ProjectId={CurrentProject.ProjectId}";

            await _jsRuntime!.InvokeVoidAsync("open", url, "_blank");
        }
    }

    private async Task HandleDeleteProjectAsync()
    {
        await Task.Yield();

        if (CurrentProject is null)
        {
            return;
        }

        var isConfirm = await ConfirmActionAsync("Delete project", $"Delete project '{CurrentProject.Name}'? This action can not be undone");

        if (isConfirm == true)
        {
            await _projectService.DeleteProjectAsync(CurrentProject.ProjectId);
            await NavigateToAsync($"{HomeViewModel.NavigationPath}");
        }
    }

    private void HandleActionUpdatedMessage(MessageArgs message)
    {
        if (CurrentProject == null)
        {
            return;
        }

        var project = CurrentProject.FullClone();
        _ = ExecuteProjectAsync(project);
    }

    private async Task ExecuteProjectAsync(Project project)
    {
        await Task.Yield();

        var dataHandler = new DataHandlerService();        

        var executionService = new ExecutionService
        {
            Project = CurrentProject!,
            Concurrency = 1,
            Browser = _browserService,
            DataHandler = dataHandler,
            XPathProcessor = _xPathProcessorService,
        };
                
        await executionService.RegisterBrowserAsync(_browserService);

        await _bus.Publish(new ExecuteProjectMessage { Project = project, AllowInteractive = false, ExecutionService = executionService });
        await executionService.UnregisterBrowserAsync(_browserService);
    }

    private async Task PerformActionInitializationAsync(ActionBase action)
    {
        await Task.Yield();

        if (action.ActionType == ActionType.Select)
        {
            await PerformSelectActionInitializationAsync(action);
            return;
        }
    }

    private async Task PerformSelectActionInitializationAsync(ActionBase action)
    {
        await Task.Yield();

        var existingSelect = CurrentProject!.AllTemplates
            .SelectMany(x => x.TraverseTree())
            .Where(x => x.ActionType == ActionType.Select && x.ActionId != action.ActionId)
            .Any();

        if (existingSelect)
        {
            return;
        }

        // Add these for convenience
        action.AddChildAction(CurrentProject!, ActionType.BeginRecord);
        action.AddChildAction(CurrentProject!, ActionType.Extract);
    }
}