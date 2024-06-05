using CodePlex.XPathParser;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text;

namespace Liquidata.Client.Services
{
    public class XPathProcessorService(BrowserService browserService)
    {
        private const string _xpathConcatenator = " | ";

        public Task<string> ProcessXPathOperationAsync(string? currentXPath, string newXPath, SelectionOperation operation)
        {
            if (operation == SelectionOperation.Replace)
            {
                return ProcessReplaceAsync(currentXPath, newXPath);                
            }

            if (operation == SelectionOperation.Combine)
            {
                return ProcessCombineAsync(currentXPath, newXPath);
            }

            if (operation == SelectionOperation.Similar)
            {
                return ProcessSimilarAsync(currentXPath, newXPath);
            }

            if (operation == SelectionOperation.Remove)
            {
                return ProcessRemoveAsync(currentXPath, newXPath);
            }

            throw new Exception($"Unknown selection operation: {operation}");
        }

        public string? MakeRelativeXPathQuery(string? parent, string? xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return "";
            }

            if (string.IsNullOrWhiteSpace(parent))
            {
                return xpath;
            }

            return $"{parent}{xpath}";
        }

        public async Task<string> DetermineRelativeXPathAsync(string parent, string xPath)
        {
            var parentSegments = await BreakXPathIntoSegmentsAsync(parent);
            var xpathSegments = await BreakXPathIntoSegmentsAsync(xPath);

            while (parentSegments.Count > 0 && xpathSegments.Count > 0 && parentSegments[0] == xpathSegments[0])
            {
                parentSegments.RemoveAt(0);
                xpathSegments.RemoveAt(0);
            }

            while (parentSegments.Count > 0)
            {
                parentSegments.RemoveAt(0);
                xpathSegments.Insert(0, "/..");
            }

            return xpathSegments.StringJoin("");
        }

        private async Task<IList<string>> BreakXPathIntoSegmentsAsync(string xpath)
        {
            var fullXPathMatches = await browserService.GetAllMatchesAsync(xpath);
            var fullXPath = fullXPathMatches.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(fullXPath))
            {
                return [];
            }

            var segments = new List<string>();
            var index = 0;

            while(true)
            {
                var searchIndex = fullXPath[index + 1] == '/'
                    ? index + 2
                    : index + 1;
                var nextIndex = fullXPath.IndexOf('/', searchIndex);
                if (nextIndex == -1)
                {
                    segments.Add(fullXPath.Substring(index));
                    break;
                }

                segments.Add(fullXPath.Substring(index, nextIndex - index));
                index = nextIndex;
            }

            return segments;
        }

        private async Task<string> ProcessReplaceAsync(string? currentXPath, string newXPath)
        {
            await Task.Yield();
            return newXPath;
        }

        private async Task<string> ProcessCombineAsync(string? currentXPath, string newXPath)
        {
            await Task.Yield();
            return MergeExpressions(currentXPath, newXPath);
        }

        private async Task<string> ProcessSimilarAsync(string? currentXPath, string newXPath)
        {
            await Task.Yield();
            var similarNewXPath = MakeSimilarSearch(newXPath);
            var similarCurrentXPath = MakeSimilarSearch(currentXPath);

            return MergeExpressions(similarCurrentXPath, similarNewXPath);
        }        

        private async Task<string> ProcessRemoveAsync(string? currentXPath, string newXPath)
        {
            await Task.Yield();

            if (newXPath == currentXPath || string.IsNullOrWhiteSpace(currentXPath))
            {
                return "";
            }

            var expressions = SplitExpressions(currentXPath);

            var isRemoved = expressions.Remove(newXPath);
            if (isRemoved)
            {
                return expressions.StringJoin(_xpathConcatenator);
            }

            var matches = (await browserService.GetAllMatchesAsync(currentXPath))
                .ToHashSet();

            matches.Remove(newXPath);
            return matches.StringJoin(_xpathConcatenator);
        }

        private string MakeSimilarSearch(string? xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return "";
            }

            return xpath.Split(_xpathConcatenator)
                .Select(x => MakeSimilarXPath(x))
                .StringJoin(_xpathConcatenator);
        }

        private string MakeSimilarXPath(string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return "";
            }

            var result = new StringBuilder();
            
            for (var x = 0; x < xpath.Length; x++)
            {
                var isDoubleSlash = IsXPathDoubleSlash(xpath, x);
                if (isDoubleSlash)
                {
                    x++;
                    result.Append("//");
                    continue;
                }

                if (xpath[x] == '/')
                {
                    result.Append("//");                    
                    continue;
                }

                var (isNumericSelector, length) = IsXPathNumericSelector(xpath, x);
                if (isNumericSelector)
                {
                    x += (length - 1);                    
                    continue;
                }

                result.Append(xpath[x]);
            }

            return result.ToString();
        }

        private bool IsXPathDoubleSlash(string xpath, int x)
        {
            return xpath[x] == '/' && xpath.Length > (x + 1) && xpath[x + 1] == '/';
        }

        private (bool isNumericSelector, int length) IsXPathNumericSelector(string xpath, int x)
        {
            if (xpath[x] != '[')
            {
                return default;
            }

            var index = xpath.IndexOf(']', x);

            if (index == -1)
            {
                return default;
            }

            var selector = xpath.Substring(x + 1, index - x - 1);
            var isNumeric = int.TryParse(selector, out var number);

            return isNumeric
                ? (true, selector.Length + 2)
                : default;
        }

        private string MergeExpressions(string? xpath1, string xpath2)
        {
            var xpaths = SplitExpressions(xpath1);
            xpaths.AddRange(SplitExpressions(xpath2));

            if (xpaths.Count == 0)
            {
                return "";
            }

            return xpaths
                .StringJoin(_xpathConcatenator);
        }

        private HashSet<string> SplitExpressions(string? xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return new HashSet<string>();
            }

            var items = xpath.Split(_xpathConcatenator);
            return new HashSet<string>(items);
        }
    }
}
