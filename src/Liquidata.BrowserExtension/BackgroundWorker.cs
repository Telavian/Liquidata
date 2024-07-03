using Blazor.BrowserExtension;
using System;
using System.Threading.Tasks;
using WebExtensions.Net.WebRequest;

namespace Liquidata.BrowserExtension;

public partial class BackgroundWorker : BackgroundWorkerBase
{
    private readonly string[] _domains = ["telavian.github.io", "localhost"];
    private readonly string[] _headers = ["Frame-Options"];

    [BackgroundWorkerMain]
    public override void Main()
    {
        WebExtensions.Runtime.OnInstalled.AddListener(OnInstalled);
        WebExtensions.Runtime.OnStartup.AddListener(OnStartup);
    }

    async Task OnInstalled()
    {
        var indexPageUrl = await WebExtensions.Runtime.GetURL("index.html");
        await WebExtensions.Tabs.Create(new()
        {
            Url = indexPageUrl
        });
    }    

    async Task OnStartup()
    {
        await WebExtensions.WebRequest.OnBeforeSendHeaders.AddListener(HandleOnBeforeSendHeaders);
        await WebExtensions.WebRequest.OnHeadersReceived.AddListener(HandleOnHeadersReceived);
    }    

    private BlockingResponse HandleOnBeforeSendHeaders(OnBeforeSendHeadersEventCallbackDetails details)
    {
        if (!IsUrlMatch(details.Url) && !IsUrlMatch(details.DocumentUrl) && !IsUrlMatch(details.OriginUrl))
        {
            return new();
        }

        return new BlockingResponse
        {
            RequestHeaders = FilterHeaders(details.RequestHeaders),
        };
    }

    private BlockingResponse HandleOnHeadersReceived(OnHeadersReceivedEventCallbackDetails details)
    {
        if (!IsUrlMatch(details.Url) && !IsUrlMatch(details.DocumentUrl) && !IsUrlMatch(details.OriginUrl))
        {
            return new();
        }

        return new BlockingResponse
        {
            ResponseHeaders = FilterHeaders(details.ResponseHeaders),
        };
    }

    private bool IsUrlMatch(string url)
    {
        var uri = new Uri(url, UriKind.RelativeOrAbsolute);
        if (!uri.IsAbsoluteUri)
        {
            return false;
        }

        var host = uri.Host;
        
        foreach (var domain in _domains)
        {
            if (host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private HttpHeaders FilterHeaders(HttpHeaders headers)
    {
        foreach (var header in _headers)
        {
            headers.RemoveAll(x => x.Name.Contains(header, StringComparison.OrdinalIgnoreCase));
        }
        
        return headers;
    }
}
