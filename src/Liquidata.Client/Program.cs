using Blazored.LocalStorage;
using CurrieTechnologies.Razor.Clipboard;
using Liquidata.Client;
using Liquidata.Client.Services;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Common.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(c =>
{
    c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});
builder.Services.AddClipboard();
builder.Services.AddBlazoredLocalStorage(o =>
{
    o.JsonSerializerOptions = new JsonSerializerOptions();
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBrowserService, ClientBrowserService>();
builder.Services.AddScoped<IClientBrowserService, ClientBrowserService>();
builder.Services.AddScoped<IXPathProcessorService, XPathProcessorService>();

await builder.Build().RunAsync();
