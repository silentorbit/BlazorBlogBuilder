namespace SilentOrbit.StaticOnline.Config;

/// <summary>
/// Configures the <html><head> Content.
/// When defined on individual pages this overrides
/// the default at <see cref="SiteConfig.Head"></see>
/// </summary>
public class HeaderConfig
{
    /// <summary>
    /// <html lang="xx"></html>
    /// </summary>
    public Language Language { get; set; } = new()
    {
        Code = "en"
    };

    public Favicon? Favicon { get; set; }

    /// <summary>
    /// See: https://indielogin.com/setup
    /// 
    /// Examples:
    /// mailto:email@example.com
    /// https://github.com/username
    /// </summary>
    public Url? IndieLogin { get; set; }

    /// <summary>
    /// <link rel="webmention" href="...
    /// </summary>
    public Url? WebMention { get; set; }
}
