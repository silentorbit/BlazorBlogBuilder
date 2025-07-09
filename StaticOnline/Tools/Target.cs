using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;

namespace SilentOrbit.StaticOnline.Tools;

class Target(SiteBuilder site, DirPath targetDir)
{
    readonly DirPath rootDir = targetDir;
    readonly Url baseURL = site.Config.BaseURL;

    internal void Store(Url url, string content)
    {
        var urlPath = url.GetRelativePath(baseURL);

        var ext = Path.GetExtension(urlPath);
        FilePath target;
        if (ext == "")
            target = rootDir.CombineFile(urlPath, "index.html");
        else
            target = rootDir.CombineFile(urlPath);

        if (target.Exists())
            throw new Exception("File already exists: " + target);

        target.WriteAllText(content);
    }

    public void StoreStatic(Url url, FilePath file)
    {
        Debug.Assert(url != "https://www.silentorbit.com/blog/tags");
        var urlPath = url.GetRelativePath(baseURL);
        var target = rootDir.CombineFile(urlPath);
        target.Parent.CreateDirectory();
        file.CopyTo(target);
    }

}
