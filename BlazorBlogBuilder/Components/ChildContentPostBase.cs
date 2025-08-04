using Microsoft.AspNetCore.Components;

namespace SilentOrbit.BlazorBlogBuilder.Components;

/// <summary>
/// Generate a new page in the feed with the update but not a new page.
/// </summary>
public abstract class ChildContentPostBase : ChildContentBase
{
    [Parameter]
    public bool Render { get; set; }

    /// <summary>
    /// Parse <see cref="ChildContent"/> as Markdown.
    /// Overrides <see cref="SiteConfig.Markdown"/>
    /// </summary>
    [Parameter]
    public bool? Markdown { get; set; }
}
