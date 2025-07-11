using MarkdownSharp;
using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Components;

public partial class Markdown
{
    static readonly MarkdownSharp.Markdown markdownSharp;

    static Markdown()
    {
        var options = new MarkdownOptions();
        markdownSharp = new(options);
    }

    [EditorRequired]
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    public SiteBuilder Site { get; set; } = null!;

    MarkupString html;

    protected override async Task OnParametersSetAsync()
    {
        var raw = await Site.Blazor.RenderFragment(ChildContent);

        //Trim whitespace from the markdown generated
        var markdown = TrimBlazorSpace(raw!);

        var sharp = markdownSharp.Transform(markdown);

        html = new MarkupString(sharp);
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

            var re = new Regex("^[ \t]+").Match(line);
            Debug.Assert(re.Success);
            var lineIndent = re.Groups[0].Value;
            if (indent == null || indent.Length < lineIndent.Length)
            {
                Debug.Assert(
                    indent == null ||
                    lineIndent.StartsWith(indent),
                    "New indent is using other characters");

                indent = lineIndent;
            }
        }

        Debug.Assert(indent != null);
        indent ??= "";

        //Trim all lines
        var md = new StringBuilder();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                md.AppendLine();
            }
            else
            {
                if (line.StartsWith(indent))
                    md.AppendLine(line.Substring(indent.Length));
                else
                    throw new NotImplementedException();
            }
        }

        var markdown = md.ToString();
        markdown = markdown.Trim(['\n', '\r']);
        return markdown;
    }
}
