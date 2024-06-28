using Liquidata.Common;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Extensions;
using Liquidata.Common.Models;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Offscreen.Execution.Models;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Liquidata.Offscreen.Services
{
    public class PlaywrightBrowserService(ExecutionSettings settings) : IBrowserService
    {
        private const string LD_Document = "LD_Document";

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;
        
        private bool _ownsInstance;

        public string RootPage { get; set; } = "";
        public string BrowserId { get; set; } = Guid.NewGuid().ToString();
        public bool IsBrowserInitialized { get; set; } = false;

        public async ValueTask DisposeAsync()
        {
            if (_ownsInstance && _browser != null)
            {
                await _browser.DisposeAsync();
            }

            if (_ownsInstance && _playwright != null)
            {
                _playwright.Dispose();
            }
        }

        public async Task StartBrowserAsync()
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = settings.IsHeadless,
                ExecutablePath = settings.BrowserPath?.FullName ?? "",
                Proxy = CreateBrowserProxy()
            };

            _playwright = await Playwright.CreateAsync();

            if (settings.Browser == Enums.BrowserType.Chromium)
            {
                _browser = await _playwright.Chromium.LaunchAsync(launchOptions);                
            }

            if (settings.Browser == Enums.BrowserType.Firefox)
            {
                _browser = await _playwright.Firefox.LaunchAsync(launchOptions);
            }

            if (settings.Browser == Enums.BrowserType.WebKit)
            {
                _browser = await _playwright.Webkit.LaunchAsync(launchOptions);
            }

            _page = await _browser!.NewPageAsync();
            await ProcessExecutionSettingsAsync(_page);

            await _page.GotoAsync(RootPage);
            await _page.WaitForLoadStateAsync(LoadState.Load);
        }

        public async Task InitializeBrowserAsync()
        {
            if (IsBrowserInitialized)
            {
                return;
            }

            Console.WriteLine($"Initializing browser: {RootPage}");
            await AddXPathJsAsync();
            await AddSelectionJsAsync();
            await AddSelectionExtensionsJsAsync();
            IsBrowserInitialized = true;
        }

        public async Task<bool> CheckForDocumentAccessAsync()
        {
            await Task.Yield();
            return true;
        }

        public async Task<bool> CheckSelectionDisabledAsync(string selection)
        {
            await Task.Yield();
            return await _page!.Locator(selection)
                .IsDisabledAsync();
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

            var page = await _browser!.NewPageAsync();
            await ProcessExecutionSettingsAsync(_page);

            var browser = new PlaywrightBrowserService(settings)
            {
                _playwright = _playwright,
                _browser = _browser,
                _page = page,
                RootPage = link,
            };

            await browser.StartBrowserAsync();
            await browser.InitializeBrowserAsync();

            return browser;
        }

        public async Task ClickSelectionAsync(string selection, ClickButton clickButton, bool isDoubleClick, bool isShift, bool isCtrl, bool isAlt)
        {
            await Task.Yield();

            if (isDoubleClick)
            {
                await _page!.Locator(selection)
                    .DblClickAsync(new()
                    {
                        Button = Convert(clickButton),
                        Modifiers = ConvertModifiers(isShift, isCtrl, isAlt),
                    });

                return;
            }

            await _page!.Locator(selection)
                .ClickAsync(new()
                {
                    Button = Convert(clickButton),
                    Modifiers = ConvertModifiers(isShift, isCtrl, isAlt),
                });
        }

        public async Task<bool> ExecuteJavascriptAsync(string script)
        {
            try
            {
                var iife = $"(() => {{ {script} }})()";
                await _page!.EvaluateAsync("eval", iife);
                return true;
            }
            catch (Exception ex)
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
                var result = await _page!.EvaluateAsync<T>("eval", iife);
                return (true, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Script error: {ex.Message}");
                return default;
            }
        }

        public async Task<string[]> GetAllMatchesAsync(string xpath, int waitTimeMs)
        {
            var startTime = Stopwatch.StartNew();

            while (true)
            {
                var result = await ExecuteJavascriptAsync<string>($"return globalThis.liquidata_getXPathMatches(`{xpath}`)");

                if (!result.success || string.IsNullOrWhiteSpace(result.result))
                {
                    throw new Exception("Unable to determine xpath matches");
                }

                var matches = result.result.FromJson<string[]>();

                if (matches is not null && matches.Length > 0)
                {
                    return matches;
                }

                if (startTime.ElapsedMilliseconds > waitTimeMs)
                {
                    return [];
                }
                else
                {
                    await Task.Delay(250);
                    continue;
                }
            }
        }

        public async Task<byte[]> GetScreenshotAsync()
        {
            await Task.Yield();
            return await _page!.ScreenshotAsync(new() { Type = ScreenshotType.Jpeg });
        }

        public async Task<SelectionInfo> GetSelectionInfoAsync(string xpath)
        {
            await Task.Yield();

            // Should not be used in offscreen
            throw new NotImplementedException();
        }

        public async Task<string> GetVariableAsync(string name)
        {
            var script = $"globalThis.${name} ?? ``;";
            var (isSuccess, result) = await ExecuteJavascriptAsync<string>(script);

            return isSuccess
                ? result
                : string.Empty;
        }

        public async Task HoverSelectionAsync(string selection)
        {
            await Task.Yield();
            await _page!.Locator(selection)
                .HoverAsync();
        }

        public async Task InputToSelectionAsync(string selection, string value)
        {
            await Task.Yield();

            await _page!.Locator(selection)
                .FillAsync(value);
        }

        public async Task KeypressToSelectionAsync(string selection, bool isShift, bool isCtrl, bool isAlt, string keypressed)
        {
            await Task.Yield();

            var sequence = new StringBuilder();

            if (isShift)
            {
                sequence.Append("Shift+");
            }

            if (isCtrl)
            {
                sequence.Append("Control+");
            }

            if (isAlt)
            {
                sequence.Append("Alt+");
            }

            sequence.Append(keypressed);

            await _page!.Locator(selection)
                .PressAsync(keypressed);
        }

        public async Task ReloadPageAsync()
        {
            await Task.Yield();
            await _page!.ReloadAsync(new() { WaitUntil = WaitUntilState.Load });

            IsBrowserInitialized = false;
            await InitializeBrowserAsync();
        }

        public async Task RemoveVariableAsync(string name)
        {
            await Task.Yield();
            
            var script = $"globalThis.${name} = null;";
            await ExecuteJavascriptAsync(script);
        }

        public async Task ScrollPageAsync(ScrollType scrollType)
        {
            var script = scrollType == ScrollType.Bottom
                ? $"document.defaultView.scrollTo({{ left: 0, top: document.body.scrollHeight, behavior: 'smooth' }});"
                : $"document.defaultView.scrollTo({{ left: 0, top: 0, behavior: 'smooth' }});";

            var isSuccess = await ExecuteJavascriptAsync(script);

            if (!isSuccess)
            {
                throw new ExecutionException("Unable to reload page");
            }
        }

        public async Task SetVariableAsync(string name, string value)
        {
            await Task.Yield();

            var script = $"globalThis.${name} = `{value}`;";
            await ExecuteJavascriptAsync(script);
        }

        public async Task SolveCaptchaAsync()
        {
            await Task.Yield();

            // Not implemented yet
            await Task.CompletedTask;
        }

        public async Task StoreDataAsync(string name, string value, StoreType storeType)
        {
            await Task.Yield();

            var script = storeType == StoreType.Replace
                ? $"globalThis.${name} = `{value}`;"
                : $"if (!globalThis.${name}) {{ globalThis.${name} = `{value}`; }} else {{ globalThis.${name} += `{value}`; }}";

            await ExecuteJavascriptAsync(script);
        }

        public async Task<bool> WaitForBrowserInitializationAsync(TimeSpan waitTime)
        {
            var startTime = Stopwatch.StartNew();

            while (true)
            {
                if (IsBrowserInitialized)
                {
                    return true;
                }

                if (startTime.Elapsed > waitTime)
                {
                    break;
                }

                await Task.Delay(100);
            }

            Console.WriteLine("Browser was not initialized in time");
            return false;
        }

        public async Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime)
        {
            await Task.Yield();
            
            try
            {
                await _page!.WaitForLoadStateAsync(LoadState.Load, new()
                {
                    Timeout = waitTime.Milliseconds
                });
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        private async Task AddXPathJsAsync()
        {
            var js = await LoadXPathJsAsync();

            var result = await ExecuteJavascriptAsync(js);
            if (!result)
            {
                throw new ExecutionException("Unable to add xpath js");
            }
        }

        private async Task AddSelectionJsAsync()
        {
            var js = await LoadSelectionJsAsync();

            var result = await ExecuteJavascriptAsync(js);
            if (!result)
            {
                throw new ExecutionException("Unable to add selection js");
            }
        }

        private async Task AddSelectionExtensionsJsAsync()
        {
            var js = await LoadSelectionExtensionsJsAsync();

            var result = await ExecuteJavascriptAsync(js);
            if (!result)
            {
                throw new ExecutionException("Unable to add selection extensions js");
            }
        }

        private async Task<string> LoadSelectionJsAsync()
        {
            await Task.Yield();
            return await LoadResourceAsync("Liquidata.Common.Resources.javascript.selection.js");
        }

        private async Task<string> LoadXPathJsAsync()
        {
            await Task.Yield();
            return await LoadResourceAsync("Liquidata.Common.Resources.javascript.xpath.js");
        }

        private async Task<string> LoadSelectionExtensionsJsAsync()
        {
            await Task.Yield();
            return await LoadResourceAsync("Liquidata.Common.Resources.javascript.selection_extensions.js");
        }

        private async Task<string> LoadResourceAsync(string name)
        {
            var assembly = typeof(Project).Assembly;

            return (await assembly.LoadResourceAsync(name))
                .Replace(LD_Document, "document");
        }

        private MouseButton? Convert(ClickButton clickButton)
        {
            return clickButton switch
            {
                ClickButton.Left => MouseButton.Left,
                ClickButton.Middle => MouseButton.Middle,
                ClickButton.Right => MouseButton.Right,
                _ => throw new Exception($"Unknown button: {clickButton}")
            };
        }

        private IEnumerable<KeyboardModifier> ConvertModifiers(bool isShift, bool isCtrl, bool isAlt)
        {
            var modifiers = new List<KeyboardModifier>();

            if (isShift)
            {
                modifiers.Add(KeyboardModifier.Shift);
            }

            if (isCtrl)
            {
                modifiers.Add(KeyboardModifier.Control);
            }

            if (isAlt)
            {
                modifiers.Add(KeyboardModifier.Alt);
            }

            return modifiers;
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

        private Proxy? CreateBrowserProxy()
        {
            if (settings.ProxyServer is null)
            {
                return null;
            }

            return new Proxy
            {
                Server = settings.ProxyServer.ToString(),
                Username = settings.ProxyUser,
                Password = settings.ProxyPassword,
            };
        }

        private async Task ProcessExecutionSettingsAsync(IPage? page)
        {
            if (page is null)
            {
                return;
            }

            if (settings.DisableImages)
            {
                await page.RouteAsync("**/*.{png,jpg,jpeg,gif,svg,webp}", async r => await r.AbortAsync());
            }
        }
    }
}
