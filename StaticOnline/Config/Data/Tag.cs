using SilentOrbit.StaticOnline.Building.FileGeneration;
using System.Text.Encodings.Web;

namespace SilentOrbit.StaticOnline.Config.Data;

public class Tag
{
    static Dictionary<string, Tag> tags = new();

    public string Name { get; }
    /// <summary>
    /// Lowercase URL safe version of <see cref="Name"/>.
    /// </summary>
    public string ID { get; }
    public RelUrl URL { get; }

    public FeedList Feed { get; }

    /// <summary>
    /// Number of pages using the tag
    /// </summary>
    public int PageCount { get; set; }

    public static IEnumerable<Tag> All => tags.Values;

    public static Tag ByName(string name)
    {
        if (name.Contains(" ") ||
            name.Contains(",") ||
            name.Contains("\t"))
            throw new ArgumentException("Single tag may not contain separator: space, comma, tab");

        var id = GetID(name);

        if (tags.TryGetValue(id, out var tag))
            return tag;

        var t = new Tag(name);
        tags[id] = t;
        return t;
    }

    public static Tag ByID(string id)
    {
        if (tags.TryGetValue(id, out var tag))
            return tag;

        throw new NotImplementedException("Must be created by name before it can be found by ID");
    }

    Tag(string name)
    {
        ID = GetID(name);
        Name = name;
        URL = SiteBuilder.Instance.Config.TagURL(ID);

        Feed = new();
        FeedGeneratorBase.Init(this);
    }

    static string GetID(string name)
    {
        if (name.Contains(" ") ||
            name.Contains(",") ||
            name.Contains("\t"))
            throw new ArgumentException("Single tag may not contain separator: space, comma, tab");

        name = name.ToLowerInvariant();
        name = UrlEncoder.Default.Encode(name);
        return name;

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
