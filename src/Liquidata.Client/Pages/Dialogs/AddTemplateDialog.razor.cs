using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.Dialogs;

public class AddTemplateDialogViewModel : DialogViewModelBase
{
    public string TemplateName { get; set; } = "";
    public string TemplateUrl { get; set; } = "";

    private Func<Task>? _addTemplateAsyncCommand;
    public Func<Task> AddTemplateAsyncCommand => _addTemplateAsyncCommand ??= CreateEventCallbackAsyncCommand(() => HandleAddTemplateAsync(), "Unable to add template");

    private async Task HandleAddTemplateAsync()
    {
        await Task.Yield();

        if (string.IsNullOrWhiteSpace(TemplateName))
        {
            await ShowAlertAsync("Template name is required");
            return;
        }

        var isValid = string.IsNullOrWhiteSpace(TemplateUrl) || Uri.TryCreate(TemplateUrl, UriKind.Absolute, out _);
        if (!isValid)
        {                
            await ShowAlertAsync("Template url is required");
            return;
        }

        var newTemplate = new Template
        {
            Name = TemplateName,
            Url = TemplateUrl,
        };

        Dialog?.Close(newTemplate);
    }
}
