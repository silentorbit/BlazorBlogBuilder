using Demo.Components;
using SilentOrbit.StaticOnline.Config;

namespace Demo;

public class DemoSiteConfig : SiteConfig<App>
{
    public DemoSiteConfig()
    {
        Title = "Demo";
        Description = "This is a Demo";
        //Build static files to be served in the subdirectory
        BaseURL = "https://www.silentorbit.com/static-online";
        Favicon = new()
        {
            URL = "https://www.silentorbit.com/favicon.png",
            MimeType = "image/png"
        };
        CommentEmail = "demo@silentorbit.com";
        DefaultRobots.NoTranslate = true;
        DefaultRobots.Sitemap = true;
        Author.Name = "Silent Orbit";

        TopMenu.Add(new()
        {
            Title = "Blog",
            Link = BaseURL.Append("/")
        });
        TopMenu.Add(new()
        {
            Title = "Tags",
            Link = BaseURL.Append("/tags")
        });
        TopMenu.Add(new()
        {
            Title = "Count 10",
            Link = BaseURL.Append("/mod10")
        });
    }

    protected override void ConfigureLive()
    {
        //Live testing on root
        BaseURL = "https://localhost:7127/";
    }
}
