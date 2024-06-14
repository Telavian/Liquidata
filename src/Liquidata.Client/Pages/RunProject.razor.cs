using Liquidata.Common;
using Microsoft.AspNetCore.Components;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Client.Messages;
using BlazorComponentBus;
using Liquidata.Client.Services;
using Liquidata.Common.Services;
using Liquidata.UI.Common.Pages.Common;

namespace Liquidata.Client.Pages
{
    public partial class RunProjectViewModel : ViewModelBase
    {
        [Inject] private IProjectService _projectService { get; set; } = null!;
        [Inject] private IClientBrowserService _browserService { get; set; } = null!;
        [Inject] private ComponentBus _bus { get; set; } = null!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid ProjectId { get; set; }

        public Project? CurrentProject { get; set; } = new Project();
        public string? ActiveUrl { get; set; } = "";
        public ExecutionService? ExecutionService { get; set; }

        private IBrowserService? _selectedBrowser;
        public IBrowserService? SelectedBrowser
        {
            get => _selectedBrowser;
            set => UpdateProperty(ref _selectedBrowser, value,
                v => ProcessSelectedBrowserChangedAsync());
        }

        private Func<IBrowserService, Task>? _browserLoadedAsyncCommand;
        public Func<IBrowserService, Task> BrowserLoadedAsyncCommand => _browserLoadedAsyncCommand ??= CreateEventCallbackAsyncCommand<IBrowserService>(HandleBrowserLoadedAsync, "Unable to process browser loaded");

        protected override async Task OnInitializedAsync()
        {
            CurrentProject = await _projectService.LoadProjectAsync(ProjectId);

            if (CurrentProject is null)
            {
                await ShowAlertAsync("Unable to load project");
                return;
            }

            ActiveUrl = CurrentProject?.Url;
            await base.OnInitializedAsync();

            _ = ExecuteProjectAsync(CurrentProject!);
        }

        protected string BuildBrowserStyle(string browserId, IBrowserService? browser)
        {
            if (browser is not null && browserId == browser.BrowserId)
            {
                return "position: absolute; z-index: 0;";
            }
            else
            {
                return "position: absolute; z-index: -1;";
            }
        }

        private async Task HandleBrowserLoadedAsync(IBrowserService browser)
        {
            await Task.Yield();
            Console.WriteLine("Initializing browser");

            // https://stackoverflow.com/questions/35432749/disable-web-security-in-chrome-48
            var isWebSecurity = await browser.CheckIfWebSecurityEnabledAsync();

            if (isWebSecurity)
            {
                // iframe onload fires event twice: https://stackoverflow.com/questions/10781880/dynamically-created-iframe-triggers-onload-event-twice
                if (browser.IsBrowserInitialized)
                {
                    return;
                }

                browser.IsBrowserInitialized = true;
                await ShowAlertAsync(Constants.WebSecurityErrorMessage, true);
                return;
            }

            browser.IsBrowserInitialized = true;
            Console.WriteLine("Executing browser initialization");
            await browser.WaitForBrowserReadyAsync(TimeSpan.FromSeconds(10));
            await browser.InitializeBrowserAsync();
        }

        private async Task ExecuteProjectAsync(Project project)
        {
            await Task.Yield();

            var dataHandler = new DataHandlerService();
            var xPathProcessorService = new XPathProcessorService(_browserService);
            var browserRegistration = () => RefreshAsync();

            _browserService.RootPage = project.Url;
            ExecutionService = new ExecutionService(CurrentProject!, CurrentProject!.Concurrency, _browserService, dataHandler, xPathProcessorService, browserRegistration);            
            await ExecutionService.RegisterBrowserAsync(_browserService);
            SelectedBrowser = _browserService;
            await RefreshAsync();

            await _browserService.WaitForBrowserInitializationAsync(TimeSpan.FromSeconds(10));
            await _browserService.WaitForBrowserReadyAsync(TimeSpan.FromSeconds(10));            

            await _bus.Publish(new ExecuteProjectMessage { Project = project, AllowInteractive = true, ExecutionService = ExecutionService });
            await ExecutionService.UnregisterBrowserAsync(_browserService);
        }

        private async Task ProcessSelectedBrowserChangedAsync()
        {
            await Task.Yield();
            await RefreshAsync();
        }
    }
}
