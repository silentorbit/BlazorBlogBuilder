using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Components;

partial class StaticBlazorGeneratorHeaders
{
    static readonly MarkupString nl = new("\n    ");

    static MarkupString FeedLink(FeedList.Item feed)
    {
        var link = new MarkupString(
            @$"<link rel=""alternate"" type=""{feed.MimeType}"" title=""{HttpUtility.HtmlEncode(feed.Title)}"" href=""{HttpUtility.HtmlEncode(feed.URL)}"" />");
        return link;
    }
}
