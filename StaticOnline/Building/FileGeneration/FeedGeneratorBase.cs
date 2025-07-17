using SilentOrbit.StaticOnline.BlazorRendering;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

abstract class FeedGeneratorBase : FileGeneratorBase
{
    public async Task<string?> GetPostContent(PageData post)
    {
        switch (Config.FeedContent)
        {
            default:
            case FeedContent.None:
                return null;
            case FeedContent.Summary:
                return post.Summary?.Value;
            case FeedContent.Full:
                var html = await new BlazorRenderer(Builder, post).RenderComponent(); //Only render the component
                return html;
        }
    }
}
