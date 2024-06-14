using Liquidata.Common.Execution;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Execution
{
    public class LogsDisplayViewModel : ViewModelBase
    {
        [Parameter] public ExecutionResults ExecutionResults { get; set; } = null!;
    }
}
