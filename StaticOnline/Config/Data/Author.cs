namespace SilentOrbit.StaticOnline.Config.Data;

public class Author
{
    public string Name { get; set; } = null!;
    public Url? URL { get; set; }
    public RelUrl? Avatar { get; set; }
    public string? Email { get; set; }
    
    public Url? GitHub { get; set; }
    public Url? Twitter { get; set; }
    public Url? Facebook { get; set; }
    public Url? Instagram { get; set; }

    /// <summary>
    /// https://indielogin.com/setup
    /// mailto:email@example.com
    /// https://github.com/username
    /// </summary>
    public Url? IndieLogin { get; set; }
}
