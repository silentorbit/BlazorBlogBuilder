using System.Collections;

namespace SilentOrbit.StaticOnline.Building;

public class FeedList : IEnumerable<FeedList.Item>
{
    public Item? RSS { get; set; }
    public Item? Atom { get; set; }
    public Item? JSON { get; set; }

    public class Item
    {
        public string MimeType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public Url URL { get; set; } = null!;
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