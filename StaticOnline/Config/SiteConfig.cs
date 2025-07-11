using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Config;

public class SiteConfig<App> : SiteConfig
    where App : IComponent
{
    public SiteConfig()
    {
        AppType = typeof(App);
    }
}

/// <summary>
/// Create a config using <see cref="SiteConfig{App}"/>.
/// </summary>
public abstract class SiteConfig
{
    /// <summary>
    /// Helper to make it easier by only having to pass <see cref="SiteConfig"/> and not the builder as well.
    /// </summary>
    internal SiteBuilder SiteBuilder { get; set; } = null!;

    /// <summary>
    /// Path to generated files.
    /// </summary>
    public DirPath Target { get; set; } = null!;

    /// <summary>
    /// Don't generate static files, only run web server.
    /// Default: false; - Will generate files
    /// </summary>
    public bool NoGeneration { get; set; }
    public bool ExitAfterBuildComplete { get; set; }

    /// <summary>
    /// Root component type for the Blazor application.
    /// Use <see cref="SiteConfig{App}"/> to specify the Blazor component type.
    /// </summary>
    public Type AppType { get; set; } = null!;
    /// <summary>
    /// Specify explicitly when <see cref="AppType"/> is not a dll next to the wwwroot folder.
    /// </summary>
    public DirPath WwwRoot { get; set; } = null!;
    public Url BaseURL { get; set; } = null!;
    public Favicon? Favicon { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

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
    /// Default title for updates generates with <see cref="Components.Update"/>
    /// </summary>
    public virtual string UpdateTitle(PageData page) => $"Update {page.Title}";

    /// <summary>
    /// Use <see cref="SiteConfig{App}"/> to create an instance
    /// </summary>
    private protected SiteConfig()
    {
    }
}