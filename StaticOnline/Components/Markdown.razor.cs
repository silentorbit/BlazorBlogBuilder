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

    public static MarkupString? Transform(MarkupString? content)
    {
        if (content == null)
            return null;

        var md = TrimBlazorSpace(content.Value.Value);
        var html = Markdig.Markdown.ToHtml(md);
        //var sharp = markdownSharp.Transform(html);
        return (MarkupString)html;
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
}
