
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Dialogs;

public class ActionOptionsDialogViewModel : DialogViewModelBase
{
    [Parameter] public ActionBase Action { get; set; } = null!;
    [Parameter] public EditProjectViewModel Parent { get; set; } = null!;
}
