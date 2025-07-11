using Demo;
using Demo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;
using SilentOrbit.StaticOnline.Building;
using System.Diagnostics;

//StaticOnline
var config = new DemoSiteConfig();
//StaticOnline: For building from a GitHub Action
if (args.FirstOrDefault() == "build" && args.Length == 2)
    config.Target = DirPath.GetCurrentDirectory().CombineDir(args[1]);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
//Static Online: inject SiteBuilder and SitePage
builder.Services.AddStaticOnline(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

//Static Online: Serve generated feeds and sitemaps
app.BuildStaticOnline(config);

app.MapStaticAssets();

app.MapRazorComponents<App>();

app.Run();
