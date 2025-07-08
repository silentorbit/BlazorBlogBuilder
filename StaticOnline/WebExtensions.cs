using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.Building;
using System.Security.Policy;

namespace SilentOrbit.StaticOnline;

public static class WebExtensions
{
    /// <summary>
    /// Also add <see cref="MapStaticOnline(WebApplication)"/>
    /// </summary>
    public static void AddStaticOnline(this IServiceCollection services)
    {
        services.AddSingleton<SiteBuilder>(SiteBuilder.Instance);
        services.AddTransient<SitePage>(TransientPage);
    }

    static SitePage TransientPage(IServiceProvider provider)
    {
        var nm = provider.GetService<NavigationManager>()!;
        var path = new Uri(nm.Uri).AbsolutePath;
        var url = SiteBuilder.Instance.Config.BaseURL.HostURL.Append(path.TrimEnd('/'));

        var page = SiteBuilder.Instance.Pages.GetOrCreate(url);
        Debug.Assert(page != null);

        return page;
    }

    /// <summary>
    /// Place before
    /// <see cref="RazorComponentsEndpointRouteBuilderExtensions.MapRazorComponents"/>
    /// 
    /// Also add <see cref="AddStaticOnline(IServiceCollection)"/>
    /// </summary>
    public static void MapStaticOnline(this WebApplication app)
    {
        var site = SiteBuilder.Instance;
        var fr = new FileBuilder(site);
        foreach (var file in fr.GetGenerators())
        {
            app.MapGet(file.URL.Href, file.Generate);
        }
    }
}
