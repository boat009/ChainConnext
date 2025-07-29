using ChainConnext.Shared;
using Microsoft.AspNetCore.ResponseCompression;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//string SectionCont = "ConnectionStrings";

//string BaseUrl = builder.Host.ToString();
//var clientBaseUrl = builder.WebHost.UseUrls;



System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddFastReport();

//builder.Services.AddCors(opt =>
//{
//    opt.AddPolicy("NewPolicy", builder =>
//    {
//        builder.AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});
builder.Services.AddRadzenComponents();

//builder.Services.AddRazorComponents()
//    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

string SectionCont = "ConnectionStringsSit";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    //SectionCont = "ConnectionStringsSit";
    SectionCont = "ConnectionStrings";
}
else
{
    BaseShared.HostUrl = app.Environment.WebRootPath;

    if (BaseShared.HostUrl.ToLower().Contains("uat"))
    {
        SectionCont = "ConnectionStringsSit";
    }
    else
    {
        SectionCont = "ConnectionStrings";
    }
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Helpers.BaseSettup.ServerName = builder.Configuration.GetSection(SectionCont)["ServerName"];
Helpers.BaseSettup.DatabaseName = builder.Configuration.GetSection(SectionCont)["DatabaseName"];
Helpers.BaseSettup.DatabaseUserName = builder.Configuration.GetSection(SectionCont)["DatabaseUserName"];
Helpers.BaseSettup.DatabasePassword = builder.Configuration.GetSection(SectionCont)["DatabasePassword"];
Helpers.BaseSettup.ApplicationName = builder.Configuration.GetSection(SectionCont)["ApplicationName"];

Helpers.BaseSettup.ApiLogin = builder.Configuration["ApiLogin"];

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseFastReport();

app.UseRouting();
//app.UseCors("NewPolicy");
app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
            );
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

//app.UseAntiforgery();

//app.MapRazorComponents<ChainConnext.Client.App>()
//    .AddInteractiveWebAssemblyRenderMode();
Version version = typeof(Program).Assembly.GetName().Version;
BaseShared.AppVersion = version.ToString();

app.Run();
