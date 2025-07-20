using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class AtomFeed : FeedGeneratorBase
{
    protected override string path { get; } = "atom.xml";

    public override void Init()
    {
        var feed = Config.Head.Feed!.Atom = new FeedList.Item
        {
            MimeType = "application/atom+xml",
            Title = Config.Title,
            URL = Config.BaseURL.Append(path),
        };
        AddGenerator(feed.URL);
    }

    static readonly XNamespace ns
        = XNamespace.Get("http://www.w3.org/2005/Atom");

    protected override async Task<string> GenerateFeed(RelUrl url, IEnumerable<PageData> posts)
    {
        var lastModified = posts
            .Select(p => p.Modified ?? p.Published)
            .Max();

        var feed = new XElement(ns + "feed",
            new XElement(ns + "title", Builder.Config.Title),
            new XElement(ns + "subtitle", Builder.Config.Description),
            new XElement(ns + "id", Builder.Config.BaseURL),
            new XElement(ns + "link",
                new XAttribute("href", Builder.Config.BaseURL)),
            new XElement(ns + "link",
                new XAttribute("href", url),
                new XAttribute("rel", "self")),
            new XElement("base", Builder.Config.BaseURL.Href)
            );
        AddElementIf(feed, "updated", lastModified);

        foreach (var post in posts)
        {
            var entry = new XElement(ns + "entry",
                new XElement(ns + "title", post.Title),
                new XElement(ns + "link",
                    new XAttribute("href", post.URL)),
                new XElement(ns + "id", post.ID ?? post.URL),
                GetAuthor(post)
            );
            if (Config.FeedContent >= FeedContent.Summary)
            {
                entry.AddElementIf("summary", post.Summary?.Value)
                    ?.SetAttributeValue("type", "html");
            }
            entry.AddElementIf("content", await GetPostContent(post))
                ?.SetAttributeValue("type", "html");
            AddElementIf(entry, "published", post.Published);
            AddElementIf(entry, "updated", post.Modified);
            feed.Add(entry);
        }

        return feed.ToUtf8String();
    }

    static void AddElementIf(XElement feed, string v, Timestamp? timestamp)
    {
        if (timestamp == null)
            return;

        feed.Add(new XElement(
            feed.Name.Namespace + v,
            timestamp.ToAtomRFC3339()));
    }

    XElement GetAuthor(PageData post)
    {
        var author = new XElement(ns + "author");

        var a = post.Author ?? Config.Author;
        if (a.Name != null)
            author.Add(new XElement(ns + "name", a.Name));
        if (a.URL != null)
            author.Add(new XElement(ns + "uri", a.URL));
        if (a.Email != null)
            author.Add(new XElement(ns + "email", a.Email));

        return author;
    }

    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
