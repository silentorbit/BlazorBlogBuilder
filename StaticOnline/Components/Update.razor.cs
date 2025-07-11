using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SilentOrbit.StaticOnline.Components;

/// <summary>
/// Generate a new page in the feed with the update but not a new page.
/// </summary>
public partial class Update
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [EditorRequired]
    [Parameter]
    public string Date { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool Render { get; set; }

    string anchorName = null!;

    protected override async Task OnParametersSetAsync()
    {
        if (Page.BuildStage != BuildStage.FinalBuild)
            return;
        if (Page.IsDraftOrNotPublished)
            return;

        var timestamp = (Timestamp)Date;

        if (timestamp.DateTime.TimeOfDay.Ticks == 0)
            anchorName = timestamp.ToString("yyyy-MM-dd");
        else
            anchorName = timestamp.ToString("yyyy-MM-dd'T'HH:mm");

        var url = Page.URL.Append("#" + anchorName);

        var update = Site.Pages.GetOrCreate(url, build: false);
        update.IsUpdate = true;
        update.IsDraft = Page.IsDraftOrNotPublished;
        update.Published = Date;
        update.SummaryHtml = await Site.Blazor.RenderFragment(ChildContent);
        update.InFeed = true;
        update.BlazorType = Page.BlazorType;
        update.Title = Title ?? Site.Config.UpdateTitle(Page);
    }

}
