using SilentOrbit.Disk;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Copy static files from wwwroot to output.
/// </summary>
class WWWRootBuilder(SiteBuilder site)
{
    readonly DirPath sourceBase = site.Config.WwwRoot ??
            new FilePath(site.Config.Type.Assembly.Location)
            .Parent.CombineDir("wwwroot");

    internal void Build()
    {
        var source = site.Config.WwwRoot ??
            new FilePath(site.Config.Type.Assembly.Location)
            .Parent.CombineDir("wwwroot");
        Debug.Assert(source.Exists());

        var target = site.Config.TargetDir;

        //Copy all files
        Copy(source, target);
    }

    void Copy(DirPath source, DirPath target)
    {
        foreach (var file in source.GetFiles())
        {
            var url = site.Config.BaseURL.Append(file - sourceBase);
            var hashUrl = site.Hasher.GetHashPath(file, url);
            site.Target.StoreStatic(hashUrl, file);
            site.Pages.DoneStatic(hashUrl);
        }

        //Subdirectories
        foreach (var dir in source.GetDirectories())
            Copy(dir, target.CombineDir(dir.Name));
    }
}
