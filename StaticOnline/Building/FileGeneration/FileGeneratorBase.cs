namespace SilentOrbit.StaticOnline.Building.FileGeneration;

/// <summary>
/// Generate other file content
/// </summary>
abstract class FileGeneratorBase
{
    internal SiteBuilder Builder { get; set; } = null!;
    
    protected SiteConfig Config => Builder.Config;

    public abstract RelUrl URL { get; }

    public virtual void Init() { }

    public abstract string? Generate();
}
