using System.Text.Json.Serialization;

namespace Liquidata.Common.Execution;

public class ExecutionResults
{
    [JsonIgnore] public string[] AllColumns { get; set; } = [];

    public DataRecord[] Records { get; set; } = [];
    public Screenshot[] Screenshots { get; set; } = [];
    public string[] LoggedMessages { get; set; } = [];
}
