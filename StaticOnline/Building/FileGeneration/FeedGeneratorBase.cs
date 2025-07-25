using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Components;
using SilentOrbit.StaticOnline.Config.Data;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

abstract class FeedGeneratorBase : FileGeneratorBase
{
    protected abstract string Filename { get; }
    protected abstract string MimeType { get; }

    const string feedPath = "feed/";

    public static void Init()
    {
        new RssFeed().InitFeed();
        new AtomFeed().InitFeed();
        new JsonFeed().InitFeed();
    }

    void InitFeed()
    {
        var config = SiteBuilder.Instance.Config;
        var url = config.BaseURL.Append(feedPath + Filename);
        config.Head.Feed!.Set(MimeType, config.Title, url);
        AddGenerator(url);
    }

    internal static void Init(Tag tag)
    {
        new RssFeed().InitTag(tag);
        new AtomFeed().InitTag(tag);
        new JsonFeed().InitTag(tag);
    }

    void InitTag(Tag tag)
    {
        var filename = Path.GetFileNameWithoutExtension(Filename);
        var ext = Path.GetExtension(Filename);

        var tagURL = Config.BaseURL.Append($"{feedPath}{filename}.{tag.ID}{ext}");
        tag.Feed.Set(MimeType, tag.Name, tagURL);

        AddGenerator(tagURL);
    }

    public sealed override Task<string> Generate(RelUrl url)
    {
        var parts = url.Href.Split('.');
        if (parts.Length == 2)
            return GenerateFeed(url, Builder.Pages.Feed);

        if (parts.Length == 3)
        {
            var id = parts[1];
            var tag = Tag.ByID(id);
            return GenerateFeed(url, Builder.Pages.Feed.Where(p => p.Tags.Contains(tag)));
        }

        throw new NotImplementedException(url.Href);
    }

    protected abstract Task<string> GenerateFeed(RelUrl url, IEnumerable<PageData> posts);

    public async Task<string?> GetPostContent(PageData post)
    {
        switch (Config.FeedContent)
        {
            default:
            case FeedContent.None:
                return null;

            case FeedContent.Summary:
                return post.Summary?.Value + @$"<p><a href=""{post.URL}"">Read more...</a></p>";

            case FeedContent.Full:

                //Updates only show <Update> summary
                if (post.IsUpdate)
                    return post.Summary?.Value + @$"<p><a href=""{post.URL}"">Read more...</a></p>";

                //Generate post content
                var html = await new BlazorRenderer(Builder, post).RenderComponent();

                if (Markdown.UseMarkdown(null, post))
                    html = Markdown.Transform(html);

                return html;
        }
    }


}
