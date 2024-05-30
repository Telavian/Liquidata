using Liquidata.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Dialogs
{
    public class ProjectSettingsDialogViewModel : DialogViewModelBase
    {
        [Parameter]
        public Project CurrentProject { get; set; } = null!;

        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public bool LoadImages { get; set; }
        public bool RotateIpAddresses { get; set; }
        public int Concurrency { get; set; }

        private Func<Task>? _saveSettingsAsyncCommand;
        public Func<Task> SaveSettingsAsyncCommand => _saveSettingsAsyncCommand ??= CreateEventCallbackAsyncCommand(() => HandleSaveSettingsAsync(), "Unable to save settings");

        protected override async Task OnParametersSetAsync()
        {
            await Task.Yield();

            Name = CurrentProject?.Name ?? "";
            Url = CurrentProject?.Url ?? "";
            LoadImages = CurrentProject?.LoadImages ?? false;
            RotateIpAddresses = CurrentProject?.RotateIpAddresses ?? false;
            Concurrency = CurrentProject?.Concurrency ?? 0;
        }

        private async Task HandleSaveSettingsAsync()
        {
            await Task.Yield();
            
            if (string.IsNullOrWhiteSpace(Name))
            {
                await ShowAlertAsync("Name is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(Url))
            {
                await ShowAlertAsync("Url is required");
                return;
            }

            CurrentProject.Name = Name;
            CurrentProject.Url = Url;
            CurrentProject.LoadImages = LoadImages;
            CurrentProject.RotateIpAddresses = RotateIpAddresses;
            CurrentProject.Concurrency = Concurrency;

            Dialog?.Close();
        }
    }
}
