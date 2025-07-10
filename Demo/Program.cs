using Demo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;
using SilentOrbit.StaticOnline.Building;
using System.Diagnostics;
using System.Reflection;

namespace Demo;

public class Program
{
    public static void Main(string[] args)
    {
        var exeName = Path.GetFileName(Assembly.GetEntryAssembly()!.Location);
        Console.WriteLine($@"Usage: {exeName} build <output path>");

        var config = new DemoSiteConfig();

        //Static Online
        SiteBuilder site;
        if (args.Length == 0 || args[0] != "build")
        {
            site = new SiteBuilder(config, null!);
            site.PreScan().Wait();
        }
        else
        {
            var target = DirPath.GetCurrentDirectory().CombineDir(args[1]);
            Console.WriteLine($@"Target: ""{args[1]}"" ==> ""{target}""");
            target.EmptyDirectory();

            site = new SiteBuilder(config, target);
            site.Build().Wait();
            return;
        }

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents();
        //Static Online: inject SiteBuilder and SitePage
        builder.Services.AddStaticOnline(site);

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
        app.MapStaticOnline(site);

        app.MapStaticAssets();
        app.MapRazorComponents<App>();

        app.Run();
    }
}
