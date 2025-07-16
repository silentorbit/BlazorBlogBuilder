namespace SilentOrbit.StaticOnline.Building;

public class LinkScanner(SiteBuilder builder)
{
    internal void Scan(string html)
    {
        var re = new Regex(@"href=""([^""]+)""");
        var matchList = re.Matches(html);
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
}
