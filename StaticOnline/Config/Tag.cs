using SilentOrbit.StaticOnline.Building;

namespace SilentOrbit.StaticOnline.Config;

public class Tag
{
    static Dictionary<string, Tag> tags = new();

    public string Href { get; }
    public string ID { get; }
    public string Name { get; }
    /// <summary>
    /// Number of pages using the tag
    /// </summary>
    public int Size { get; set; }

    public static IEnumerable<Tag> All => tags.Values;

    public static Tag ByName(string name)
    {
        if (name.Contains(" ") ||
            name.Contains(",") ||
            name.Contains("\t"))
            throw new ArgumentException("Single tag may not contain separator: space, comma, tab");

        if (tags.TryGetValue(name, out var tag))
            return tag;

        var t = tags[name] = new Tag(name);
        return t;
    }

    Tag(string name)
    {
        Href = SiteBuilder.Instance.Config.BaseURL.Append("/tags/" + name).Href;
        ID = name.ToLowerInvariant();
        Name = name;
    }

    public static implicit operator Tag(string name)
        => ByName(name);

    public override bool Equals(object? obj)
    {
        if (obj is Tag tag)
            return tag.ID == ID;
        return false;
    }
    public override int GetHashCode() => ID.GetHashCode();

    public static bool operator ==(Tag? a, Tag? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.ID == b.ID;
    }
    public static bool operator !=(Tag a, Tag b)
    {
        if (a is null && b is null) return false;
        if (a is null || b is null) return true;
        return a.ID != b.ID;
    }

}
