using Liquidata.Client.Services;
using Liquidata.Common.Extensions;
using Liquidata.Common.Services;
using Liquidata.Offscreen.Execution.Models;
using Liquidata.Offscreen.Services;

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

            if (string.IsNullOrWhiteSpace(settings.Project.Url))
            {
                throw new Exception("Project url is not defined");
            }

            await using var browser = new PlaywrightBrowserService(settings);
            browser.RootPage = settings.Project.Url;

            var dataHandler = new DataHandlerService();
            var xPathProcessor = new XPathProcessorService(browser);

            Console.WriteLine("Starting browser");
            await browser.StartBrowserAsync();

            Console.WriteLine("Initializing browser");
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
