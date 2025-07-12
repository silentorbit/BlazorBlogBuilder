using Microsoft.AspNetCore.Components;

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
        var raw = await Site.Blazor.RenderFragment(ChildContent);
        if (raw == null)
            return;

        Page.Summary = (MarkupString)raw;

        if (Markdown ?? Site.Config.Markdown.Summary)
            Page.Summary = Components.Markdown.Transform(Page.Summary);
    }
}
