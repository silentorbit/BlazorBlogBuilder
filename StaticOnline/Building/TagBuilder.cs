namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Track all tags within the site
/// </summary>
public class TagBuilder(SiteBuilder builder)
{
    public PageData[] GetPages(Tag page)
    {
        var pages = new List<PageData>();
        var list = builder.Pages.All.Where(p => p.Tags.Contains(page)).ToArray();
        return list;
    }

    public List<Tag> GetTags()
    {
        //Reset size
        foreach (var tag in Tag.All)
            tag.PageCount = 0;

        var tags = new List<Tag>();
        foreach (var p in builder.Pages.All)
        {
            foreach (var tag in p.Tags)
            {
                tag.PageCount++;
                if (tags.Contains(tag) == false)
                    tags.Add(tag);
            }
        }
        tags.Sort((a, b) => a.ID.CompareTo(b.ID));
        return tags;
    }
}
