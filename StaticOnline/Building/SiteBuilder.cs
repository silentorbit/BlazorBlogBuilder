using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Building.BlazorRendering;
using SilentOrbit.StaticOnline.Tools;

namespace SilentOrbit.StaticOnline.Building;

public class SiteBuilder
{
    internal static SiteBuilder Instance { get; private set; } = null!;

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

    readonly FileBuilder fileRenderer;
    readonly BlazorIndex blazorIndex;
    readonly WWWRootBuilder wwwroot;
    readonly LinkScanner linkScanner;

    public SiteBuilder(SiteConfig config, DirPath targetDir)
    {
        Debug.Assert(Instance == null, "SiteGenerator instance already exists.");

        Instance = this;

        //public
        Config = config;
        Tags = new(this);
        Pages = new(this);

        //internal
        Target = new(this, targetDir);
        Hasher = new();
        Blazor = new(this);

        //private
        fileRenderer = new FileBuilder(this);
        blazorIndex = new(this);
        wwwroot = new(this, targetDir);
        linkScanner = new(this);
    }

    public void AddPage(Url url)
    {
        Pages.AddLink(url);
    }

    internal void Scan()
    {
        blazorIndex.Scan();

        fileRenderer.Scan();
    }

    internal async Task PreScan()
    {
        await BuildBlazorPages(onlyPrescan: true);
    }

    internal async Task Build()
    {
        //Static files first to be able to hash them
        wwwroot.Build();

        //Render all Blazor pages
        await BuildBlazorPages(onlyPrescan: false);

        //Render indexes(feeds) last
        fileRenderer.Generate();
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
