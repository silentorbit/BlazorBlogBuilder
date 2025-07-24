using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Components;

/// <summary>
/// Generate a new page in the feed with the update but not a new page.
/// </summary>
public partial class Update : ChildContentPostBase
{
    [EditorRequired]
    [Parameter]
    public string Date { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [CascadingParameter(Name = PageRender.CascadingPageName)]
    private PageData? PageCascadingUpdate { get; set; }

    string anchorName = null!;

    protected override async Task OnChildContentParametersSetAsync(PageData page)
    {
        var timestamp = (Timestamp)Date;

        var update = siteBuilder.Pages.CreateUpdate(page, timestamp);
        update.Summary = await GetChildContent();
        if (Markdown ?? siteConfig.BuildConfig.Markdown.Update)
            update.Summary = Components.Markdown.Transform(update.Summary);
        if (Title != null)
            update.Title = Title;

        anchorName = update.URL.Fragment;
    }

}
