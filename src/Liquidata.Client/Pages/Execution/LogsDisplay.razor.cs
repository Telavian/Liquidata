using Liquidata.Client.Pages.Common;
using Liquidata.Common.Execution;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Execution
{
    public class LogsDisplayViewModel : ViewModelBase
    {
        [Parameter] public ExecutionResults ExecutionResults { get; set; } = null!;
    }
}
