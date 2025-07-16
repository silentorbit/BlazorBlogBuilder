namespace SilentOrbit.StaticOnline.Building;

class Target(SiteConfig config)
{
    readonly DirPath rootDir = config.BuildConfig.Target;
    
    internal void Store(Url url, string content)
    {
        var urlPath = config.BaseURL.GetRelativePath(url);

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
        var urlPath = config.BaseURL.GetRelativePath(url);
        var target = rootDir.CombineFile(urlPath);
        target.Parent.CreateDirectory();
        file.CopyTo(target);
    }

}
