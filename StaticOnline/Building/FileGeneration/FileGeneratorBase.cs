namespace SilentOrbit.StaticOnline.Building.FileGeneration;

/// <summary>
/// Generate other file content
/// </summary>
abstract class FileGeneratorBase
{
    internal SiteBuilder Site { get; set; } = null!;
    
    protected SiteConfig Config => Site.Config;

    public abstract RelUrl URL { get; }

    public virtual void Init() { }

    public abstract string? Generate();
}
