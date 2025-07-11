namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Track all tags within the site
/// </summary>
public class TagBuilder(SiteBuilder site)
{
    readonly Dictionary<string, Tag> tags = new();

    public PageData[] GetPages(Tag page)
    {
        var pages = new List<PageData>();
        var list = site.Pages.All.Where(p => p.Tags.Contains(page)).ToArray();
        return list;
    }

    public List<Tag> GetTags()
    {
        //Reset size
        foreach (var tag in Tag.All)
            tag.Size = 0;

        var tags = new List<Tag>();
        foreach(var p in site.Pages.All)
        {
            foreach(var tag in p.Tags)
            {
                tag.Size++;
                if (tags.Contains(tag) == false)
                    tags.Add(tag);
            }
        }
        return tags;
    }
}
