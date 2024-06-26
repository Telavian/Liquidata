﻿using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Models;

namespace Liquidata.Common.Services.Interfaces;

public interface IBrowserService : IAsyncDisposable
{        
    string RootPage { get; set; }
    string BrowserId { get; set; }
    bool IsBrowserInitialized { get; set; }

    Task StartBrowserAsync();
    Task InitializeBrowserAsync();
    Task<bool> CheckForDocumentAccessAsync();
    Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime);
    Task<bool> WaitForBrowserInitializationAsync(TimeSpan waitTime);
    Task<SelectionInfo> GetSelectionInfoAsync(string xpath);
    Task<string[]> GetAllMatchesAsync(string xpath, int waitTimeMs);

    Task<bool> ExecuteJavascriptAsync(string script);
    Task<(bool success, T result)> ExecuteJavascriptAsync<T>(string script);
    Task StoreDataAsync(string name, string value, StoreType storeType);
    Task<IBrowserService> ClickOpenInNewPageAsync(string selection, ClickButton clickButton, bool isDoubleClick);
    Task<bool> CheckSelectionDisabledAsync(string selection);
    Task ClickSelectionAsync(string selection, ClickButton clickButton, bool isDoubleClick, bool isShift, bool isCtrl, bool isAlt);
    Task<string> GetVariableAsync(string name);
    Task SetVariableAsync(string name, string value);
    Task RemoveVariableAsync(string name);
    Task HoverSelectionAsync(string selection);
    Task InputToSelectionAsync(string selection, string value);
    Task KeypressToSelectionAsync(string selection, bool isShift, bool isCtrl, bool isAlt, string keypressed);
    Task ReloadPageAsync();
    Task<byte[]> GetScreenshotAsync();
    Task ScrollPageAsync(ScrollType scrollType);
    Task SolveCaptchaAsync();
}
