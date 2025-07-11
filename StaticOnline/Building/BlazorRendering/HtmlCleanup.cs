namespace SilentOrbit.StaticOnline.Building.BlazorRendering;

public static partial class HtmlCleanup
{
    public static string Clean(string html)
    {
        //Remove blazor attributes
        html = ReReplaceBlazorAttributes().Replace(html, "$1");

        //Remove double newline
        html = ReReplaceNewline().Replace(html, "\n");
        return html;
    }

    [GeneratedRegex(@" b-[a-z0-9]+([ />])")]
    private static partial Regex ReReplaceBlazorAttributes();

    [GeneratedRegex(@"[\r\n][\r\n]+")]
    private static partial Regex ReReplaceNewline();

}
