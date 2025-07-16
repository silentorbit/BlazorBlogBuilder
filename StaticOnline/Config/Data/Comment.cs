using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Config.Data;

public class Comment
{
    public MarkupString Text { get; set; }
    public string Author { get; set; } = null!;
}
