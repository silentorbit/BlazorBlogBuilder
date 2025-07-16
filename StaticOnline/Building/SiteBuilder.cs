using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Building.BlazorRendering;

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

    static readonly ILogger logger = new CompactConsoleLogger<SiteBuilder>();
    readonly FileBuilder fileRenderer;
    readonly BlazorIndex blazorIndex;
    readonly WWWRootBuilder wwwroot;
    readonly LinkScanner linkScanner;

    internal SiteBuilder(SiteConfig config)
    {
        if (Instance != null)
            throw new Exception($"{nameof(SiteBuilder)} instance already created.");
        Instance = this;

        config.BuildConfig.WwwRoot ??= FindWwwRoot(config.BuildConfig);

        //public
        Config = config;
        Tags = new(this);
        Pages = new(config);

        //internal
        Target = new(config);

        //private
        fileRenderer = new FileBuilder(this);
        blazorIndex = new(this);
        wwwroot = new(this);
        linkScanner = new(this);
    }

    static DirPath FindWwwRoot(BuildConfig config)
    {
        var asmPath = new FilePath(config.AppType.Assembly.Location);

        var dir = asmPath.Parent.CombineDir("wwwroot");
        if (dir.Exists())
        {
            logger.LogInformation($"Found {dir} next to {asmPath}");
            return dir;
        }

        //Try to find wwwroot when running in Visual Studio
        dir = asmPath.Parent;
        var testPath = dir.Path.Replace('\\', '/');
        if (testPath.EndsWith("/bin/Debug/net9.0") ||
            testPath.EndsWith("/bin/Release/net9.0"))
        {
            dir = dir.Parent.Parent.Parent.CombineDir("wwwroot");
            if (dir.Exists())
            {
                logger.LogInformation($"Found {dir}\n   in project root above {asmPath}");
                return dir;
            }
        }

        logger.LogCritical(@$"Failed to find wwwroot near:
{asmPath}
You must configure {nameof(BuildConfig.WwwRoot)} in code.");
        throw new ArgumentException("Missing wwwroot path in config.");
    }

    internal PageData TransientPage(IServiceProvider provider)
    {
        var nm = provider.GetService<NavigationManager>()!;
        var path = new Uri(nm.Uri).AbsolutePath;
        var url = Config.BaseURL.HostURL.Append(path);
        var relUrl = new RelUrl(Config.BaseURL, url);
        var page = Config.SiteBuilder.Pages.GetOrCreate(relUrl);

        //Only Blazor pages would inject a SitePage
        page.IsBlazor = true;
        Debug.Assert(page.Href.EndsWith(".css") == false);
        Debug.Assert(page.Href.EndsWith(".js") == false);
        return page;
    }

    private HttpClient httpClient = null!;

    public async Task Build(WebApplication app)
    {
        httpClient = new HttpClient()
        {
            BaseAddress = new Uri(app.Urls.First() + Config.BaseURL.Href + "/")
        };

        Config.BuildConfig.Target.EmptyDirectory();

        if (Target == null)
            throw new Exception($"Missing target in new {nameof(SiteBuilder)}().");

        if (Config.BuildConfig.NoGeneration)
        {
            //PreRender all Blazor pages
            //Otherwise the live pages need to be loaded twice

            blazorIndex.Scan();

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
        //Config.BaseURL.Replace(httpClient.BaseAddress);
    }

    async Task BuildBlazorPages()
    {
        while (Pages.Next(out var page))
        {
            using var scope = logger.BeginScope(page.BuildStage);

            logger.LogDebug($"/{page.Href} (starting...)");

            var originalURL = page.URL;
            byte[] fileContent = null!;

            if (page.BuildStage == BuildStage.PreScan)
            {
                if (page.BlazorType == null)
                {
                    fileContent = await RenderPage(page);
                    if (page.IsBlazor == false)
                    {
                        //No need to run a second stage for non blazor files.
                        page.BuildStage = BuildStage.FinalBuild;
                    }
                }
                else
                    await new BlazorRenderer(this, page).RenderComponent(); //Only render the component
            }
            else
            {
                Debug.Assert(page.BuildStage == BuildStage.FinalBuild);
                //BuildFinal
                fileContent = await RenderPage(page);
            }

            if (page.InFeed && !page.IsDraftOrNotPublished && page.URL == page.BlogPostRandomURL)
                page.URL = Config.PostURL(page);

            if (page.URL != originalURL)
            {
                logger.LogDebug($"New Path: /{page.Href}");
                Pages.UpdatePageUrl(oldURL: originalURL, page);
            }

            if (page.IsDraftOrNotPublished)
            {
                logger.LogInformation($"Draft: /{page.Href}");
                Pages.RemoveDraft(page);
                continue;
            }

            if (page.BuildStage == BuildStage.PreScan)
            {
                Pages.DonePreScan(page);
            }
            else
            {
                Debug.Assert(page.BuildStage == BuildStage.FinalBuild);

                //BuildFinal

                if (page.IsBlazor)
                {
                    var html = Encoding.UTF8.GetString(fileContent);
                    html = HtmlCleanup.Clean(html, Config);
                    fileContent = Encoding.UTF8.GetBytes(html);

                    linkScanner.Scan(html);
                }
                //Don't cleanup css,js...

                Target.Store(page.URL, fileContent);

                logger.LogInformation($"/{page.Href} ({fileContent.Length:#,#} bytes)");

                Pages.DoneFinalBuild(page);
            }
        }
    }

    async Task<byte[]> RenderPage(PageData page)
    {
        try
        {
            using var resp = await httpClient.GetAsync(page.Href, CancellationToken.None);
            var data = await resp.Content.ReadAsByteArrayAsync();
            return data;
            //return await httpClient.GetStringAsync(page.Href, CancellationToken.None);
        }
        catch (HttpRequestException ex)
        {
            logger.LogCritical(ex, page.URL.Href);
            Pages.FailBlazor(page);
            throw;
        }
    }
}
