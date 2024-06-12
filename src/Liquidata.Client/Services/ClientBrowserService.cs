﻿using Liquidata.Client.Exceptions;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Common;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Models;
using Liquidata.Common.Services.Interfaces;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Liquidata.Client.Services;

public class ClientBrowserService(IJSRuntime jsRuntime) : IClientBrowserService
{
    private bool _initialized = false;
    private static int _totalBrowserCount;

    private const string LD_Document = "LD_Document";
    private string IFrameContentDocument => $"document.getElementById('{BrowserId}').contentDocument";

    private const string SelectedCSSClass = "liquidata_selected";
    private const string RelativeSelectedCSSClass = "liquidata_relative";
    private const string RelativeSelectedParentCSSClass = "liquidata_relative_parent";
    private const string SelectorHighlightCSSClass = "liquidata_selector_highlight";

    public string RootPage { get; set; } = "";
    public string BrowserId { get; set; } = GlobalConstants.LDBrowser_Name;
    public bool IsBrowserInitialized { get; set; }

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
        if (_initialized)
        {
            return;
        }

        _initialized = true;
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

    public async Task<bool> WaitForBrowserInitializationAsync(TimeSpan waitTime)
    {
        var startTime = Stopwatch.StartNew();

        while (true)
        {
            await Task.Delay(100);
            if (_initialized)
            {
                return true;
            }

            if (startTime.Elapsed > waitTime)
            {
                break;
            }
        }

        Console.WriteLine("Browser was not initialized in time");
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

    public async Task<IBrowserService> ClickOpenInNewPageAsync(string selection, ClickButton clickButton, bool isDoubleClick)
    {        
        var link = await GetSelectionLinkAsync(selection);

        if (string.IsNullOrWhiteSpace(link))
        {
            // TODO: No link - How to get new page?
            await ClickSelectionAsync(selection, clickButton, isDoubleClick, false, false, false);
            return this;
        }

        var browserCount = Interlocked.Increment(ref _totalBrowserCount);
        var browser = new ClientBrowserService(jsRuntime)
        {
            RootPage = link,
            BrowserId = $"{GlobalConstants.LDBrowser_Name}{browserCount}"
        };

        return browser;
    }

    public async Task<bool> CheckSelectionDisabledAsync(string selection)
    {
        var script = $"return `{selection}`.getDisabled();";
        var (success, result) = await ExecuteJavascriptAsync<string>(script);

        if (success)
        {
            var isValid = bool.TryParse(result, out var isDisabled);
            return isValid && isDisabled;
        }

        throw new ExecutionException("Unable to determine whether selection is disabled");
    }

    public async Task ClickSelectionAsync(string selection, ClickButton clickButton, bool isDoubleClick, bool isShift, bool isCtrl, bool isAlt)
    {
        var button = clickButton switch
        {
            ClickButton.Left => 0,
            ClickButton.Middle => 1,
            ClickButton.Right => 2,
            _ => throw new Exception($"Unknown click button: {clickButton}")
        };

        var eventType = isDoubleClick
            ? "dblclick"
            : "click";

        var script = $"`{selection}`.{eventType}({button}, {isShift.ToString().ToLower()}, {isCtrl.ToString().ToLower()}, {isAlt.ToString().ToLower()});";
        var isSuccess = await ExecuteJavascriptAsync(script);

        if (!isSuccess)
        {
            throw new ExecutionException("Unable to click selection");
        }
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

    public async Task HoverSelectionAsync(string selection)
    {
        var script = $"`{selection}`.hover();";
        var success = await ExecuteJavascriptAsync(script);

        if (!success)
        {
            throw new ExecutionException("Unable to hover selection");
        }
    }

    public async Task InputToSelectionAsync(string selection, string value)
    {
        var script = $"`{selection}`.input(`{value}`);";
        var success = await ExecuteJavascriptAsync(script);

        if (!success)
        {
            throw new ExecutionException("Unable to input value");
        }
    }

    public async Task KeypressToSelectionAsync(string selection, bool isShift, bool isCtrl, bool isAlt, string keypressed)
    {
        var script = $"`{selection}`.keypress(`{keypressed}`, {isShift.ToString().ToLower()}, {isCtrl.ToString().ToLower()}, {isAlt.ToString().ToLower()});";
        var isSuccess = await ExecuteJavascriptAsync(script);

        if (!isSuccess)
        {
            throw new ExecutionException("Unable to keypress selection");
        }
    }

    public async Task ReloadPageAsync()
    {
        var script = $"{IFrameContentDocument}.defaultView.location.reload();";
        var isSuccess = await ExecuteJavascriptAsync(script);

        if (!isSuccess)
        {
            throw new ExecutionException("Unable to reload page");
        }
    }

    public async Task<byte[]> GetScreenshotAsync()
    {
        await Task.Yield();

        // TODO - html2canvas and domToImage seem to fail for testing
        return Array.Empty<byte>();
    }

    public async Task ScrollPageAsync(ScrollType scrollType)
    {
        var script = scrollType == ScrollType.Bottom
            ? $"{IFrameContentDocument}.defaultView.scrollTo({{ left: 0, top: {IFrameContentDocument}.body.scrollHeight, behavior: 'smooth' }});"
            : $"{IFrameContentDocument}.defaultView.scrollTo({{ left: 0, top: 0, behavior: 'smooth' }});";
        
        var isSuccess = await ExecuteJavascriptAsync(script);

        if (!isSuccess)
        {
            throw new ExecutionException("Unable to reload page");
        }
    }

    public Task SolveCaptchaAsync()
    {
        // Not implemented yet
        return Task.CompletedTask;
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

    private async Task<string> GetSelectionLinkAsync(string selection)
    {
        var script = $"return `{selection}`.getLink();";
        var (success, result) = await ExecuteJavascriptAsync<string>(script);

        if (!success)
        {
            throw new ExecutionException("Unable to get selection link");
        }

        var isValid = Uri.TryCreate(result, UriKind.Absolute, out var uri);
        if (isValid && uri is not null)
        {
            return uri.ToString();
        }

        script = "return location.href;";
        (success, result) = await ExecuteJavascriptAsync<string>(script);

        if (!success)
        {
            throw new ExecutionException("Unable to get selection link root");
        }

        isValid = Uri.TryCreate(result, UriKind.Absolute, out var uriRoot);
        if (isValid && uriRoot is not null)
        {
            isValid = Uri.TryCreate(uri, uriRoot, out var fullUri);

            if (isValid && fullUri is not null)
            {
                return fullUri.ToString();
            }
        }

        throw new ExecutionException("Unable to build selection link");
    }
}
