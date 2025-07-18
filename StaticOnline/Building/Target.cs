namespace SilentOrbit.StaticOnline.Building;

class Target(SiteConfig config)
{
    internal void Store(RelUrl url, FilePath file)
    {
        var target = CreateTarget(url);
        target.Parent.CreateDirectory();
        file.CopyTo(target);
    }

    internal void Store(RelUrl url, string content)
    {
        var target = CreateTarget(url);
        target.WriteAllText(content);
    }

    internal void Store(RelUrl url, byte[] content)
    {
        var target = CreateTarget(url);
        target.WriteAllBytes(content);
    }

    internal FilePath GetTarget(RelUrl url)
    {
        var rootDir = config.BuildConfig.Target;

        FilePath target;

        var ext = Path.GetExtension(url.Href);
        if (ext == "")
            target = rootDir.CombineFile(url.Href, "index.html");
        else
            target = rootDir.CombineFile(url.Href);

        return target;
    }

    FilePath CreateTarget(RelUrl url)
    {
        var target = GetTarget(url);

        if (target.Exists())
            throw new Exception("File already exists: " + target);

        return target;
    }

}
