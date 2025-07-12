using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Building.BlazorRendering;

namespace SilentOrbit.StaticOnline.Building;

public class SiteBuilder
{
    ILogger logger;

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

    readonly FileBuilder fileRenderer;
    readonly BlazorIndex blazorIndex;
    readonly WWWRootBuilder wwwroot;
    readonly LinkScanner linkScanner;

    internal SiteBuilder(SiteConfig config)
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

    internal PageData TransientPage(IServiceProvider provider)
    {
        var nm = provider.GetService<NavigationManager>()!;
        var path = new Uri(nm.Uri).AbsolutePath;
        var url = Config.BaseURL.Append(path.TrimEnd('/'));

        var page = Config.Builder.Pages.GetOrCreate(url);

        //Only Blazor pages would inject a SitePage
        page.IsBlazor = true;

        return page;
    }

    private HttpClient httpClient = null!;

    public async Task Build(WebApplication app)
    {
        logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<SiteBuilder>();

        httpClient = new HttpClient()
        {
            BaseAddress = new Uri(app.Urls.First())
        };

        Config.Target.EmptyDirectory();

        if (Target == null)
            throw new Exception($"Missing target in new {nameof(SiteBuilder)}().");

        if (Config.NoGeneration)
        {
            //PreRender all Blazor pages
            //Otherwise the live pages need to be loaded twice
            await BuildBlazorPages();
        }
        else
        {
            //Static files first to be able to hash them
            wwwroot.Build();

            blazorIndex.Scan();

            fileRenderer.Scan();

            //Render all Blazor pages
            await BuildBlazorPages();

            //Render indexes(feeds) last
            fileRenderer.Generate();
        }

        //Change base url to work with live version
        Config.BaseURL.Replace(httpClient.BaseAddress);
    }

    async Task BuildBlazorPages()
    {
        while (Pages.Next(out var page, out var finalBuild))
        {
            var prefix = finalBuild ? "Build" : "PreScan";
            logger.LogInformation($"{prefix}: {page.Href}");

            var originalURL = page.URL;
            string html = null!;

            //PreScan
            if (finalBuild == false)
            {
                if (page.BlazorType == null)
                    await RenderPage(page);
                else
                    await new BlazorRenderer(this, page).RenderComponent(); //Only render the component
            }
            else
            {
                //BuildFinal
                html = await RenderPage(page);
            }

            if (page.InFeed && !page.IsDraftOrNotPublished && page.URL == page.BlogPostRandomURL)
                page.URL = Config.PostURL(page);

            if (page.URL != originalURL)
                Pages.UpdatePageUrl(oldURL: originalURL, page);

            if (page.IsDraftOrNotPublished)
            {
                Pages.RemoveDraft(page);
                continue;
            }

            if (finalBuild == false)
            {
                Pages.DonePreScan(page);
            }
            else
            {
                //BuildFinal

                if (page.IsBlazor)
                    html = HtmlCleanup.Clean(html);
                //Don't cleanup css,js...

                linkScanner.Scan(html);

                Target.Store(page.URL, html);

                Pages.DoneFinalBuild(page);
            }
        }
    }

    async Task<string> RenderPage(PageData page)
    {
        var url = "/" + Config.BaseURL.GetRelativePath(page.URL);
        Debug.Assert(url != null);
        try
        {
            return await httpClient.GetStringAsync(url, CancellationToken.None);
        }
        catch (HttpRequestException ex)
        {
            logger.LogCritical(ex, page.URL.Href);
            Pages.FailBlazor(page);
            throw;
        }
    }
}
