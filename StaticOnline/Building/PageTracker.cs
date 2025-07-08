using Microsoft.AspNetCore.Mvc.RazorPages;
using SilentOrbit.StaticOnline.Building.FileGeneration;
using System.Collections;

namespace SilentOrbit.StaticOnline.Building;

public class PageTracker(SiteBuilder site)
{
    readonly Url baseUrl = site.Config.BaseURL;

    readonly Dictionary<Url, SitePage> urlPage = new();

    readonly Queue<SitePage> queuePreScan = new();
    readonly Queue<SitePage> queueFinalBuild = new();
    readonly Queue<SitePage> queueBuildLast = new();
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
    internal SitePage? NextPreScan()
    {
        if (queuePreScan.Count == 0)
            return null;

        var page = queuePreScan.Dequeue();
        page.BuildStage = BuildStage.PreScan;
        return page;
    }

    internal void DonePreScan(SitePage page)
    {
        Debug.Assert(page.BuildStage == BuildStage.PreScan);
        page.BuildStage = BuildStage.PreScanDone;

        if (page.BuildLast)
            queueBuildLast.Enqueue(page);
        else
            queueFinalBuild.Enqueue(page);
    }

    internal SitePage? NextFinalBuild()
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

    internal void UpdatePageUrl(Url oldURL, SitePage page)
    {
        //Update URL set by the page's own code.
        urlPage.Remove(oldURL);
        urlPage[page.URL] = page;
    }

    internal void FailBlazor(SitePage page)
    {
        page.BuildStage = BuildStage.Fail;
    }

    internal void DoneFinalBuild(SitePage page)
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

    public IEnumerable<SitePage> All => urlPage.Values
        .Where(p => !p.IsUpdate);

    public IEnumerable<SitePage> Updates => urlPage.Values
        .Where(p => p.IsUpdate)
        .OrderByDescending(p => p.Published);

    public IEnumerable<SitePage> BlogPosts => urlPage.Values
        .Where(p => p.InFeed && !p.IsUpdate)
        .OrderByDescending(p => p.Published);

    public IEnumerable<SitePage> Feed => urlPage.Values
        .Where(p => p.InFeed)
        .OrderByDescending(p => p.Published);

    #endregion

    public SitePage GetOrCreate(Url url, bool build = true)
    {
        if (urlPage.TryGetValue(url, out var page))
        {
            Debug.Assert(page.URL == url);
            return page;
        }

        page = new SitePage { URL = url, FromURL = true };
        urlPage.Add(page.URL, page);
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
}
