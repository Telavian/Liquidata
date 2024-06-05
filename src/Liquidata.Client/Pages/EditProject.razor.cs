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
using System;

namespace Liquidata.Client.Pages;

public partial class EditProjectViewModel : ViewModelBase
{
    private bool _isBrowserInitialized;
    private static Func<XPathSelection, Task> _processSelectedItemAction = async selection => await Task.Yield();

    private BrowserService _browserService = null!;
    private XPathProcessorService _xpathProcessorService = null!;

    public const string NavigationPath = "EditProject";

    [Inject]
    private IJSRuntime? _jsRuntime { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery]
    public Guid ProjectId { get; set; }

    public string RelativeSelectionParent { get; set; } = "";

    public Project CurrentProject { get; set; } = new Project();
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

    [JSInvokable]
    public static async Task ProcessSelectedItemAsync(string xpathSelection)
    {
        await Task.Yield();
        var selection = JsonSerializer.Deserialize<XPathSelection>(xpathSelection)!;

        await _processSelectedItemAction(selection);
    }    

    protected override async Task OnInitializedAsync()
    {
        _browserService = new BrowserService(_jsRuntime!);
        _xpathProcessorService = new XPathProcessorService(_browserService);        

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
        
        ActiveUrl = string.IsNullOrWhiteSpace(SelectedTemplate?.Url) 
            ? CurrentProject?.Url 
            : SelectedTemplate?.Url;

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

        var _ = action.AllowChildren 
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
                
        selection.XPath = await _xpathProcessorService.DetermineRelativeXPathAsync(RelativeSelectionParent, selection.XPath);
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

        selection!.XPath = await _xpathProcessorService.ProcessXPathOperationAsync(selection.XPath, xpath, operation);
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
                selectionXPath = _xpathProcessorService.MakeRelativeXPathQuery(lastSelection, selectionXPath);
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
}