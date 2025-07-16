namespace SilentOrbit.StaticOnline.Building;

class Target(SiteConfig config)
{
    internal void Store(Url url, string content)
    {
        var target = GetTarget(url);
        target.WriteAllText(content);
    }

    internal void Store(Url url, byte[] content)
    {
        var target = GetTarget(url);
        target.WriteAllBytes(content);
    }

    FilePath GetTarget(Url url)
    {
        var urlPath = config.BaseURL.GetRelativePath(url);
        var rootDir = config.BuildConfig.Target;

        var ext = Path.GetExtension(urlPath);
        FilePath target;
        if (ext == "")
            target = rootDir.CombineFile(urlPath, "index.html");
        else
            target = rootDir.CombineFile(urlPath);

        if (target.Exists())
            throw new Exception("File already exists: " + target);

        return target;
    }


    public void StoreStatic(Url url, FilePath file)
    {
        var urlPath = config.BaseURL.GetRelativePath(url);
        var target = config.BuildConfig.Target.CombineFile(urlPath);
        target.Parent.CreateDirectory();
        file.CopyTo(target);
    }

}
