using SilentOrbit.StaticOnline.Building.FileGeneration;
using System.Collections.Concurrent;

namespace SilentOrbit.StaticOnline.Building;

public class PageTracker(SiteBuilder site)
{
    readonly Url baseUrl = site.Config.BaseURL;

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

    internal void AddLink(Url url)
    {
        if (all.Contains(url))
            return;

        if (url.StartsWith(baseUrl) == false)
        {
            //Console.WriteLine($"External: {url}");
            return; //external URL
        }

        GetOrCreate(url);
    }

    #endregion

    #region Run Next

    /// <summary>
    /// Next page to run a prescan
    /// </summary>
    /// <returns></returns>
    internal PageData? NextPreScan()
    {
        if (queuePreScan.Count == 0)
            return null;

        var page = queuePreScan.Dequeue();
        page.BuildStage = BuildStage.PreScan;
        return page;
    }

    internal void DonePreScan(PageData page)
    {
        Debug.Assert(page.BuildStage == BuildStage.PreScan);
        page.BuildStage = BuildStage.PreScanDone;

        if (page.IsDraft)
            return;

        if (page.BuildLast)
            queueBuildLast.Enqueue(page);
        else
            queueFinalBuild.Enqueue(page);
    }

    internal PageData? NextFinalBuild()
    {
        if (queueFinalBuild.TryDequeue(out var page))
        {
            page.BuildStage = BuildStage.FinalBuild;
            return page;
        }

        if (queueBuildLast.TryDequeue(out page))
        {
            page.BuildStage = BuildStage.FinalBuild;
            return page;
        }

        return null;
    }


    #endregion

    #region Result

    internal void UpdatePageUrl(Url oldURL, PageData page)
    {
        //Update URL set by the page's own code.
        urlPage.TryRemove(oldURL, out var oldPage);
        Debug.Assert(oldPage == page);
        urlPage[page.URL] = page;
    }

    internal void FailBlazor(PageData page)
    {
        page.BuildStage = BuildStage.Fail;
    }

    internal void DoneFinalBuild(PageData page)
    {
        Debug.Assert(page.BuildStage == BuildStage.FinalBuild);
        page.BuildStage = BuildStage.FinalBuildDone;
    }

    /// <summary>
    /// Not added before, first reported here when done
    /// </summary>
    internal void DoneStatic(Url url)
    {
        if (all.Contains(url))
            return;

        all.Add(url);
    }

    #endregion

    #region Enumeration

    public IEnumerable<PageData> All => urlPage.Values
        .Where(p => !p.IsUpdate && p.IsBlazor);

    public IEnumerable<PageData> Updates => urlPage.Values
        .Where(p => p.IsUpdate)
        .OrderByDescending(p => p.Published);

    public IEnumerable<PageData> BlogPosts => urlPage.Values
        .Where(p => p.InFeed && !p.IsUpdate)
        .OrderByDescending(p => p.Published);

    public IEnumerable<PageData> Feed => urlPage.Values
        .Where(p => p.InFeed)
        .OrderByDescending(p => p.Published);

    #endregion

    public PageData GetOrCreate(Url url, bool build = true)
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
            GetOrCreate(simple);
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
        page.BuildStage = BuildStage.Fail;
        urlPage.TryRemove(page.URL, out var removed);
        Debug.Assert(removed == page);
    }
}
