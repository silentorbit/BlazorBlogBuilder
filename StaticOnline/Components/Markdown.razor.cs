//using MarkdownSharp;
using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Components;

public partial class Markdown : ChildContentBase
{
    /*static readonly MarkdownSharp.Markdown markdownSharp;

    static Markdown()
    {
        var options = new MarkdownOptions();
        markdownSharp = new(options);
    }*/

    MarkupString? html;

    protected override async Task OnChildContentParametersSetAsync(PageData page)
    {
        var c = await GetChildContent();
        html = Transform(c);
    }

    #region Static

    public static bool UseMarkdown(bool? markdown, PageData page)
    {
        //Save work during PreScan.
        if (page.BuildStage != BuildStage.FinalBuild)
            return false;

        if (markdown != null)
            return markdown.Value;

        if (page.Markdown != null)
            return page.Markdown.Value;

        var markdownDefault = SiteBuilder.Instance.Config.BuildConfig.Markdown;
        if (page.BlazorType?.IsAssignableTo(typeof(BlogPost)) == true)
            return markdownDefault.BlogPost;

        return markdownDefault.Page;
    }

    /// <summary>
    /// Determine if a layout should be applied at the MainLayout.
    /// Will assume <see cref="BlogPost"/> markdown is generated at another level.
    /// </summary>
    public static bool UseMarkdownLayout(PageData page)
    {
        //Don't transform during PreScan.
        if (page.BuildStage != BuildStage.FinalBuild)
            return false;

        //BlogPosts are rendered in a special Post.razor page defined by the website.
        //Known issue, BlogPosts with custom 
        if (page.BlazorType?.IsAssignableTo(typeof(BlogPost)) == true)
            return false;

        if (page.Markdown != null)
            return page.Markdown.Value;

        return SiteBuilder.Instance.Config.BuildConfig.Markdown.Page;
    }

    public static MarkupString? Transform(MarkupString? content)
    {
        if (content == null)
            return null;

        return (MarkupString)Transform(content.Value.Value);
    }

    [return: NotNullIfNotNull(nameof(content))]
    public static string? Transform(string? content)
    {
        if (content == null)
            return null;

        var md = TrimBlazorSpace(content);
        var html = Markdig.Markdown.ToHtml(md);
        //var sharp = markdownSharp.Transform(html);
        return html;
    }

    /// <summary>
    /// Trim indentation added by Blazor to get correct Markdown code.
    /// </summary>
    static string TrimBlazorSpace(string raw)
    {
        var lines = raw.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        //Find common indentation.
        string? indent = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var re = ReLeadingWhitespace().Match(line);
            Debug.Assert(re.Success);
            var lineIndent = re.Groups[0].Value;
            if (indent == null || lineIndent.Length < indent.Length)
            {
                Debug.Assert(
                    indent == null ||
                    lineIndent.StartsWith(indent),
                    "New indent is using other characters");

                indent = lineIndent;
                //if (indent == "")
                //    break; //Optimization, no need to look for shorter indents
            }
        }

        indent ??= "";

        //Trim all lines
        var sb = new StringBuilder();
        using var md = new StringWriter(sb) { NewLine = "\n" };
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                md.WriteLine();
            }
            else
            {
                if (line.StartsWith(indent))
                    md.WriteLine(line.Substring(indent.Length));
                else
                    throw new NotImplementedException();
            }
        }

        var markdown = md.ToString();
        markdown = markdown.Trim(['\n', '\r']);
        return markdown;
    }

    [GeneratedRegex("^[ \t]*")]
    private static partial Regex ReLeadingWhitespace();

    #endregion
}
