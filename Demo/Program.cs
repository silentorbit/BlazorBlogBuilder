using Demo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;

namespace Demo;

public class Program
{
    public static void Main(string[] args)
    {
        //Static Online: Build entire site
        if (args.Length > 1 && args[0] == "build")
        {
            var config = new DemoSiteConfig();

            var workingDir = DirPath.GetCurrentDirectory();
            var target = workingDir.CombineDir(args[1]);

            config.Build(target).Wait();
            return;
        }

        //Static Online: Run before staring web server
        new DemoSiteConfig(localhost: true)
            .Init();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents();
        //Static Online: inject SiteBuilder and SitePage
        builder.Services.AddStaticOnline();

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
        app.MapStaticOnline();

        app.MapStaticAssets();
        app.MapRazorComponents<App>();

        app.Run();
    }
}
