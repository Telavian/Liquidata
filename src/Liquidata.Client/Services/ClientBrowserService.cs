using Liquidata.Client.Exceptions;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Common;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Models;
using Liquidata.Common.Services.Interfaces;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using static MudBlazor.Colors;

namespace Liquidata.Client.Services;

public class ClientBrowserService(IJSRuntime jsRuntime) : IClientBrowserService
{
    private const string LD_Document = "LD_Document";
    private const string IFrameContentDocument = "document.getElementById('liquidata_browser').contentDocument";

    private const string SelectedCSSClass = "liquidata_selected";
    private const string RelativeSelectedCSSClass = "liquidata_relative";
    private const string RelativeSelectedParentCSSClass = "liquidata_relative_parent";
    private const string SelectorHighlightCSSClass = "liquidata_selector_highlight";

    public async Task UpdateBrowserSelectionModeAsync(ActionBase? action, BrowserMode browserMode)
    {
        await Task.Yield();
        var selectionMode = "browse";

        if (action is not null && browserMode == BrowserMode.Select)
        {
            if (action.ActionType == ActionType.Select)
            {
                selectionMode = "selection";
            }
            else if(action.ActionType == ActionType.RelativeSelect)
            {
                selectionMode = "relativeSelection";
            }
        }

        await ExecuteJavascriptAsync($"globalThis.liquidata_selection_mode = `{selectionMode}`;");
    }

    public async Task InitializeBrowserAsync()
    {
        await AddSelectionCssAsync();
        await AddXPathJsAsync();
        await AddSelectionJsAsync();
        await AddSelectionExtensionsJsAsync();
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

    public async Task HighlightSelectionsAsync(string?[] selections)
    {
        foreach (var selection in selections)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                continue;
            }

            await ExecuteJavascriptAsync($"globalThis.liquidata_highlightSelection(`{selection}`, `{SelectedCSSClass}`)");
        }
    }

    public async Task HighlightRelativeSelectionsAsync(string?[] selections)
    {
        foreach (var selection in selections)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                continue;
            }

            await ExecuteJavascriptAsync($"globalThis.liquidata_highlightSelection(`{selection}`, `{RelativeSelectedCSSClass}`)");
        }
    }

    public async Task HighlightRelativeSelectionParentAsync(string? selection)
    {
        if (string.IsNullOrWhiteSpace(selection))
        {
            return;
        }

        await ExecuteJavascriptAsync($"globalThis.liquidata_highlightSelection(`{selection}`, `{RelativeSelectedParentCSSClass}`)");
    }

    public async Task<SelectionInfo> GetSelectionInfoAsync(string xpath)
    {
        var result = await ExecuteJavascriptAsync<string>($"return globalThis.liquidata_getSelectionDetails(`{xpath}`)");

        if (!result.success || string.IsNullOrWhiteSpace(result.result))
        {
            throw new Exception("Unable to determine selection information");
        }

        var info = JsonSerializer.Deserialize<SelectionInfo>(result.result)!;
        info.Attributes = info.Attributes
            .Select(x => x
                .Replace(SelectedCSSClass, "")
                .Replace(RelativeSelectedCSSClass, "")
                .Replace(SelectorHighlightCSSClass, "")
                .Replace("  ", " "))
            .ToArray();

        return info;
    }

    public async Task<string[]> GetAllMatchesAsync(string xpath)
    {
        var result = await ExecuteJavascriptAsync<string>($"return globalThis.liquidata_getXPathMatches(`{xpath}`)");

        if (!result.success || string.IsNullOrWhiteSpace(result.result))
        {
            throw new Exception("Unable to determine xpath matches");
        }

        var matches = JsonSerializer.Deserialize<string[]>(result.result)!;
        return matches;
    }

    public async Task<bool> ExecuteJavascriptAsync(string script)
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

    public async Task<(bool success, T result)> ExecuteJavascriptAsync<T>(string script)
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

    public async Task StoreDataAsync(string name, string value, StoreType storeType)
    {
        await Task.Yield();

        var script = storeType == StoreType.Replace
            ? $"globalThis.${name} = `{value}`;"
            : $"if (!globalThis.${name}) {{ globalThis.${name} = `{value}`; }} else {{ globalThis.${name} += `{value}`; }}";

        await ExecuteJavascriptAsync(script);
    }

    public Task<IBrowserService> ClickOpenInNewPageAsync(string selection, ClickButton clickButton, bool isDoubleClick)
    {
        throw new ClientExecutionException();
    }

    public Task ClickSelectionAsync(string selection, ClickButton clickButton, bool isDoubleClick)
    {
        throw new ClientExecutionException();
    }

    public async Task SetVariableAsync(string name, string value)
    {
        await Task.Yield();

        var script = $"globalThis.${name} = `{value}`;";
        await ExecuteJavascriptAsync(script);
    }

    public async Task<string> GetVariableAsync(string name)
    {
        await Task.Yield();

        var script = $"globalThis.${name} ?? ``;";
        var (isSuccess, result) = await ExecuteJavascriptAsync<string>(script);

        return isSuccess 
            ? result 
            : string.Empty;
    }

    public async Task RemoveVariableAsync(string name)
    {
        await Task.Yield();

        var script = $"globalThis.${name} = null;";
        await ExecuteJavascriptAsync(script);
    }

    public Task HoverSelectionAsync(string selection)
    {
        throw new ClientExecutionException();
    }

    public Task InputToSelectionAsync(string selection, string value)
    {
        throw new ClientExecutionException();
    }

    public Task KeypressToSelectionAsync(string currentSelection, bool isShiftPressed, bool isCtrlPressed, bool isAltPressed, string keypressed)
    {
        throw new ClientExecutionException();
    }

    public Task ReloadPageAsync()
    {
        throw new ClientExecutionException();
    }

    public Task<byte[]> GetScreenshotAsync()
    {
        throw new ClientExecutionException();
    }

    public Task ScrollPageAsync(ScrollType scrollType)
    {
        throw new ClientExecutionException();
    }

    public Task SolveCaptchaAsync()
    {
        throw new ClientExecutionException();
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

    private async Task AddSelectionExtensionsJsAsync()
    {
        var js = await LoadSelectionExtensionsJsAsync();
        await ExecuteJavascriptAsync(js);
    }

    private Task<string> LoadSelectionCssAsync()
    {
        return LoadResourceAsync(Assembly.GetExecutingAssembly(), "Liquidata.Client.Resources.css.selection.css");
    }

    private Task<string> LoadSelectionJsAsync()
    {        
        return LoadResourceAsync(Assembly.GetExecutingAssembly(), "Liquidata.Client.Resources.javascript.selection.js");        
    }

    private Task<string> LoadXPathJsAsync()
    {
        return LoadResourceAsync(Assembly.GetExecutingAssembly(), "Liquidata.Client.Resources.javascript.xpath.js");
    }

    private Task<string> LoadSelectionExtensionsJsAsync()
    {
        return LoadResourceAsync(typeof(Project).Assembly, "Liquidata.Common.Resources.javascript.selection_extensions.js");
    }

    private async Task<string> LoadResourceAsync(Assembly assembly, string name)
    {
        using var stream = assembly
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
}
