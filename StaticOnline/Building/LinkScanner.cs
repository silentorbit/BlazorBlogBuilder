using System.Text.RegularExpressions;
using System.Web;

namespace SilentOrbit.StaticOnline.Building;

public class LinkScanner(SiteBuilder site)
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
                url = new Uri(href);
            }
            else if (href.StartsWith('/'))
            {
                //Root url
                url = site.Config.BaseURL.HostURL.Append(href);
            }
            else
            {
                //Local path, add domain
                url = site.Config.BaseURL.Append(href);
            }

            site.Pages.AddLink(url);
        }
    }
}
