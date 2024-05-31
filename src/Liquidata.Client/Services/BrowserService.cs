using Liquidata.Common.Actions.Enums;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reflection;

namespace Liquidata.Client.Services;

public class BrowserService(IJSRuntime jsRuntime)
{
    private const string LD_Document = "LD_Document";
    private const string IFrameContentDocument = "document.getElementById('liquidata_browser').contentDocument";

    private const string SelectedCSSClass = "liquidata_selected";
    private const string RelativeSelectedCSSClass = "liquidata_relative";

    public async Task UpdateBrowserSelectionModeAsync(BrowserMode browserMode)
    {
        await Task.Yield();
        var booleanValue = browserMode == BrowserMode.Select
            ? "true"
            : "false";

        await ExecuteJavascriptAsync($"globalThis.liquidata_is_selection_mode = {booleanValue};");
    }

    public async Task InitializeBrowserAsync()
    {
        await AddSelectionCssAsync();
        await AddXPathJsAsync();
        await AddSelectionJsAsync();
    }

    public async Task<bool> CheckIfWebSecurityEnabledAsync()
    {        
        await Task.Yield();
        var js = $@"
                if ({IFrameContentDocument} == null)
                {{
                    return true;
                }}

                return false;";

        var result = await ExecuteJavascriptAsync<bool>(js);
        return result.success && result.result;
    }

    public async Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime)
    {
        var startTime = Stopwatch.StartNew();
        await Task.Yield();
        var js = $@"
                if ({IFrameContentDocument}.readyState === 'complete')
                {{
                    return true;
                }}

                return false;";

        while (true)
        {
            var result = await ExecuteJavascriptAsync<bool>(js);
            if (result.success && result.result)
            {
                Console.WriteLine($"Browser ready in {startTime.ElapsedMilliseconds:N0} ms");
                return true;
            }

            if (startTime.Elapsed > waitTime)
            {                
                break;
            }
        }

        Console.WriteLine("Browser was not ready in time");
        return false;
    }

    public async Task ClearCurrentSelectionsAsync()
    {
        await Task.Yield();
        await ExecuteJavascriptAsync("globalThis.liquidata_removeAllSelectionHighlights()");
    }

    public async Task HighlightSelectionsAsync(string?[] directSelections)
    {
        foreach (var selection in directSelections)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                continue;
            }

            await ExecuteJavascriptAsync($"globalThis.liquidata_highlightSelection(`{selection}`, `{SelectedCSSClass}`)");
        }
    }

    public async Task HighlightRelativeSelectionsAsync(string?[] directSelections)
    {
        foreach (var selection in directSelections)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                continue;
            }

            await ExecuteJavascriptAsync($"globalThis.liquidata_highlightSelection(`{selection}`, `{RelativeSelectedCSSClass}`)");
        }
    }

    private async Task AddSelectionCssAsync()
    {
        var css = await LoadSelectionCssAsync();

        var js = $@"
                    var style = {IFrameContentDocument}.createElement('style'); 
                    style.innerHTML = `{css}`; 

                    {IFrameContentDocument}.head.appendChild(style);";

        await ExecuteJavascriptAsync(js);
    }

    private async Task AddXPathJsAsync()
    {
        var js = await LoadXPathJsAsync();
        await ExecuteJavascriptAsync(js);
    }

    private async Task AddSelectionJsAsync()
    {
        var js = await LoadSelectionJsAsync();
        await ExecuteJavascriptAsync(js);
    }

    private Task<string> LoadSelectionCssAsync()
    {
        return LoadResourceAsync("css.selection.css");
    }

    private Task<string> LoadSelectionJsAsync()
    {        
        return LoadResourceAsync("javascript.selection.js");        
    }

    private Task<string> LoadXPathJsAsync()
    {
        return LoadResourceAsync("javascript.xpath.js");
    }

    private async Task<string> LoadResourceAsync(string name)
    {
        name = $"Liquidata.Client.Resources.{name}";

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(name);

        if (stream is null)
        {
            throw new Exception($"Resource '{name}' not found");
        }

        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();

        return result
            .Replace(LD_Document, IFrameContentDocument);
    }

    private async Task<bool> ExecuteJavascriptAsync(string script)
    {        
        try
        {
            var iife = $"(() => {{ {script} }})()";            
            await jsRuntime.InvokeVoidAsync("eval", iife);
            return true;
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return false;
        }        
        catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return false;
        }
        catch (JSDisconnectedException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return false;
        }
    }

    private async Task<(bool success, T result)> ExecuteJavascriptAsync<T>(string script)
    {
        try
        {
            var iife = $"(() => {{ {script} }})()";            
            var result = await jsRuntime.InvokeAsync<T>("eval", iife);
            return (true, result);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return default;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return default;
        }
        catch (JSDisconnectedException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return default;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Script error: {ex.Message}");
            return default;
        }
    }
}
