using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class AtomFeed : FileGeneratorBase
{
    public override RelUrl URL => Config.BaseURL.Append("atom.xml");

    public override void Init()
    {
        Site.Feed.Atom = new FeedList.Item
        {
            MimeType = "application/atom+xml",
            Title = Config.Title,
            URL = URL
        };
    }

    static readonly XNamespace ns
        = XNamespace.Get("http://www.w3.org/2005/Atom");

    public override string? Generate()
    {
        var lastModified = Site.Pages.Feed
            .Select(p => p.Modified ?? p.Published)
            .Max();

        var feed = new XElement(ns + "feed",
            new XElement(ns + "title", Site.Config.Title),
            new XElement(ns + "subtitle", Site.Config.Description),
            new XElement(ns + "id", Site.Config.BaseURL),
            new XElement(ns + "link",
                new XAttribute("href", Site.Config.BaseURL)),
            new XElement(ns + "link",
                new XAttribute("href", URL),
                new XAttribute("rel", "self"))
            );
        if (lastModified != null)
            feed.Add(new XElement(ns + "updated", lastModified?.ToRFC3339()));

        foreach (var post in Site.Pages.Feed)
        {
            var entry = new XElement(ns + "entry",
                new XElement(ns + "title", post.Title),
                new XElement(ns + "link",
                    new XAttribute("href", post.URL)),
                new XElement(ns + "id", post.URL),
                GetAuthor(post)
            );
            entry.AddElement("summary", post.Summary?.Value)
                ?.SetAttributeValue("type", "html");
            AddElementIf(entry, "published", post.Published);
            AddElementIf(entry, "updated", post.Modified);
            feed.Add(entry);
        }

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), feed);

        //doc.ToString will not include the <?xml version="1.0" encoding="utf-8"?>
        //Which is needed for RSS feeds
        return doc.ToString();
    }

    static void AddElementIf(XElement feed, string v, Timestamp? published)
    {
        if (published == null)
            return;
        feed.Add(new XElement(
            feed.Name.Namespace + v,
            published.ToRFC3339()));
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
