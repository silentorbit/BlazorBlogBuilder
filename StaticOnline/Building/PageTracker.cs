using SilentOrbit.StaticOnline.Building.FileGeneration;
using System.Collections.Concurrent;

namespace SilentOrbit.StaticOnline.Building;

public class PageTracker(SiteConfig config)
{
    readonly ILogger logger = new CompactConsoleLogger<PageTracker>();

    readonly BaseUrl baseUrl = config.BaseURL;

    readonly ConcurrentDictionary<Url, PageData> urlPage = new();

    readonly Queue<PageData> queuePreScan = new();
    readonly Queue<PageData> queueFinalBuild = new();
    readonly Queue<PageData> queueBuildLast = new();
    readonly List<Url> all = new();

    #region Adding

    internal void AddIndex(FileGeneratorBase file)
    {
        var url = file.URL;
        Debug.Assert(all.Contains(url) == false);

        if (all.Contains(url))
            return;
        all.Add(url);
    }

    public void AddLink(Url url)
    {
        if (all.Contains(url))
            return;

        if (url.StartsWith(baseUrl) == false)
        {
            logger.LogInformation($"External: {url}");

            all.Add(url);
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
        if (queuePreScan.TryDequeue(out page!))
        {
            page.BuildStage = BuildStage.PreScan;
            return true;
        }

        if (config.BuildConfig.NoGeneration)
        {
            page = null!;
            return false;
        }

        if (queueFinalBuild.TryDequeue(out page!))
        {
            page.BuildStage = BuildStage.FinalBuild;
            return true;
        }

        if (queueBuildLast.TryDequeue(out page!))
        {
            page.BuildStage = BuildStage.FinalBuild;
            return true;
        }

        return false;
    }

    internal void DonePreScan(PageData page)
    {
        Debug.Assert(page.BuildStage == BuildStage.PreScan);

        if (page.IsDraftOrNotPublished)
            return;

        if (page.BuildLast)
            queueBuildLast.Enqueue(page);
        else
            queueFinalBuild.Enqueue(page);
    }

    #endregion

    #region Result

    internal void UpdatePageUrl(Url oldURL, PageData page)
    {
        //Update URL set by the page's own code.
        var removed = urlPage.TryRemove(oldURL, out var oldPage);
        Debug.Assert(removed && oldPage == page);
        urlPage[page.URL] = page;
    }

    internal void FailBlazor(PageData page)
    {
        page.BuildStage = BuildStage.Fail;
    }

    internal void DoneFinalBuild(PageData page)
    {
        Debug.Assert(page.BuildStage == BuildStage.FinalBuild);
    }

    /// <summary>
    /// Not added before, first reported here when done
    /// </summary>
    internal void DoneStatic(RelUrl url)
    {
        if (all.Contains(url))
            return;

        all.Add(url);
    }

    #endregion

    #region Enumeration

    public IEnumerable<PageData> All => urlPage.Values
        .Where(p => !p.IsDraftOrNotPublished && !p.IsUpdate && p.IsBlazor)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> Updates => urlPage.Values
        .Where(p => !p.IsDraftOrNotPublished && p.IsUpdate)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> BlogPosts => urlPage.Values
        .Where(p => !p.IsDraftOrNotPublished && p.InFeed && !p.IsUpdate)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    public IEnumerable<PageData> Feed => urlPage.Values
        .Where(p => !p.IsDraftOrNotPublished && p.InFeed)
        .OrderByDescending(p => p.Published)
        .ThenBy(p => p.Href); //Make sure articles published at the same time always appear in the same order

    #endregion

    public PageData GetOrCreate(RelUrl url, bool build = true)
    {
        if (urlPage.TryGetValue(url, out var page))
        {
            Debug.Assert(page.URL == url);
            return page;
        }

        page = new PageData { URL = url };
        urlPage.TryAdd(page.URL, page);
        all.Add(page.URL);

        if (url.HasQueryOrFragment)
        {
            //URL has ?query=123&or#fragment
            //Make sure the version without query or fragment exists
            var simple = url.HostURL.Append(new Uri(url).AbsolutePath);
            var simpleRel = new RelUrl(config.BaseURL, simple);
            GetOrCreate(simpleRel);
        }
        else
        {
            if (build)
                queuePreScan.Enqueue(page);
        }
        return page;
    }

    internal void RemoveDraft(PageData page)
    {
        page.BuildStage = BuildStage.Draft;
        urlPage.Remove(page);
    }
}

