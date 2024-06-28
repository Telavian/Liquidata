using Liquidata.Offscreen.Enums;
using Liquidata.Offscreen.Execution.Models;
using Liquidata.Common.Extensions;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Liquidata.Offscreen.Execution;
using Liquidata.Common.Exceptions;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Liquidata.Offscreen command line data extractor");

        var projectOption = new Option<FileInfo>("--project", "Path to the JSON project file")
            .LegalFilePathsOnly();
        projectOption.AddAlias("-p");
        projectOption.IsRequired = true;
        rootCommand.AddOption(projectOption);

        var browserOption = new Option<BrowserType>("--browser", "Browser to use for extraction");
        browserOption.AddAlias("-b");
        browserOption.SetDefaultValue(BrowserType.Chromium);
        browserOption.IsRequired = false;
        rootCommand.AddOption(browserOption);

        var browserPathOption = new Option<FileInfo>("--browser-path", "Path to the browser executable")
            .LegalFilePathsOnly();
        browserPathOption.AddAlias("-bp");
        browserPathOption.IsRequired = false;
        rootCommand.AddOption(browserPathOption);

        var disableImagesOption = new Option<bool>("--disable-images", "Disables the loading of images");            
        disableImagesOption.AddAlias("-di");
        disableImagesOption.IsRequired = false;
        rootCommand.AddOption(disableImagesOption);

        var proxyServerOption = new Option<Uri>("--proxy-server", "Address of proxy to use in project execution. Example: https://domain.com:1234/");
        proxyServerOption.AddAlias("-ps");
        proxyServerOption.IsRequired = false;        
        rootCommand.AddOption(proxyServerOption);

        var proxyUserOption = new Option<string>("--proxy-user", "Username to use for the browser proxy");
        proxyUserOption.AddAlias("-pu");
        proxyUserOption.IsRequired = false;
        rootCommand.AddOption(proxyUserOption);

        var proxyPasswordOption = new Option<string>("--proxy-password", "Password to use for the browser proxy");
        proxyPasswordOption.AddAlias("-pp");
        proxyPasswordOption.IsRequired = false;
        rootCommand.AddOption(proxyPasswordOption);

        var concurrencyOption = new Option<int?>("--concurrency", "Concurrency of the extraction operation. Defaults to value in project file");
        concurrencyOption.AddAlias("-c");
        concurrencyOption.AddValidator(result =>
        {
            if (result.GetValueForOption(concurrencyOption) < 1)
            {
                result.ErrorMessage = "Concurrency must be greater than 0";
            }
        });
        concurrencyOption.IsRequired = false;
        rootCommand.AddOption(concurrencyOption);

        var outputOption = new Option<FileInfo>("--output", "Path to save the JSON execution results")
            .LegalFilePathsOnly();
        outputOption.AddAlias("-o");
        outputOption.IsRequired = false;
        rootCommand.AddOption(outputOption);

        var printOutputOption = new Option<bool>("--print-output", "Indicates whether to print the output");
        printOutputOption.AddAlias("-po");
        printOutputOption.IsRequired = false;
        rootCommand.AddOption(printOutputOption);

        var headlessOption = new Option<bool>("--headless", "Indicates whether browser instance is headless");
        headlessOption.AddAlias("-h");
        headlessOption.IsRequired = false;
        rootCommand.AddOption(headlessOption);

        rootCommand.SetHandler(async c =>
        {
            ExecutionSettings executionSettings;

            try
            {
                executionSettings = await BuildExecutionSettingsAsync(c);
            }
            catch (Exception ex)
            {
                throw new ExecutionException($"Unable to build execution settings.\r\n{ex.Message}");
            }

            try
            {
                var executor = new ProjectExecutor();
                await executor.ExecuteAsync(executionSettings);
            }
            catch (Exception ex)
            {
                throw new ExecutionException($"Unable to execute project.\r\n{ex.Message}");
            }
        });

        try
        {
            return await rootCommand.InvokeAsync(args);
        }
        catch
        {
            Console.WriteLine("Unable to execute project");
            return 1;
        }

        async Task<ExecutionSettings> BuildExecutionSettingsAsync(InvocationContext c)
        {
            var projectFile = c.ParseResult.GetValueForOption(projectOption);

            return new ExecutionSettings
            {
                Project = await projectFile.LoadProjectAsync(),
                Browser = c.ParseResult.GetValueForOption(browserOption),
                BrowserPath = c.ParseResult.GetValueForOption(browserPathOption),
                Concurrency = c.ParseResult.GetValueForOption(concurrencyOption),
                OutputLocation = c.ParseResult.GetValueForOption(outputOption),
                PrintOutput = c.ParseResult.GetValueForOption(printOutputOption),
                IsHeadless = c.ParseResult.GetValueForOption(headlessOption),
                DisableImages = c.ParseResult.GetValueForOption(disableImagesOption),
                ProxyServer = c.ParseResult.GetValueForOption(proxyServerOption),
                ProxyUser = c.ParseResult.GetValueForOption(proxyUserOption),
                ProxyPassword = c.ParseResult.GetValueForOption(proxyPasswordOption),
            };
        }
    }
}