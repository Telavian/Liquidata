using Blazored.LocalStorage;
using CurrieTechnologies.Razor.Clipboard;
using Liquidata.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddClipboard();
builder.Services.AddBlazoredLocalStorage(o =>
{
    o.JsonSerializerOptions = new JsonSerializerOptions()
    {
        ReferenceHandler = ReferenceHandler.Preserve,        
    };

    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

await builder.Build().RunAsync();
