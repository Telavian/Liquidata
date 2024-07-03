using Blazor.BrowserExtension;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Liquidata.BrowserExtension;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Task.Yield();
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.UseBrowserExtension(browserExtension =>
        {
            if (browserExtension.Mode == BrowserExtensionMode.Background)
            {
                builder.RootComponents.AddBackgroundWorker<BackgroundWorker>();
            }
            else
            {
                builder.RootComponents.Add<App>("#app");
                builder.RootComponents.Add<HeadOutlet>("head::after");
            }
        });

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        await builder.Build().RunAsync();
    }
}
