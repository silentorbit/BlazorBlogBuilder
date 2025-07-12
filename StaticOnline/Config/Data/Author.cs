namespace SilentOrbit.StaticOnline.Config.Data;

public class Author
{
    public string Name { get; set; } = null!;
    public Url? URL { get; set; }
    public RelUrl? Avatar { get; set; }
    public string? Email { get; set; }
}
