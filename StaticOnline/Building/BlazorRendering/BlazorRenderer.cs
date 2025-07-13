using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SilentOrbit.StaticOnline.Building.BlazorRendering;
using System;

namespace SilentOrbit.StaticOnline.BlazorRendering;

partial class BlazorRenderer
{
    readonly SiteBuilder site;
    readonly PageData page;
    readonly StaticNavigation nav;

    public BlazorRenderer(SiteBuilder site, PageData page)
    {
        this.site = site;
        this.page = page;
        nav = new StaticNavigation(site.Config.BaseURL);
    }

    HtmlRenderer CreateHtmlRenderer()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCompactConsoleLogger();
        services.AddSingleton<NavigationManager>(nav);
        services.AddSingleton<INavigationInterception>(nav);
        services.AddSingleton<IScrollToLocationHash>(nav);
        services.AddSingleton<IJSRuntime>(new StaticJsRuntime());
        services.AddSingleton<SiteConfig>(site.Config);
        services.AddSingleton<SiteBuilder>(site);
        services.AddSingleton<PageData>(page);

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        return new HtmlRenderer(serviceProvider, loggerFactory);
    }

    /// <summary>
    /// Running the component once to update its <see cref="PageData"/>
    /// </summary>
    public async Task RenderComponent()
    {
        await using var htmlRenderer = CreateHtmlRenderer();
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(page.BlazorType!);
            return output.ToHtmlString();
        });
    }

    /// <summary>
    /// Get HTML content of <paramref name="fragment"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(fragment))]
    public async Task<MarkupString> RenderFragment(RenderFragment fragment)
    {
        await using var htmlRenderer = CreateHtmlRenderer();
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dic = new Dictionary<string, object?>
            {
                { nameof(RenderFragmentComponent.Fragment), fragment }
            };
            var parameters = ParameterView.FromDictionary(dic);

            var output = await htmlRenderer.RenderComponentAsync<RenderFragmentComponent>(parameters);

            return output.ToHtmlString();
        });

        return (MarkupString)html;
    }

}
