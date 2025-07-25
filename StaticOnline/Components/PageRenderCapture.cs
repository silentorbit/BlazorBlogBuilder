using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SilentOrbit.StaticOnline.BlazorRendering;

namespace SilentOrbit.StaticOnline.Components;

/// <summary>
/// Only for internal use by <see cref="PageRender"/>.
/// Captures the rendered content for each blog post by storing 
/// </summary>
public class PageRenderCapture : ComponentBase
{
    [EditorRequired]
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Optional,
    /// Specify the page to apply the update.
    /// 
    /// Priority: 1
    /// </summary>
    [EditorRequired]
    [Parameter]
    public PageData Page { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Page.BuildStage == BuildStage.FinalBuild)
        {
            Debug.Assert(Page.Full == null);
            var blazor = new BlazorRenderer(SiteBuilder.Instance, Page);
            Page.Full = blazor.RenderFragment(ChildContent).Result;
        }

        builder.AddContent(0, ChildContent);
    }
}
