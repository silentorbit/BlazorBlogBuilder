using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.BlazorRendering;

namespace SilentOrbit.StaticOnline.Components;

public class Summary : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    public SiteBuilder Site { get; set; } = null!;

    [Inject]
    public PageData Page { get; set; } = null!;

    /// <summary>
    /// Parse content as Markdown.
    /// Overridden by <see cref="SiteConfig.Markdown"/>.<see cref="MarkdownConfig.Summary"/>
    /// </summary>
    [Parameter]
    public bool? Markdown { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (ChildContent == null)
            return;

        Page.Summary = await new BlazorRenderer(Site, Page)
            .RenderFragment(ChildContent);

        if (Markdown ?? Site.Config.Markdown.Summary)
            Page.Summary = Components.Markdown.Transform(Page.Summary);
    }
}
