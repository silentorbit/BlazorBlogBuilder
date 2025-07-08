using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.Building;

namespace SilentOrbit.StaticOnline.Components;

public class Summary : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    public SiteBuilder Site { get; set; } = null!;

    [Inject]
    public SitePage Page { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var raw = await Site.Blazor.RenderFragment(ChildContent);
        Page.SummaryHtml = raw;
    }
}
