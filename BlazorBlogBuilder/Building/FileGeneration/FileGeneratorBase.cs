namespace SilentOrbit.BlazorBlogBuilder.Building.FileGeneration;

/// <summary>
/// Generate other file content
/// </summary>
abstract class FileGeneratorBase
{
    internal SiteBuilder Builder => SiteBuilder.Instance;

    protected SiteConfig Config => SiteBuilder.Instance.Config;

    protected void AddGenerator(RelUrl url)
    {
        var page = Builder.Pages.GetOrCreate(url);
        page.BuildLast = true;
        page.Generator = this;
        page.BuildStage = BuildStage.PreScan; //Ready for FinalBuild
    }

    public abstract Task<string> Generate(RelUrl url);
}
