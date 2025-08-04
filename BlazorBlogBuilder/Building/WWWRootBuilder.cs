namespace SilentOrbit.BlazorBlogBuilder.Building;

/// <summary>
/// Copy static files from wwwroot to output.
/// </summary>
class WWWRootBuilder
{
    readonly SiteBuilder builder;
    readonly DirPath targetRoot;

    public WWWRootBuilder(SiteBuilder builder)
    {
        this.builder = builder;
        targetRoot = builder.Config.BuildConfig.Target;
    }

    /// <summary>
    /// Copy all files from wwwroot
    /// </summary>
    internal void Build()
    {
        CopyDir(builder.Config.BuildConfig.WwwRoot, targetRoot);
    }

    void CopyDir(DirPath source, DirPath target)
    {
        foreach (var file in source.GetFiles())
        {
            var relDiskPath = file - builder.Config.BuildConfig.WwwRoot;
            var path = relDiskPath.RelativePath.Replace('\\', '/');
            var url = builder.Config.BaseURL.Append(path);
            builder.Target.Store(url, file);
            builder.Pages.DoneStatic(url);
        }

        //Subdirectories
        foreach (var dir in source.GetDirectories())
            CopyDir(dir, target.CombineDir(dir.Name));
    }
}
