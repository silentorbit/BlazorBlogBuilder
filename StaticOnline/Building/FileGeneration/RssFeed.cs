using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class RssFeed : FeedGeneratorBase
{
    public override RelUrl URL => Config.BaseURL.Append("rss.xml");

    public override void Init()
    {
        Site.Feed.RSS = new FeedList.Item
        {
            MimeType = "application/rss+xml",
            Title = Config.Title,
            URL = URL
        };
    }

    const string dateFormat = "r";

    public override string Generate()
    {
        var rss = new XElement("rss",
            new XAttribute("version", "2.0"));
        var channel = new XElement("channel",
            new XElement("title", Site.Config.Title),
            new XElement("description", Site.Config.Description),
            new XElement("link", Site.Config.BaseURL),
            new XElement(XName.Get("link", "http://www.w3.org/2005/Atom"),
                new XAttribute("rel", "self"),
                new XAttribute("href", URL),
                new XAttribute("type", "application/rss+xml"))
            );
        rss.Add(channel);

        foreach (var post in Site.Pages.Feed)
        {
            var item = new XElement("item",
                new XElement("guid", post.URL),
                new XElement("title", post.Title),
                new XElement("link", post.URL)
            );
            item.AddElement("description", post.Summary?.Value);
            AddElementIf(item, "pubDate", post.Published);
            channel.Add(item);
        }

        return rss.ToUtf8String(xmlHeader: false);
    }

    static void AddElementIf(XElement feed, string v, Timestamp? published)
    {
        if (published == null)
            return;
        feed.Add(new XElement(
            feed.Name.Namespace + v,
            published.ToString(dateFormat)));
    }

}
