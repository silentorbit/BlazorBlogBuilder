namespace SilentOrbit.StaticOnline.Config.Data;

/// <summary>
/// https://developers.google.com/search/docs/crawling-indexing/robots-meta-tag
/// </summary>
public class Robots
{
    /// <summary>
    /// Include in sitemap
    /// </summary>
    public bool? Sitemap { get; set; }

    public bool? NoIndex { get; set; }
    public bool? NoFollow { get; set; }
    public bool? NoSnippet { get; set; }
    public bool? NoTranslate { get; set; }

    internal string? GetContent(SiteConfig config)
    {
        var defaultRobots = config.DefaultRobots;
        var list = new List<string>();

        if (NoIndex ?? defaultRobots.NoIndex == true)
            list.Add("noindex");
        if (NoFollow ?? defaultRobots.NoFollow == true)
            list.Add("nofollow");
        if (NoSnippet ?? defaultRobots.NoSnippet == true)
            list.Add("nosnippet");
        if (NoTranslate ?? defaultRobots.NoTranslate == true)
            list.Add("notranslate");

        if (list.Count == 0)
            return null;
        else
            return string.Join(",", list);
    }
}
