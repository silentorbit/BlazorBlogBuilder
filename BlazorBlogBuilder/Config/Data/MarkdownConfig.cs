namespace SilentOrbit.BlazorBlogBuilder.Config.Data;

public class MarkdownConfig
{
    public MarkdownConfig(bool enableAll = false)
    {
        All = enableAll;
    }

    /// <summary>
    /// Set all Markdown options at once.
    /// <see cref="BlogPost"/>, <see cref="Update"/>
    /// </summary>
    public bool All
    {
        set
        {
            Page = value;
            BlogPost = value;
            Update = value;
            Summary = value;
            Comment = value;
        }
    }

    /// <summary>
    /// Parse all pages using markdown by default.
    /// Only applies to pages rendered via <see cref="PageData.Render"/>
    /// Does not work automatically for pages rendered directly by Blazor pages using @page.
    /// 
    /// Overridden by <see cref="PageData.Markdown"/>
    /// </summary>
    public bool Page { get; set; }

    /// <summary>
    /// Parse blogposts using markdown by default.
    /// Only applies to pages rendered via <see cref="PageData.Render"/>
    /// 
    /// Overridden by <see cref="PageData.Markdown"/>
    /// </summary>
    public bool BlogPost { get; set; }

    /// <summary>
    /// Parse updates using markdown by default.
    /// 
    /// Overridden by <see cref="Components.Update.Markdown"/>
    /// </summary>
    public bool Update { get; set; }

    /// <summary>
    /// Parse summary using markdown by default.
    /// 
    /// Overridden by <see cref="Components.Summary.Markdown"/>
    /// </summary>
    public bool Summary { get; set; }

    /// <summary>
    /// Parse summary using markdown by default.
    /// 
    /// Overridden by <see cref="Components.Comment.Markdown"/>
    /// </summary>
    public bool Comment { get; set; }
}
