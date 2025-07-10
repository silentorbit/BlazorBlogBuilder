using Demo.Components;
using SilentOrbit.StaticOnline;

namespace Demo;

public class Program
{
    public static int Main(string[] args)
    {
        var config = new DemoSiteConfig();

        //Static Online: Run before staring web server to generate index of pages
        var site = config.InitLive();

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

        if (ProgramStaticOnline.BuildMain(args, config, out int exitCode))
            return exitCode;

        app.Run();
        return 0;
    }
}
