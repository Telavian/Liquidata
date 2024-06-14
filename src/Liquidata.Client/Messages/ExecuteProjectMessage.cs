using Liquidata.Common;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Client.Messages;

public class ExecuteProjectMessage
{
    public Project Project { get; set; } = null!;
    public bool AllowInteractive { get; set; }
    public IExecutionService ExecutionService { get; set; } = null!;
}
