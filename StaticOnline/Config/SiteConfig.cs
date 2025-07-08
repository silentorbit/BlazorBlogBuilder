using Microsoft.AspNetCore.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;

namespace SilentOrbit.StaticOnline.Config;

public abstract class SiteConfig
{
    /// <summary>
    /// Root component type for the Blazor application.
    /// Use <see cref="SiteConfig{App}"/> to specify the Blazor component type.
    /// </summary>
    public Type Type { get; set; } = null!;
    /// <summary>
    /// Specify explicitly when <see cref="Type"/> is not a dll next to the wwwroot folder.
    /// </summary>
    public DirPath WwwRoot { get; set; } = null!;
    public DirPath TargetDir { get; set; } = null!;
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

    public virtual string UpdateTitle(SitePage page) => $"Update {page.Title}";

    private protected SiteConfig()
    {
    }

    public abstract Task Build();
}

public class SiteConfig<App> : SiteConfig
    where App : IComponent
{
    public SiteConfig()
    {
        Type = typeof(App);
    }

    /// <summary>
    /// Called before starting live Blazor website
    /// </summary>
    public void Init()
    {
        var sg = new SiteBuilder(this);
        sg.Scan();
        sg.PreScan().Wait();
    }

    public override async Task Build()
    {
        TargetDir.DeleteDir();
        TargetDir.CreateDirectory();

        var sg = new SiteBuilder(this);
        sg.Scan();
        await sg.Build();

        Console.WriteLine("Done");
    }
}
