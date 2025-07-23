using BlazorDemo;
using BlazorDemo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;
using SilentOrbit.StaticOnline.Config;

#region StaticOnline

var config = new BlazorDemoConfig();

if (args.FirstOrDefault() == "build" && args.Length == 2)
{
    //Building from a GitHub Action
    config.BuildConfig.Target = new DirPath(args[1]);
    Console.WriteLine($"Target: {args[1]}");
    Console.WriteLine($"Target: {config.BuildConfig.Target}");
    config.BuildConfig.AfterBuild = AfterBuildConfig.Exit;
}
else
{
    //Generation target and options
    config.BuildConfig.Target = new DirPath("../generated/demo/");
    config.BuildConfig.AfterBuild = AfterBuildConfig.LaunchBrowser;
}

#endregion StaticOnline

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

#region StaticOnline

builder.Services.AddStaticOnline(config);

#endregion StaticOnline

var app = builder.Build();

//Required for sites not located at root.
app.UsePathBase(config.BaseURL.Href + '/');

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>();

#region Static Online

app.BuildStaticOnline(config);

#endregion

app.Run();
