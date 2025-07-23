namespace SilentOrbit.StaticOnline.Building.FileGeneration;

abstract class SitemapBase : FileGeneratorBase
{
    public abstract RelUrl URL { get; }

    public static void Init()
    {
        var txt = new SitemapTXT();
        txt.AddGenerator(txt.URL);

        var xml = new SitemapXML();
        xml.AddGenerator(xml.URL);
    }

    protected IEnumerable<PageData> SitemapPages()
    {
        var defaultRobots = Config.DefaultRobots;

        foreach (var page in Builder.Pages.All.OrderBy(p => p.Href))
        {
            //default or page sitemap need to be set
            if ((page.Robots.Sitemap ?? defaultRobots.Sitemap) != true)
                continue;

            //Remove robots: noindex
            if ((page.Robots.NoIndex ?? defaultRobots.NoIndex) == true)
                continue;

            //Remove redirects, unless page sitemap is set explicitly
            if (page.Redirect != null && page.Robots.Sitemap == null)
                continue;

            //Skip updates, links inside of pages.
            if (page.URL.HasQueryOrFragment)
                continue;

            yield return page;
        }
    }

}
