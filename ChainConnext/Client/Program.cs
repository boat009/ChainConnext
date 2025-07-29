using Append.Blazor.Printing;
using Blazored.LocalStorage;
using ChainConnext.Client;
using ChainConnext.Client.AuthProviders;
using ChainConnext.Client.Services;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OfficeOpenXml;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Uri u = new Uri(builder.HostEnvironment.BaseAddress);
ShareValues.CurrentURL = builder.HostEnvironment.BaseAddress;

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRadzenComponents();
//builder.Services.AddScoped<DialogService>();
//builder.Services.AddScoped<NotificationService>();
//builder.Services.AddScoped<TooltipService>();
//builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<IPrintingService, PrintingService>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddSingleton<WindowDimension>();
builder.Services.AddSingleton<BaseShared>();

builder.Services.AddBlazoredLocalStorage();


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

await builder.Build().RunAsync();
