using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SilentOrbit.StaticOnline.Building;
using SilentOrbit.StaticOnline.Building.BlazorRendering;
using SilentOrbit.StaticOnline.Tools;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SilentOrbit.StaticOnline.BlazorRendering;

partial class BlazorRenderer
{
    readonly SiteBuilder site;

    readonly StaticNavigation nav;
    readonly IServiceProvider serviceProvider;
    readonly ILoggerFactory loggerFactory;

    public BlazorRenderer(SiteBuilder site)
    {
        this.site = site;

        nav = new StaticNavigation(site.Config.BaseURL);

        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<NavigationManager>(nav);
        services.AddSingleton<INavigationInterception>(nav);
        services.AddSingleton<IScrollToLocationHash>(nav);
        services.AddSingleton<IJSRuntime>(new StaticJsRuntime());
        services.AddSingleton<SiteBuilder>(SiteBuilder.Instance);
        services.AddTransient<SitePage>(provider => buildPage ?? throw new NullReferenceException());

        serviceProvider = services.BuildServiceProvider();
        loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    }

    SitePage? buildPage = null;

    public async Task<string?> Build(SitePage page, bool presScan)
    {
        buildPage = page;

        var relURL = '/' + page.URL.GetRelativePath(site.Config.BaseURL).TrimStart('/');
        nav.NavigateToRelative(relURL);

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(site.Config.Type);
            if (presScan)
                return null;

            return output.ToHtmlString();
        });

        if (page.NotFound)
            return null;

        if (html == null)
            return null;

        html = HtmlCleanup.Clean(html);

        return html;
    }

    /// <summary>
    /// Running the component once to update its <see cref="SitePage"/>
    /// </summary>
    public async Task RenderComponent(Type componentType, SitePage page)
    {
        buildPage = page;

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(componentType);
            return output.ToHtmlString();
        });
        Debug.WriteLine(html);
    }

    /// <summary>
    /// Get HTML content of <paramref name="fragment"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(fragment))]
    public async Task<string?> RenderFragment(RenderFragment? fragment)
    {
        if (fragment == null)
            return null;

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dic = new Dictionary<string, object?>
            {
                {
                    nameof(RenderFragmentComponent.Fragment),
                    fragment
                }
            };
            var parameters = ParameterView.FromDictionary(dic);

            var output = await htmlRenderer.RenderComponentAsync<RenderFragmentComponent>(parameters);

            return output.ToHtmlString();
        });
        return html;
    }
}
