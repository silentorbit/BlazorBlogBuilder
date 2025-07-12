namespace SilentOrbit.StaticOnline.Config.Data;

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
            BlogPost = value;
            Update = value;
            Summary = value;
        }
    }

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
}
