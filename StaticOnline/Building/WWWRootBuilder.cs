using SilentOrbit.Disk;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Copy static files from wwwroot to output.
/// </summary>
class WWWRootBuilder
{
    readonly SiteBuilder site;
    readonly DirPath targetRoot;

    public WWWRootBuilder(SiteBuilder site)
    {
        this.site = site;
        targetRoot = site.Config.Target;
    }

    /// <summary>
    /// Copy all files from wwwroot
    /// </summary>
    internal void Build()
    {
        CopyDir(site.Config.WwwRoot, targetRoot);
    }

    void CopyDir(DirPath source, DirPath target)
    {
        foreach (var file in source.GetFiles())
        {
            var relDiskPath = file - site.Config.WwwRoot;
            var path = relDiskPath.RelativePath.Replace('\\', '/');
            var url = site.Config.BaseURL.Append(path);
            var hashUrl = site.Hasher.GetHashPath(file, url);
            site.Target.StoreStatic(hashUrl, file);
            site.Pages.DoneStatic(hashUrl);
        }

        //Subdirectories
        foreach (var dir in source.GetDirectories())
            CopyDir(dir, target.CombineDir(dir.Name));
    }
}
