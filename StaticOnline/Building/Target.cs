namespace SilentOrbit.StaticOnline.Building;

class Target(SiteBuilder site)
{
    readonly DirPath rootDir = site.Config.Target;
    
    internal void Store(Url url, string content)
    {
        var urlPath = site.Config.BaseURL.GetRelativePath(url);

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
        var urlPath = site.Config.BaseURL.GetRelativePath(url);
        var target = rootDir.CombineFile(urlPath);
        target.Parent.CreateDirectory();
        file.CopyTo(target);
    }

}
