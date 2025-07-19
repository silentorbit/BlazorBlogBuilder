using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class RssFeed : FeedGeneratorBase
{
    public override RelUrl URL => Config.BaseURL.Append("rss.xml");

    public override void Init()
    {
        Builder.Feed.RSS = new FeedList.Item
        {
            MimeType = "application/rss+xml",
            Title = Config.Title,
            URL = URL
        };
    }

    const string dateFormat = "r";

    public override async Task<string> Generate()
    {
        var rss = new XElement("rss",
            new XAttribute("version", "2.0"));
        var channel = new XElement("channel",
            new XElement("title", Builder.Config.Title),
            new XElement("description", Builder.Config.Description),
            new XElement("link", Builder.Config.BaseURL),
            new XElement(XName.Get("link", "http://www.w3.org/2005/Atom"),
                new XAttribute("rel", "self"),
                new XAttribute("href", URL),
                new XAttribute("type", "application/rss+xml"))
            );
        rss.Add(channel);

        foreach (var post in Builder.Pages.Feed)
        {
            var item = new XElement("item",
                new XElement("guid", post.ID ?? post.URL),
                new XElement("title", post.Title),
                new XElement("link", post.URL)
            );
            var content = await GetPostContent(post);
            item.AddElementIf("description", content ?? "");
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
