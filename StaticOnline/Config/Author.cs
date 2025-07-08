namespace SilentOrbit.StaticOnline.Config;

public class Author
{
    public string Name { get; set; } = null!;
    public Url? URL { get; set; }
    public Url? Avatar { get; set; }
    public string? Email { get; set; }
}
