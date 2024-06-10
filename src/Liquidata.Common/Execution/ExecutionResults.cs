namespace Liquidata.Common.Execution
{
    public class ExecutionResults
    {
        public string[] AllColumns { get; set; } = [];
        public DataRecord[] Records { get; set; } = [];
        public Screenshot[] Screenshots { get; set; } = [];
    }
}
