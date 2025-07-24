using BlazorDemo.Components;
using SilentOrbit.StaticOnline.Config;

namespace BlazorDemo;

public class BlazorDemoConfig : SiteConfig<App>
{
    public BlazorDemoConfig()
    {
        //Build static files to be served in the subdirectory
        BaseURL = "https://www.silentorbit.com/static-online/blazor/";

        Head.Language = new() { Code = "en" };
        Head.Favicon = new()
        {
            URL = "https://www.silentorbit.com/favicon.png",
            MimeType = "image/png"
        };

        //TimeZone, hardcode your local ID to get a consistent build, regardless where it's building.
        var timeZoneID = TimeZoneInfo.Local.Id;
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);

        Title = "Demo";
        Description = "This is a Demo";

        CommentEmail = "demo@silentorbit.com";
        DefaultRobots = new()
        {
            NoTranslate = true,
        };
        DefaultSitemap = true;
        Author = new() { Name = "Demo", };

        BuildConfig.Markdown.All = true;
        BuildConfig.Markdown.Page = false;

        TopMenu = new() {
            new() { Title = "Blog",     Link = BaseURL.Append("/")    },
            new() { Title = "Tags",     Link = BaseURL.Append("/tags")    },
            new() { Title = "Count 10", Link = BaseURL.Append("/mod10")    }
        };

    }

}
