using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Building.BlazorRendering;
using SilentOrbit.StaticOnline.Tools;
using System.Security.AccessControl;

namespace SilentOrbit.StaticOnline.Building;

public class SiteBuilder
{
    internal static SiteBuilder Instance { get; private set; } = null!;

    /// <summary>
    /// True: is running as a live Blazor website.
    /// False: is generating static files.
    /// </summary>
    public bool IsLive { get; }

    public SiteConfig Config { get; }
    public TagBuilder Tags { get; }
    public FeedList Feed { get; } = new();
    public PageTracker Pages { get; }

    /// <summary>
    /// All urls to build.
    /// May be added more as the generated pages link to more urls.
    /// </summary>
    internal Target Target { get; }
    internal Hasher Hasher { get; }
    internal BlazorRenderer Blazor { get; }
    internal HttpClient HttpClient { get; private set; }

    readonly FileBuilder fileRenderer;
    readonly BlazorIndex blazorIndex;
    readonly WWWRootBuilder wwwroot;
    readonly LinkScanner linkScanner;

    public SiteBuilder(SiteConfig config)
    {
        if (Instance != null)
            throw new Exception($"{nameof(SiteBuilder)} instance already created.");
        Instance = this;

        config.WwwRoot ??= FindWwwRoot(config);

        //public
        Config = config;
        Tags = new(this);
        Pages = new(this);

        //internal
        Target = new(this);
        Hasher = new();
        Blazor = new(this);

        //private
        fileRenderer = new FileBuilder(this);
        blazorIndex = new(this);
        wwwroot = new(this);
        linkScanner = new(this);
    }

    static DirPath FindWwwRoot(SiteConfig config)
    {
        var asmPath = new FilePath(config.AppType.Assembly.Location);

        var dir = asmPath.Parent.CombineDir("wwwroot");
        if (dir.Exists())
        {
            Console.WriteLine($"Found {dir} next to {asmPath}");
            return dir;
        }

        //Try to find wwwroot when running in Visual Studio
        dir = asmPath.Parent;
        if (dir.Path.EndsWith(@"\bin\Debug\net9.0") ||
            dir.Path.EndsWith(@"\bin\Release\net9.0"))
        {
            dir = dir.Parent.Parent.Parent.CombineDir("wwwroot");
            if (dir.Exists())
            {
                Console.WriteLine($"Found {dir}\n   in project root above {asmPath}");
                return dir;
            }
        }

        Debug.Fail($"Failed to find wwwroot near\n   {asmPath}");
        Console.Error.WriteLine($"Failed to find wwwroot near:\n   {asmPath}");
        Console.Error.WriteLine($"You must configure {nameof(SiteConfig.WwwRoot)} in code.");
        throw new ArgumentException("Missing wwwroot path in config.");
    }


    public void AddPage(Url url)
    {
        Pages.AddLink(url);
    }

    /// <summary>
    /// Only used in live mode, where <see cref="Build"/> is not called.
    /// <see cref="Build"/> will 
    /// </summary>
    public async Task PreScan()
    {
        if (Target != null)
            throw new Exception($"No need to call {nameof(PreScan)}, call {nameof(Build)} directly.");

        Scan();

        await BuildBlazorPages(onlyPrescan: true);
    }

    public async Task Build(WebApplication app)
    {
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(app.Urls.First())
        };

        Config.Target.EmptyDirectory();

        if (Target == null)
            throw new Exception($"Missing target in new {nameof(SiteBuilder)}().");

        //Static files first to be able to hash them
        wwwroot.Build();

        Scan();

        //Render all Blazor pages
        await BuildBlazorPages(onlyPrescan: false);

        //Render indexes(feeds) last
        fileRenderer.Generate();

        //Change base url to work with live version
        Config.BaseURL = HttpClient.BaseAddress;
    }

    void Scan()
    {
        blazorIndex.Scan();

        fileRenderer.Scan();
    }

    async Task BuildBlazorPages(bool onlyPrescan)
    {
        while (true)
        {
            //Prescan
            var page = Pages.NextPreScan();
            if (page != null)
            {
                var url = page.URL;

                if (page.BlazorType == null)
                {
                    //Render full url
                    await Blazor.Build(page, presScan: true);
                }
                else
                {
                    //Only render the page if known
                    await Blazor.RenderComponent(page.BlazorType!, page);
                }

                if (page.URL != url)
                    Pages.UpdatePageUrl(oldURL: url, page);

                Pages.DonePreScan(page);
                //Continue with Prescan until empty
                continue;
            }

            if (onlyPrescan)
                return;

            //Build
            page = Pages.NextFinalBuild();
            if (page != null)
            {
                var originalUrl = page.URL; //Track change

                var html = await Blazor.Build(page, presScan: false);
                if (html == null)
                {
                    Pages.FailBlazor(page);
                    continue;
                }

                if (page.URL != originalUrl)
                    Pages.UpdatePageUrl(originalUrl, page);

                html = Hasher.RewriteHTML(html);

                linkScanner.Scan(html);

                Target.Store(page.URL, html);
                Pages.DoneFinalBuild(page);

                continue;
            }

            break;
        }
    }
}
