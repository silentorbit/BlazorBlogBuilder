using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using SilentOrbit.StaticOnline.Building;
using System.Security.Policy;

namespace SilentOrbit.StaticOnline;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Also add <see cref="MapStaticOnline(WebApplication)"/>
    /// </summary>
    public static void AddStaticOnline(this IServiceCollection services, SiteConfig config)
    {
        config.SiteBuilder ??= new SiteBuilder(config);

        services.AddSingleton<SiteBuilder>(config.SiteBuilder);
        services.AddTransient<SitePage>((provider) => TransientPage(provider, config));
    }

    static SitePage TransientPage(IServiceProvider provider, SiteConfig config)
    {
        var nm = provider.GetService<NavigationManager>()!;
        var path = new Uri(nm.Uri).AbsolutePath;
        var url = config.BaseURL.Append(path.TrimEnd('/'));

        var page = config.SiteBuilder.Pages.GetOrCreate(url);
        Debug.Assert(page != null);

        return page;
    }

    /// <summary>
    /// Place before
    /// <see cref="RazorComponentsEndpointRouteBuilderExtensions.MapRazorComponents"/>
    /// 
    /// Also add <see cref="AddStaticOnline(IServiceCollection, SiteBuilder)"/>
    /// </summary>
    public static void BuildStaticOnline(this WebApplication app, SiteConfig config)
    {
        var fr = new FileBuilder(config.SiteBuilder);
        foreach (var file in fr.GetGenerators())
        {
            app.MapGet(file.URL.Href, file.Generate);
        }

        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StaticOnline");
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            await config.SiteBuilder.Build(app);
            logger.LogInformation("Generation complete");

            if (config.ExitAfterBuildComplete)
                lifetime.StopApplication();
        });
    }
}
