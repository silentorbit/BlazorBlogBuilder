using SilentOrbit.StaticOnline.Building.FileGeneration;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Generate other types of files
/// such as feeds and sitemaps
/// </summary>
class FileBuilder
{
    readonly SiteBuilder site;

    public FileBuilder(SiteBuilder site)
    {
        this.site = site;
    }

    FileGeneratorBase[] files = null!;

    public void Scan()
    {
        files = GetGenerators().ToArray();
        foreach (var file in files)
        {
            file.Init();
            site.Pages.AddIndex(file);
        }
    }

    public void Generate()
    {
        foreach (var file in files)
        {
            var content = file.Generate();
            if (content == null)
                continue;

            site.Target.Store(file.URL, content);
            site.Pages.DoneStatic(file.URL);
        }
    }

    public IEnumerable<FileGeneratorBase> GetGenerators()
    {
        //Scan site assmebly
        var types = site.Config.Type.Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAssignableTo(typeof(FileGeneratorBase)))
            {
                var file = (FileGeneratorBase)Activator.CreateInstance(type)!;
                file.Site = site;
                yield return file;
            }
        }

        //Scan this assmebly
        types = GetType().Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAbstract)
                continue;
            if (type.IsAssignableTo(typeof(FileGeneratorBase)))
            {
                var file = (FileGeneratorBase)Activator.CreateInstance(type)!;
                file.Site = site;
                yield return file;
            }
        }
    }
}
