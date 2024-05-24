using Liquidata.Client.Extensions;
using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
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
        public bool IsMouseOver { get; set; }        

        private Func<bool, Task>? _updateIsMouseOverAsyncCommand;
        public Func<bool, Task> UpdateIsMouseOverAsyncCommand => _updateIsMouseOverAsyncCommand ??= CreateEventCallbackAsyncCommand<bool>(UpdateIsMouseOverAsync, "Unable to update is mouse over");

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // TODO: Optimize
            await RefreshAsync();
        }

        protected string BuildItemVisibility(params bool[] states)
        {
            var anyTrue = states.Any(x => x);

            return anyTrue
                ? "display: block"
                : "display: none";
        }

        protected string BuildActionIcon()
        {
            return Action!.ActionType.BuildActionIcon();
        }

        protected Color BuildActionColor()
        {
            return Action!.ActionType.BuildActionColor();
        }

        private async Task UpdateIsMouseOverAsync(bool isOver)
        {
            await Task.Yield();
            IsMouseOver = isOver;
        }

    }
}
