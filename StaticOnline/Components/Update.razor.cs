using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SilentOrbit.StaticOnline.BlazorRendering;

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

    protected string anchorName = null!;

    [CascadingParameter(Name = PageRender.CascadingPageName)]
    private PageData? PageCascadingUpdate { get; set; }

    protected override async Task OnChildContentParametersSetAsync(PageData page)
    {
        var timestamp = (Timestamp)Date;

        if (timestamp.DateTime.TimeOfDay.Ticks == 0)
            anchorName = timestamp.ToString("yyyy-MM-dd");
        else
            anchorName = timestamp.ToString("yyyy-MM-dd'T'HH:mm");

        var url = page.URL.Append("#" + anchorName);

        if (page.Modified < timestamp ?? true)
            page.Modified = timestamp;

        var update = Site.Pages.GetOrCreate(url, build: false);
        update.IsUpdate = true;
        update.IsDraft = page.IsDraftOrNotPublished;
        update.Published = Date;
        update.Modified = Date;
        update.Summary = await GetChildContent();
        if (Markdown ?? Site.Config.Markdown.Update)
            update.Summary = Components.Markdown.Transform(update.Summary);

        update.InFeed = true;
        //update.BlazorType = BlogPost.BlazorType;
        update.Title = Title ?? Site.Config.UpdateTitle(page);
    }

}
