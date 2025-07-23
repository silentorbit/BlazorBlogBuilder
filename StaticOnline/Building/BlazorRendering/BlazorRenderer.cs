using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SilentOrbit.StaticOnline.Building.BlazorRendering;

namespace SilentOrbit.StaticOnline.BlazorRendering;

partial class BlazorRenderer
{
    readonly SiteBuilder builder;
    readonly PageData page;
    readonly StaticNavigation nav;

    public BlazorRenderer(SiteBuilder builder, PageData page)
    {
        this.builder = builder;
        this.page = page;
        nav = new StaticNavigation(builder.Config.BaseURL);
    }

    HtmlRenderer CreateHtmlRenderer()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCompactConsoleLogger();
        services.AddSingleton<NavigationManager>(nav);
        services.AddSingleton<INavigationInterception>(nav);
        services.AddSingleton<IScrollToLocationHash>(nav);
        services.AddSingleton<IJSRuntime>(new StaticJsRuntime());
        services.AddSingleton<SiteConfig>(builder.Config);
        services.AddSingleton<SiteBuilder>(builder);
        services.AddSingleton<PageData>(page);

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        return new HtmlRenderer(serviceProvider, loggerFactory);
    }

    /// <summary>
    /// Running the component once to update its <see cref="PageData"/>
    /// </summary>
    public async Task<string> RenderComponent()
    {
        await using var htmlRenderer = CreateHtmlRenderer();
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync(page.BlazorType!);
            return output.ToHtmlString();
        });

        return html;
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
