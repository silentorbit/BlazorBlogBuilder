using BlogDemo;
using BlogDemo.Components;
using SilentOrbit.Disk;
using SilentOrbit.BlazorBlogBuilder;
using SilentOrbit.BlazorBlogBuilder.Config;

#region BlazorBlogBuilder Commands

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

#endregion BlazorBlogBuilder

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

#region BlazorBlogBuilder

builder.Services.AddBlazorBlogBuilder(config);

#endregion BlazorBlogBuilder

var app = builder.Build();

#region BlazorBlogBuilder

//Add first
app.RunBlazorBlogBuilder(config);

#endregion BlazorBlogBuilder

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
