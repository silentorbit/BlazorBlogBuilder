using System.Collections.Concurrent;

namespace SilentOrbit.StaticOnline.Building;

public class PageTracker(SiteConfig config)
{
    readonly ILogger logger = new CompactConsoleLogger<PageTracker>();

    readonly BaseUrl baseUrl = config.BaseURL;

    readonly HashSet<PageData> pages = new();

    readonly List<Url> allURLs = new();

    #region Adding

    public void AddLink(Url url)
    {
        if (allURLs.Contains(url))
            return;

        if (url.StartsWith(baseUrl) == false)
        {
            logger.LogInformation($"External: {url}");

            allURLs.Add(url);
            return; //external URL
        }

        if (url is RelUrl rel == false)
            rel = new RelUrl(baseUrl, baseUrl.GetRelativePath(url));

        //all.Add inside:
        GetOrCreate(rel);
    }

    #endregion

    #region Run Next

    internal bool Next(out PageData page)
    {
        var removed = pages.RemoveWhere(p => p.IsDraftOrNotPublished);

        //PreScan
        page = pages.FirstOrDefault(p => p.BuildStage == BuildStage.Added)!;
        if (page != null)
        {
            page.BuildStage = BuildStage.PreScan;
            return true;
        }

        if (config.BuildConfig.NoGeneration)
        {
            page = null!;
            return false;
        }

        //FinalBuild
        page = pages.FirstOrDefault(p =>
            p.BuildStage == BuildStage.PreScan &&
            p.BuildLast == false)!;
        if (page != null)
        {
            page.BuildStage = BuildStage.FinalBuild;
            return true;
        }

        //BuildLast: Index pages, feeds and sitemaps
        page = pages.FirstOrDefault(p =>
            p.BuildStage == BuildStage.PreScan &&
            p.BuildLast)!;
        if (page != null)
        {
            page.BuildStage = BuildStage.FinalBuild;
            return true;
        }

        return false;
    }

    #endregion

    #region Result

    /// <summary>
    /// Not added before, first reported here when done
    /// </summary>
    internal void DoneStatic(RelUrl url)
    {
        if (allURLs.Contains(url))
            return;

        allURLs.Add(url);
    }

    #endregion

    #region Enumeration

    public IEnumerable<PageData> All => pages
        .Where(p => !p.IsDraftOrNotPublished && !p.IsUpdate && p.IsBlazor)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> Updates => pages
        .Where(p => !p.IsDraftOrNotPublished && p.IsUpdate)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> BlogPosts => pages
        .Where(p => !p.IsDraftOrNotPublished && p.InFeed && !p.IsUpdate)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> Feed => pages
        .Where(p => !p.IsDraftOrNotPublished && p.InFeed)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    #endregion

    public PageData? GetExisting(RelUrl url)
    {
        var page = pages.FirstOrDefault(p => p.URL == url);
        return page;
    }

    public PageData GetOrCreate(RelUrl url)
    {
        var page = pages.FirstOrDefault(p => p.URL == url);
        if (page != null)
            return page;

        page = new PageData { URL = url };
        Debug.Assert(page.Href != "atom.c");
        pages.Add(page);
        allURLs.Add(page.URL);

        if (url.HasQueryOrFragment)
        {
            //URL has ?query=123&or#fragment
            //Make sure the version without query or fragment exists
            var simple = url.HostURL.Append(new Uri(url).AbsolutePath);
            var simpleRel = new RelUrl(config.BaseURL, simple);
            GetOrCreate(simpleRel);
        }

        return page;
    }

}

