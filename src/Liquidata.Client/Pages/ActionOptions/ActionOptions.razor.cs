using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ActionOptionsViewModel : ViewModelBase
{    
    [Parameter]
    public ActionBase? Action { get; set; }

    [Parameter]
    public EditProjectViewModel? Parent { get; set; }

    protected override async Task OnParametersSetAsync()
    {        
        await base.OnParametersSetAsync();

        // TODO: Optimize
        await RefreshAsync();
    }
}
