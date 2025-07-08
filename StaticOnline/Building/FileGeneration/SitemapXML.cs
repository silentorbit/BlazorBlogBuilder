using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class SitemapXML : SitemapBase
{
    public override Url URL => Config.BaseURL + "sitemap.xml";

    static readonly XNamespace ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");

    public override string Generate()
    {
        var urlset = new XElement(ns + "urlset");

        foreach (var page in SitemapPages())
        {
            var url = new XElement(ns + "url",
                new XElement(ns + "loc", page.URL));
            AddElementIf(url, "lastmod", page.Modified);
            urlset.Add(url);
        }

        return urlset.ToUtf8String(true);
    }

    const string dateFormat = "yyyy-MM-dd";

    static void AddElementIf(XElement feed, string v, DateTime? published)
    {
        if (published == null)
            return;
        feed.Add(new XElement(
            feed.Name.Namespace + v,
            published.Value.ToString(dateFormat)));
    }

}
