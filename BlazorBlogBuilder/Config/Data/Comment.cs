using Microsoft.AspNetCore.Components;

namespace SilentOrbit.BlazorBlogBuilder.Config.Data;

public class Comment
{
    public MarkupString Text { get; set; }
    public string Author { get; set; } = null!;

    public override int GetHashCode()
        => (Author?.GetHashCode() ?? 0) + Text.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is Comment c)
            return c.Text.Value == Text.Value && c.Author == Author;
        return false;
    }
}
