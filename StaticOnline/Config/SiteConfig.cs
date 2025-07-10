using Microsoft.AspNetCore.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;
using System.Reflection;
using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Config;

public abstract class SiteConfig
{
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

    public Robots DefaultRobots { get; } = new();

    public Author Author { get; set; } = new();

    public Language Language { get; set; } = new()
    {
        Code = "en"
    };

    /// <summary>
    /// Default title for updates generates with <see cref="Components.Update"/>
    /// </summary>
    public virtual string UpdateTitle(SitePage page) => $"Update {page.Title}";

    /// <summary>
    /// Change the configuration to serve the site live during debugging.
    /// </summary>
    internal protected virtual void ConfigureLive() { }

    /// <summary>
    /// Use <see cref="SiteConfig{App}"/> to create an instance
    /// </summary>
    private protected SiteConfig()
    {
    }
}

public class SiteConfig<App> : SiteConfig
    where App : IComponent
{
    public SiteConfig()
    {
        AppType = typeof(App);
    }
}

