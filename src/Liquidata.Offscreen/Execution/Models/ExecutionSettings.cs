using Liquidata.Common;
using Liquidata.Offscreen.Enums;

namespace Liquidata.Offscreen.Execution.Models
{
    public class ExecutionSettings
    {
        public Project Project { get; init; } = null!;
        public BrowserType Browser { get; init; }
        public FileInfo? BrowserPath { get; init; }
        public bool DisableImages { get; init; }
        public Uri? ProxyServer { get; init; }
        public string? ProxyUser { get; init; }
        public string? ProxyPassword { get; init; }
        public int? Concurrency { get; init; }        
        public bool IsHeadless { get; init; }
        public FileInfo? OutputLocation { get; init; }
        public bool PrintOutput { get; init; }
    }
}
