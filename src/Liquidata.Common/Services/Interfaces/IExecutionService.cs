namespace Liquidata.Common.Services.Interfaces
{
    public interface IExecutionService
    {
        public IDataExtractorService DataExtractor { get; }
        public IBrowserService Browser { get; }
    }
}
