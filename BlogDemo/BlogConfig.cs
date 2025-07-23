using BlogDemo.Components;
using SilentOrbit.StaticOnline.Config;

namespace BlogDemo;

public class BlogConfig : SiteConfig<App> //Inherit the generic version of the base class
{
    public BlogConfig()
    {
        BaseURL = "https://www.silentorbit.com/static-online/blog/";

        Head.Language = new() { Code = "en" };
        Head.Favicon = new()
        {
            URL = "https://www.silentorbit.com/favicon.png",
            MimeType = "image/png"
        };

        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        Title = "Blog Demo";
        Description = "This is a Blog built using StaticOnline";

        CommentEmail = "blog-demo@silentorbit.com";
        DefaultRobots = new()
        {
            NoTranslate = true,
            Sitemap = true,
        };
        Author = new() { Name = "Blog Author", };

        BuildConfig.Markdown.All = true;
        BuildConfig.Markdown.Page = false;

        BuildConfig.AfterBuild = AfterBuildConfig.LaunchBrowser;
    }
}
