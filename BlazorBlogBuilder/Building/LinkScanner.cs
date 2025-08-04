namespace SilentOrbit.BlazorBlogBuilder.Building;

public partial class LinkScanner(SiteBuilder builder)
{
    internal void Scan(string html)
    {
        //Links to other sources
        var matchList = ReSrc().Matches(html);
        foreach (Match m in matchList)
        {
            var href = m.Groups[1].Value;
            href = HttpUtility.HtmlDecode(href);
            if (href == "#")
                continue;

            Url url;
            if (href.Contains("://"))
            {
                //Full URL
                url = href;
                if (url.StartsWith(builder.Config.BaseURL) == false)
                {
                    //Skipping sources outside build path.
                    continue;
                }
            }
            else if (href.StartsWith('/'))
            {
                //Root url
                var baseHref = builder.Config.BaseURL.Href;
                if (href.StartsWith(baseHref))
                    url = builder.Config.BaseURL.Append(href.Substring(baseHref.Length));
                else
                    url = builder.Config.BaseURL.HostURL.Append(href);
            }
            else
            {
                //Local path, add to BaseURL
                url = builder.Config.BaseURL.Append(href);
            }

            builder.Pages.AddLink(url);
        }

        //Links to other pages
        matchList = ReHref().Matches(html);
        foreach (Match m in matchList)
        {
            var href = m.Groups[1].Value;
            if (href.StartsWith("mailto:"))
                continue;
            if (href.StartsWith("tel:"))
                continue;

            href = HttpUtility.HtmlDecode(href);
            if (href == "#")
                continue;

            Url url;
            if (href.Contains("://"))
            {
                //Full URL
                url = href;
            }
            else if (href.StartsWith('/'))
            {
                //Root url
                var baseHref = builder.Config.BaseURL.Href;
                if (href.StartsWith(baseHref))
                    url = builder.Config.BaseURL.Append(href.Substring(baseHref.Length));
                else
                    url = builder.Config.BaseURL.HostURL.Append(href);
            }
            else
            {
                //Local path, add domain
                url = builder.Config.BaseURL.Append(href);
            }

            builder.Pages.AddLink(url);
        }
    }

    [GeneratedRegex(@" href=""([^""]+)""")]
    private static partial Regex ReHref();

    [GeneratedRegex(@" src=""([^""]+)""")]
    private static partial Regex ReSrc();
}
