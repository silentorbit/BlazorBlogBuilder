using BlogDemo.Components;
using SilentOrbit.BlazorBlogBuilder.Config;

namespace BlogDemo;

public class BlogConfig : SiteConfig<App> //Inherit the generic version of the base class
{
    public BlogConfig()
    {
        BaseURL = "https://www.silentorbit.com/blazor-blog-builder/blog/";

        Head.Language = new() { Code = "en" };
        Head.Favicon = new()
        {
            URL = "https://www.silentorbit.com/favicon.png",
            MimeType = "image/png"
        };

        //TimeZone, hardcode your local ID to get a consistent build, regardless where it's building.
        var timeZoneID = TimeZoneInfo.Local.Id;
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);

        Title = "Blog Demo";
        Description = "This is a Blog built using BlazorBlogBuilder";

        CommentEmail = "blog-demo@silentorbit.com";
        DefaultRobots = new()
        {
            NoTranslate = true,
        };
        DefaultSitemap = true;
        Author = new() { Name = "Blog Author", };

        BuildConfig.Markdown.All = true;
        BuildConfig.Markdown.Page = false;

        BuildConfig.AfterBuild = AfterBuildConfig.LaunchBrowser;
    }
}
