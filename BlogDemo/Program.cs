using BlogDemo;
using BlogDemo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;
using SilentOrbit.StaticOnline.Config;

#region StaticOnline Build Commands

var config = new BlogConfig();

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
    config.BuildConfig.Target = new DirPath("../generated/blog/");
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

#region StaticOnline

//Add first
app.BuildStaticOnline(config);

#endregion StaticOnline

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
