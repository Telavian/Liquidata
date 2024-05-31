using Liquidata.Client.Extensions;
using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.ActionDisplay
{
    public abstract class ActionDisplayViewModelBase<T> : ViewModelBase
        where T : ActionBase
    {
        [Parameter]
        public ActionBase? Action { get; set; }

        [Parameter]
        public EditProjectViewModel? Parent { get; set; }

        public T TypedAction => (T)Action!;
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // TODO: Optimize
            await RefreshAsync();
        }
    }
}
