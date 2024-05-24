using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions
{
    public abstract class ActionOptionsViewModelBase<T> : ViewModelBase
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
