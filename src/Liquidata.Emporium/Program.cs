using Blazored.LocalStorage;
using Liquidata.Emporium;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using Liquidata.Emporium.Services.Interfaces;
using Liquidata.Emporium.Services;
using CurrieTechnologies.Razor.Clipboard;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(c =>
{
    c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    c.SnackbarConfiguration.ShowTransitionDuration = 500;
    c.SnackbarConfiguration.VisibleStateDuration = 2500;
    c.SnackbarConfiguration.HideTransitionDuration = 1000;
});

builder.Services.AddClipboard();
builder.Services.AddBlazoredLocalStorage(o =>
{
    o.JsonSerializerOptions = new JsonSerializerOptions();
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.AddScoped<IEmporiumService, EmporiumService>();

await builder.Build().RunAsync();
