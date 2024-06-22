using Liquidata.Client.Services;
using Liquidata.Common.Extensions;
using Liquidata.Common.Services;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Offscreen.Enums;
using Liquidata.Offscreen.Execution.Models;
using Liquidata.Offscreen.Services;
using System.Text.Json;
using System.Xml;

namespace Liquidata.Offscreen.Execution
{
    public class ProjectExecutor
    {
        public async Task ExecuteAsync(ExecutionSettings settings)
        {
            var concurrency = settings.Concurrency ?? settings.Project.Concurrency;

            if (concurrency <= 0)
            {
                concurrency = 1;
            }

            await using var browser = new PlaywrightBrowserService(settings);
            var dataHandler = new DataHandlerService();
            var xPathProcessor = new XPathProcessorService(browser);

            await browser.StartBrowserAsync();
            await browser.InitializeBrowserAsync();

            var executionService = new ExecutionService
            {
                Project = settings.Project,
                Concurrency = concurrency,
                Browser = browser,
                DataHandler = dataHandler,
                XPathProcessor = xPathProcessor,
                BrowserPageAddedAction = async b =>
                {
                    await Task.Yield();
                    Console.WriteLine($"New page added: {b.RootPage}");
                },
                BrowserPageRemovedAction = async b =>
                {
                    await Task.Yield();
                    Console.WriteLine($"Page removed: {b.RootPage}");
                },
            };

            await settings.Project.ExecuteProjectAsync(executionService);
            var results = dataHandler.GetExecutionResults();            

            if (settings.OutputLocation != null)
            {                
                using var outStream = settings.OutputLocation.OpenWrite();
                using var output = new StreamWriter(outStream);

                var json = results.ToJson();
                await output.WriteAsync(json);
            }

            if (settings.PrintOutput)
            {
                var json = results.ToJson(true);
                Console.WriteLine("Execution results:");
                Console.WriteLine(json);
            }
        }
    }
}
