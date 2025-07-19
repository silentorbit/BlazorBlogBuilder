using Microsoft.AspNetCore.Mvc.RazorPages;
using SilentOrbit.StaticOnline.BlazorRendering;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

/// <summary>
/// Generate other file content
/// </summary>
abstract class FileGeneratorBase
{
    internal SiteBuilder Builder { get; set; } = null!;

    protected SiteConfig Config => Builder.Config;

    public abstract void Init();

    protected void AddGenerator(RelUrl url)
    {
        var page = Builder.Pages.GetOrCreate(url);
        page.BuildLast = true;
        page.Generator = this;
        page.BuildStage = BuildStage.PreScan; //Ready for FinalBuild
    }

    public abstract Task<string> Generate(RelUrl url);
}
