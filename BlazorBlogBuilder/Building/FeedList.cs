namespace SilentOrbit.BlazorBlogBuilder.Building;

public class FeedList : IEnumerable<FeedList.Item>
{
    public Item RSS { get; set; } = null!;
    public Item Atom { get; set; } = null!;
    public Item JSON { get; set; } = null!;

    public class Item
    {
        public string MimeType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public RelUrl URL { get; set; } = null!;
    }

    internal void Set(string mimeType, string title, RelUrl url)
    {
        Item item = new()
        {
            MimeType = mimeType,
            Title = title,
            URL = url
        };
        switch (item.MimeType)
        {
            case "application/rss+xml": RSS = item; break;
            case "application/atom+xml": Atom = item; break;
            case "application/feed+json": JSON = item; break;

            default:
                throw new NotImplementedException(item.MimeType);
        }
    }

    IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
    {
        if (RSS != null) yield return RSS;
        if (Atom != null) yield return Atom;
        if (JSON != null) yield return JSON;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Item>)this).GetEnumerator();
    }
}