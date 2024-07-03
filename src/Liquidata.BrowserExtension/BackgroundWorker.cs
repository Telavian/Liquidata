using Blazor.BrowserExtension;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WebExtensions.Net.DeclarativeNetRequest;

namespace Liquidata.BrowserExtension;

public partial class BackgroundWorker : BackgroundWorkerBase
{
    private static readonly string[] _domains = ["telavian.github.io", "localhost"];

    [BackgroundWorkerMain]
    public override void Main()
    {
        //Logger.LogInformation("Initializing extension");
        _ = WebExtensions.Runtime.OnInstalled.AddListener(OnInstalled);
        _ = WebExtensions.Runtime.OnStartup.AddListener(OnStartup);
    }    

    private async Task OnInstalled()
    {
        Logger.LogInformation("Extension installed");
        var indexPageUrl = await WebExtensions.Runtime.GetURL("index.html");
        await WebExtensions.Tabs.Create(new()
        {
            Url = indexPageUrl
        });

        await SubscribeAsync();
    }

    private async Task OnStartup()
    {
        await Task.Yield();
        Logger.LogInformation("Extension starting");
        await SubscribeAsync();
    }

    private async Task SubscribeAsync()
    {
        Logger.LogInformation("Subscribing events");

        var options = new UpdateSessionRulesOptions()
        {
            RemoveRuleIds = [1, 2],
            AddRules = 
            [
                new Rule
                {
                    Id = 1,
                    Condition = new Condition
                    {
                        UrlFilter = "*",
                        InitiatorDomains = _domains
                    },
                    Action = new Action
                    {
                        Type = "modifyHeaders",
                        ResponseHeaders = [new ResponseHeader { Operation = "remove", Header = "X-Frame-Options" }]
                    }
                },
                new Rule
                {
                    Id = 2,
                    Condition = new Condition
                    {
                        UrlFilter = "*",
                        InitiatorDomains = _domains
                    },
                    Action = new Action
                    {
                        Type = "modifyHeaders",
                        ResponseHeaders = [new ResponseHeader { Operation = "remove", Header = "Frame-Options" }]
                    }
                }
            ]
        };
                
        await WebExtensions.DeclarativeNetRequest.UpdateSessionRules(options);
    }
}
