using System.Collections;

namespace SilentOrbit.StaticOnline.Config;

/// <summary>
/// Collection of tags
/// </summary>
public class Tags : IEnumerable<Tag>
{
    readonly List<Tag> list = new();

    public int Count => list.Count;

    public bool Contains(Tag tag)
    {
        return list.Contains(tag);
    }

    /// <summary>
    /// Multiple tags separated by space, comma, tab, newline
    /// </summary>
    public void Add(string tags)
    {
        var split = tags.Split([",", " ", "\t", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
        foreach (var tag in split)
            list.Add(Tag.ByName(tag));
    }

    public void Add(params Tag[] tags)
    {
        list.AddRange(tags);
    }

    IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator()
        => list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => list.GetEnumerator();

    public static implicit operator Tags(string multiple_tags)
    {
        var tags = new Tags();
        tags.Add(multiple_tags);
        return tags;
    }

}
