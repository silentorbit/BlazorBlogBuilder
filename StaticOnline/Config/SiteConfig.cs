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
        Type = typeof(App);

        var asmPath = new FilePath(Type.Assembly.Location);

        //Find wwwroot
        WwwRoot = FindWwwRoot(asmPath)!;
        if (WwwRoot == null)
        {
            Console.Error.WriteLine($"Failed to find wwwroot next to {asmPath}");
            Console.Error.WriteLine($"You have to manually configure it in your config.{nameof(WwwRoot)}");
        }

    }

    DirPath? FindWwwRoot(FilePath asmPath)
    {
        var dir = asmPath.Parent.CombineDir("wwwroot");
        if (dir.Exists())
        {
            Console.WriteLine($"Found {dir} next to {asmPath}");
            return dir;
        }

        //Try to find wwwroot when running in Debug
        dir = asmPath.Parent;
        if (dir.Path.EndsWith(@"\bin\Debug\net9.0"))
            dir = dir.Parent.Parent.Parent.CombineDir("wwwroot");
        if (dir.Exists())
        {
            Console.WriteLine($"Found {dir} in project root above {asmPath}");
            return dir;
        }

        return null;
    }

    /// <summary>
    /// Called before starting a live Blazor website
    /// </summary>
    public SiteBuilder Init()
    {
        var sg = new SiteBuilder(this, null!);
        sg.Scan();
        sg.PreScan().Wait();
        return sg;
    }

}

