using Microsoft.AspNetCore.Components;
using SilentOrbit.BlazorBlogBuilder.BlazorRendering;

namespace SilentOrbit.BlazorBlogBuilder.Components;

/// <summary>
/// Shared codebase with <see cref="Update"/>, <see cref="Summary"/> and <see cref="Markdown"/>
/// Code that generates from the <see cref="ChildContent"/>
/// </summary>
public abstract class ChildContentBase : ComponentBase
{
    [EditorRequired]
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional,
    /// Specify the page to apply the update.
    /// 
    /// Priority: 1
    /// </summary>
    [Parameter]
    public PageData? Page { get; set; } = null!;

    /// <summary>
    /// Find the page from the post being rendered
    /// 
    /// Priority: 2
    /// </summary>
    [CascadingParameter(Name = PageRender.CascadingPageName)]
    private PageData? PageCascading { get; set; }

    /// <summary>
    /// Use top page being rendered.
    /// To allow updates on any page.
    /// 
    /// Priority: 3
    /// </summary>
    [Inject]
    private PageData PageInjected { get; set; } = null!;

    [Inject]
    private SiteBuilder Builder { get; set; } = null!;

    [Inject]
    private SiteConfig Config { get; set; } = null!;

    protected SiteConfig siteConfig => Config ?? Builder.Config;
    protected SiteBuilder siteBuilder => Builder ?? Config.SiteBuilder;

    protected abstract Task OnChildContentParametersSetAsync(PageData page);

    protected PageData pageData = null!;

    protected bool OnlyFinalBuild = true;

    protected override sealed async Task OnParametersSetAsync()
    {
        pageData = Page ?? PageCascading ?? PageInjected;

        if (OnlyFinalBuild && pageData.BuildStage != BuildStage.FinalBuild)
            return;
        if (pageData.IsDraftOrNotPublished)
            return;

        await OnChildContentParametersSetAsync(pageData);
    }

    protected virtual async Task<MarkupString?> GetChildContent()
    {
        if (ChildContent == null)
            return null;

        var page = Page ?? PageCascading ?? PageInjected;

        var blazor = new BlazorRenderer(Builder ?? Config.SiteBuilder, page);
        MarkupString? c = await blazor.RenderFragment(ChildContent);
        return c;
    }
}
