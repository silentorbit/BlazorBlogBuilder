using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.Building;
using System.Security.Policy;

namespace SilentOrbit.StaticOnline;

public static class WebExtensions
{
    /// <summary>
    /// Also add <see cref="MapStaticOnline(WebApplication)"/>
    /// </summary>
    public static void AddStaticOnline(this IServiceCollection services, SiteBuilder site)
    {
        services.AddSingleton<SiteBuilder>(site);
        services.AddTransient<SitePage>((provider) => TransientPage(provider, site));
    }

    static SitePage TransientPage(IServiceProvider provider, SiteBuilder site)
    {
        var nm = provider.GetService<NavigationManager>()!;
        var path = new Uri(nm.Uri).AbsolutePath;
        var url = site.Config.BaseURL.HostURL.Append(path.TrimEnd('/'));

        var page = site.Pages.GetOrCreate(url);
        Debug.Assert(page != null);

        return page;
    }

    /// <summary>
    /// Place before
    /// <see cref="RazorComponentsEndpointRouteBuilderExtensions.MapRazorComponents"/>
    /// 
    /// Also add <see cref="AddStaticOnline(IServiceCollection)"/>
    /// </summary>
    public static void MapStaticOnline(this WebApplication app, SiteBuilder site)
    {
        var fr = new FileBuilder(site);
        foreach (var file in fr.GetGenerators())
        {
            app.MapGet(file.URL.Href, file.Generate);
        }
    }
}
