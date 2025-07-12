using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SilentOrbit.StaticOnline.BlazorRendering;

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

    /// <summary>
    /// Parse <see cref="ChildContent"/> as Markdown.
    /// 
    /// Overrides <see cref="SiteConfig.Markdown"/>.<see cref="MarkdownConfig.Update"/>
    /// </summary>
    [Parameter]
    public bool? Markdown { get; set; }

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
        if (ChildContent != null)
        {
            var blazor = new BlazorRenderer(Site, Page);
            update.Summary = await blazor.RenderFragment(ChildContent);
            if (Markdown ?? Site.Config.Markdown.Update)
                update.Summary = Components.Markdown.Transform(update.Summary);
        }

        update.InFeed = true;
        update.BlazorType = Page.BlazorType;
        update.Title = Title ?? Site.Config.UpdateTitle(Page);
    }

}
