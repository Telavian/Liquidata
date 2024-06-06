using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Services.Interfaces;

public interface IXPathProcessorService
{
    Task<string> ProcessXPathOperationAsync(string? currentXPath, string newXPath, SelectionOperation operation);
    string? MakeRelativeXPathQuery(string? parent, string? xpath);
    Task<string> DetermineRelativeXPathAsync(string parent, string xPath);
}
