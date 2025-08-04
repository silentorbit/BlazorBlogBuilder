namespace SilentOrbit.BlazorBlogBuilder.Building.BlazorRendering;

public static partial class HtmlCleanup
{
    public static string Clean(string html, SiteConfig config)
    {
        //Remove Blazor attributes <... b-az09 />
        if (config.BuildConfig.StripBlazorAttributes)
            html = ReReplaceBlazorAttributes().Replace(html, "$1");

        //Remove Blazor comments: <!--/bl:15-->
        html = ReRemoveBlazorComments().Replace(html, "");

        //Remove double newline
        html = ReReplaceNewline().Replace(html, "\n");
        return html;
    }

    [GeneratedRegex(@" b-[a-z0-9]+([ />])")]
    private static partial Regex ReReplaceBlazorAttributes();

    [GeneratedRegex(@"<!--/?bl:\d+-->")]
    private static partial Regex ReRemoveBlazorComments();

    [GeneratedRegex(@"[\r\n][\r\n]+")]
    private static partial Regex ReReplaceNewline();

}
