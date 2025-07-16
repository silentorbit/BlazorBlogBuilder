using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Config;

public class SiteConfig<App> : SiteConfig
    where App : IComponent
{
    public SiteConfig()
    {
        BuildConfig.AppType = typeof(App);
    }
}

/// <summary>
/// Create a config using <see cref="SiteConfig{App}"/>.
/// 
/// Here is most of the configuration for the generated content.
/// See also <see cref="BuildConfig"/> for configuration of the build process.
/// </summary>
public abstract class SiteConfig
{
    /// <summary>
    /// Use <see cref="SiteConfig{App}"/> to create an instance
    /// </summary>
    private protected SiteConfig() { }

    /// <summary>
    /// Helper to make it easier by only having to pass <see cref="SiteConfig"/> and not the builder as well.
    /// </summary>
    internal SiteBuilder SiteBuilder { get; set; } = null!;

    /// <summary>
    /// Configure how the website is built
    /// </summary>
    public BuildConfig BuildConfig { get; set; } = new();

    /// <summary>
    /// The full URL where the site is built.
    /// 
    /// Most iportant is the path after the domain, 
    /// this is used to generate a site that will work in a subdirectory.
    /// 
    /// The full URL is used in links where the specification requires a full URL.
    /// </summary>
    public BaseUrl BaseURL { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Favicon? Favicon { get; set; }

    public List<MenuItem> TopMenu { get; set; } = new();

    /// <summary>
    /// Public email address to receive comments.
    /// Public: this address is shown to all visitors.
    /// </summary>
    public string? CommentEmail { get; set; }

    public Url? GitHub { get; set; }
    public Url? Twitter { get; set; }
    public Url? Facebook { get; set; }
    public Url? Instagram { get; set; }

    public Robots DefaultRobots { get; set; } = new();

    public Author Author { get; set; } = new();

    public Language Language { get; set; } = new()
    {
        Code = "en"
    };

    /// <summary>
    /// Timestamps for page publish and modified fields are parsed using this timestamp.
    /// </summary>
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;

    /// <summary>
    /// Default Markdown config.
    /// Set individually or use <see cref="MarkdownConfig.All"/> to set all at once.
    /// These default can be overriden by the Markdown property of each page or component.
    /// </summary>
    public MarkdownConfig Markdown { get; set; } = new();

    /// <summary>
    /// Default title for updates generates with &lt;<see cref="Components.Update"/>&gt;
    /// </summary>
    internal protected virtual string UpdateTitle(PageData update) => $"Update {update.Title}";

    internal protected virtual RelUrl PostURL(PageData post)
    {
        var snippet =
            post.UrlSnippet ??
            post.Title?.ToLowerInvariant().Replace(" ", "_") ??
            post.BlazorType?.Name.ToLowerInvariant();
        if (snippet == null)
            throw new Exception($"Failed to generate snippet for post {post.URL}, need one of: UrlSnippet, Title or BlazorType");

        //Fix what would be an invalid filename on Windows.
        snippet = snippet.Trim('.');

        snippet = Uri.EscapeDataString(snippet);

        if (post.Published == null)
        {
            var logger = new CompactConsoleLogger<SiteConfig>();
            var error = $"Page {post.Href} is missing a {nameof(post.Published)} date.";

            if (post.IsDraft)
                logger.LogWarning(error);
            else
                logger.LogError(error);

            return BaseURL.Append($"post/no-date/{snippet}");
        }

        return BaseURL.Append($"post/{post.Published!.ToString("yyyy-MM-dd")}/{snippet}");
    }

    internal protected virtual RelUrl TagURL(string name)
    {
        return BaseURL.Append("/tags/" + name);
    }

}