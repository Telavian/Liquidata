using Liquidata.Client.Pages.Common;
using Liquidata.Client.Pages.Dialogs;
using Liquidata.Client.Services;
using Liquidata.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using Liquidata.Client.Models;

namespace Liquidata.Client.Pages;

public partial class EditProjectViewModel : ViewModelBase
{
    private bool _isBrowserInitialized;
    private static Func<XPathSelection, Task> _processSelectedItemAction = async selection => await Task.Yield();

    private BrowserService _browserService = null!;

    public const string NavigationPath = "EditProject";

    [Inject]
    private IJSRuntime? _jsRuntime { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery]
    public Guid ProjectId { get; set; }

    public ElementReference OptionsPanel { get; set; }

    public Project CurrentProject { get; set; } = new Project();
    public BrowserMode BrowserMode { get; set; } = BrowserMode.Select;

    private Template _selectedTemplate = new Template();
    public Template SelectedTemplate 
    {
        get => _selectedTemplate;
        set => UpdateProperty(ref _selectedTemplate, value,
            v => TemplateActionItems = [v]);
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

    [JSInvokable]
    public static async Task ProcessSelectedItemAsync(string xpathSelection)
    {
        await Task.Yield();
        var selection = JsonSerializer.Deserialize<XPathSelection>(xpathSelection);

        await _processSelectedItemAction(selection);
    }

    protected override async Task OnInitializedAsync()
    {
        _browserService = new BrowserService(_jsRuntime!);

        var projectKey = Constants.Browser.ProjectKey(ProjectId);
        CurrentProject = (await LoadSettingAsync<Project>(projectKey))!;
        
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

        CurrentProject.AllTemplates.Add(templateResult);
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
            ? action.AddChildAction(actionType) 
            : action.AddSiblingAction(actionType);
    }

    private async Task HandleRemoveActionAsync(ActionBase action)
    {
        await Task.Yield();

        var isConfirmed = await ConfirmActionAsync("Remove action?", "Remove action and all descendants?");

        if (isConfirmed == true)
        {
            await RemoveActionAsync(action);
        }        
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

            CurrentProject.AllTemplates.Remove((action as Template)!);
            SelectedTemplate = CurrentProject.AllTemplates.First();
            return;
        }

        var isSelected = SelectedAction == action ||
            action.FindActions(x => x.ActionId == action.ActionId).Any();

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
        await Task.Yield();
        Console.WriteLine("Initializing browser");

        // https://stackoverflow.com/questions/35432749/disable-web-security-in-chrome-48
        var isWebSecurity = await _browserService.CheckIfWebSecurityEnabledAsync();

        if (isWebSecurity)
        {
            // iframe onload fires event twice: https://stackoverflow.com/questions/10781880/dynamically-created-iframe-triggers-onload-event-twice
            if (_isBrowserInitialized)
            {
                return;
            }

            _isBrowserInitialized = true;
            await ShowAlertAsync("<b>Web security is currently enabled or the site can't be embedded. Many features will not work correctly</b><br>" +
                "<a href='https://xyz.com'><u>Discover ways to work around this limitation</u></a>", true);
            return;
        }

        _isBrowserInitialized = true;
        Console.WriteLine("Executing browser initialization");
        await _browserService.WaitForBrowserReadyAsync(TimeSpan.FromSeconds(10));
        await _browserService.InitializeBrowserAsync();

        await UpdateBrowserSelectionModeAsync();
    }

    private async Task HandleItemSelectedAsync(XPathSelection selection)
    {
        await Task.Yield();

        if (selection.IsShiftKey)
        {
            await ShowSelectionDetailsAsync(selection.XPath);
            return;
        }

        var shouldContinue = await MergeSelectedItemAsync(SelectedAction, selection.XPath);

        if (!shouldContinue)
        {
            return;
        }

        await HighlightSelectionsAsync();
        await RefreshAsync();
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

    private async Task<bool> MergeSelectedItemAsync(ActionBase? action, string xpath)
    {
        if (action is null || action is not SelectionActionBase selection)
        {
            return false;
        }

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

        selection!.XPath = DetermineSelectionXPath(selection.XPath, xpath, operation);
        return true;
    }

    private string DetermineSelectionXPath(string? currentXPath, string newXPath, SelectionOperation operation)
    {
        if (operation == SelectionOperation.Replace)
        {
            return newXPath;
        }

        if (operation == SelectionOperation.Combine)
        {
            // TODO: Finish combine
            return "xyz";
        }

        throw new Exception($"Unknown selection operation: {operation}");
    }

    private async Task ProcessSelectedActionChangedAsync()
    {
        await Task.Yield();
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
            .UpdateBrowserSelectionModeAsync(effectiveMode);
    }

    private async Task HighlightSelectionsAsync()
    {
        var allSelections = (SelectedAction?.GetAllSelectionAncestors() ?? [])
            .Reverse();

        await _browserService.ClearCurrentSelectionsAsync();

        foreach (var selection in allSelections)
        {
            if (selection.ActionType == ActionType.Select)
            {
                await _browserService.HighlightSelectionsAsync([selection.XPath]);
            }
            else if (selection.ActionType == ActionType.RelativeSelect)
            {
                await _browserService.HighlightRelativeSelectionsAsync([selection.XPath]);
            }
        }                       
    }
}