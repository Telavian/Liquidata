using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text;

namespace Liquidata.Client.Services
{
    public class XPathProcessorService(BrowserService browserService)
    {
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
                return expressions.StringJoin(" | ");
            }

            var matches = (await browserService.GetAllMatchesAsync(currentXPath))
                .ToHashSet();

            matches.Remove(newXPath);
            return matches.StringJoin(" | ");
        }

        private string MakeSimilarSearch(string? xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return "";
            }

            return xpath.Split(" | ")
                .Select(x => MakeSimilarXPath(x))
                .StringJoin(" | ");
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
                .StringJoin(" | ");
        }

        private HashSet<string> SplitExpressions(string? xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return new HashSet<string>();
            }

            var items = xpath.Split(" | ");
            return new HashSet<string>(items);
        }
    }
}
