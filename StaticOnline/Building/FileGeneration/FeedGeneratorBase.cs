using SilentOrbit.StaticOnline.BlazorRendering;
using SilentOrbit.StaticOnline.Config.Data;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

abstract class FeedGeneratorBase : FileGeneratorBase
{
    protected abstract string path { get; }

    protected abstract Task<string> GenerateFeed(RelUrl url, IEnumerable<PageData> posts);

    public sealed override Task<string> Generate(RelUrl url)
    {
        if (url.Href == path)
            InitTags(url);

        var parts = url.Href.Split('.');
        if (parts.Length == 2)
            return GenerateFeed(url, Builder.Pages.Feed);

        if (parts.Length == 3)
        {
            var id = parts[1];
            var tag = Tag.ByID(id);
            return GenerateFeed(url, Builder.Pages.Feed.Where(p=>p.Tags.Contains(tag)));
        }

        throw new NotImplementedException(url.Href);
    }

    void InitTags(RelUrl url)
    {
        var filename = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);

        foreach (var tag in Builder.Tags.GetTags())
        {
            var name = $"{filename}.{tag.ID}{ext}";
            var tagURL = Config.BaseURL.Append(name);
            AddGenerator(tagURL);
        }
    }

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
                var html = await new BlazorRenderer(Builder, post).RenderComponent(); //Only render the component
                return html;
        }
    }
}
